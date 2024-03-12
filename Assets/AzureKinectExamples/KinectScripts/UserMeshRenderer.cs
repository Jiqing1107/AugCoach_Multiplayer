﻿using UnityEngine;
using System.Collections;
using com.rfilkov.kinect;


namespace com.rfilkov.components
{
    /// <summary>
    /// UserMeshRenderer renders virtual mesh of a user in the scene, as detected by the given sensor.
    /// </summary>
    public class UserMeshRenderer : MonoBehaviour
    {
        [Tooltip("Index of the player, tracked by this component. 0 - the 1st player only, 1 - the 2nd player only, etc.")]
        public int playerIndex = 0;

        [Tooltip("Depth sensor index - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        [Tooltip("Resolution of the images used to generate the scene.")]
        private DepthSensorBase.PointCloudResolution sourceImageResolution = DepthSensorBase.PointCloudResolution.DepthCameraResolution;

        [Tooltip("Whether to show the mesh as point cloud, or as solid mesh.")]
        public bool showAsPointCloud = true;

        [Tooltip("Time interval between scene mesh updates, in seconds. 0 means no wait.")]
        private float updateMeshInterval = 0f;

        //[Tooltip("Whether to update the mesh collider as well, when the user mesh changes.")]
        //public bool updateMeshCollider = false;

        [Tooltip("Time interval between mesh-collider updates, in seconds. 0 means no mesh-collider updates.")]
        private float updateColliderInterval = 0f;


        // reference to object's mesh
        private Mesh mesh = null;

        // references to KM and data
        private KinectManager kinectManager = null;
        private KinectInterop.SensorData sensorData = null;
        private DepthSensorBase sensorInt = null;
        private Vector3 spaceScale = Vector3.zero;

        // render textures 
        private RenderTexture colorTexture = null;
        private RenderTexture colorTextureBuildMesh = null;
        private RenderTexture colorTextureUpdateMesh = null;
        private bool colorTextureCreated = false;

        private bool bColorCamRes = false;
        private ushort[] tDepthImage = null;
        private byte[] tBodyIndexImage = null;

        // times
        private ulong lastSpaceCoordsTime = 0;
        private float lastMeshUpdateTime = 0f;
        private float lastColliderUpdateTime = 0f;

        // user parameters
        //private ulong prevUserId = 0;
        private ulong userId = 0;
        private byte userBodyIndex = 255;
        private Vector3 userBodyPos = Vector3.zero;

        // image parameters
        private int imageWidth = 0;
        private int imageHeight = 0;

        // mesh parameters
        private Vector3[] spaceTable = null;
        private Vector3[] meshVertices = null;
        private int[] meshIndices = null;
        private byte[] meshVertUsed = null;
        private bool bMeshInited = false;

        // thread parameters
        private System.Threading.Thread buildMeshThread = null;
        private bool bStopThread = false;

        private bool bBuildMesh = false;
        private bool bUpdateMesh = false;


        void Start()
        {
            // get sensor data
            kinectManager = KinectManager.Instance;
            sensorData = (kinectManager != null && kinectManager.IsInitialized()) ? kinectManager.GetSensorData(sensorIndex) : null;

            buildMeshThread = new System.Threading.Thread(new System.Threading.ThreadStart(BuildMesh));
            buildMeshThread.IsBackground = true;
            buildMeshThread.Start();
        }


        void OnDestroy()
        {
            if (buildMeshThread != null)
            {
                // stop the thread
                bStopThread = true;
                buildMeshThread.Join();
                buildMeshThread = null;
            }

            if (bMeshInited)
            {
                // release the mesh-related resources
                FinishMesh();
            }
        }


        void LateUpdate()
        {
            if (mesh == null && sensorData != null && sensorData.depthCamIntr != null)
            {
                // init mesh and its related data
                InitMesh();
            }

            if (bMeshInited)
            {
                // user params
                userId = kinectManager.GetUserIdByIndex(playerIndex);
                userBodyIndex = (byte)(userId != 0 ? kinectManager.GetBodyIndexByUserId(userId) : 255);
                userBodyPos = userId != 0 ? kinectManager.GetUserKinectPosition(userId, true) : Vector3.zero;

                // update the mesh
                UpdateMesh();
            }
        }


        // inits the mesh and related data
        private void InitMesh()
        {
            // create mesh
            mesh = new Mesh
            {
                name = "User" + playerIndex + "Mesh-S" + sensorIndex,
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
            }
            else
            {
                Debug.LogWarning("MeshFilter not found! You may not see the mesh on screen");
            }

            if (sensorData != null && sensorData.sensorInterface != null)
            {
                sensorInt = (DepthSensorBase)sensorData.sensorInterface;

                Vector2Int imageRes = Vector2Int.zero;
                if (sensorInt.pointCloudColorTexture == null)
                {
                    sensorInt.pointCloudResolution = sourceImageResolution;
                    imageRes = sensorInt.GetPointCloudTexResolution(sensorData);

                    colorTexture = KinectInterop.CreateRenderTexture(colorTexture, imageRes.x, imageRes.y, RenderTextureFormat.ARGB32);
                    sensorInt.pointCloudColorTexture = colorTexture;
                    colorTextureCreated = true;
                }
                else
                {
                    imageRes = sensorInt.GetPointCloudTexResolution(sensorData);
                    colorTexture = sensorInt.pointCloudColorTexture;
                    colorTextureCreated = false;
                }

                bColorCamRes = sensorInt.pointCloudResolution == DepthSensorBase.PointCloudResolution.ColorCameraResolution;
                if (bColorCamRes)
                {
                    // enable transformed depth & BI frames
                    tDepthImage = new ushort[sensorData.colorImageWidth * sensorData.colorImageHeight];
                    tBodyIndexImage = new byte[sensorData.colorImageWidth * sensorData.colorImageHeight];

                    sensorData.sensorInterface.EnableColorCameraDepthFrame(sensorData, true);
                    sensorData.sensorInterface.EnableColorCameraBodyIndexFrame(sensorData, true);
                }
                else
                {
                    tDepthImage = new ushort[sensorData.depthImageWidth * sensorData.depthImageHeight];
                    tBodyIndexImage = new byte[sensorData.depthImageWidth * sensorData.depthImageHeight];
                }

                // create copy texture
                colorTextureBuildMesh = KinectInterop.CreateRenderTexture(colorTextureBuildMesh, imageRes.x, imageRes.y, RenderTextureFormat.ARGB32);
                colorTextureUpdateMesh = KinectInterop.CreateRenderTexture(colorTextureUpdateMesh, imageRes.x, imageRes.y, RenderTextureFormat.ARGB32);

                // set the color texture
                Renderer meshRenderer = GetComponent<Renderer>();
                if (meshRenderer && meshRenderer.material && meshRenderer.material.mainTexture == null)
                {
                    meshRenderer.material.mainTexture = colorTextureUpdateMesh; // sensorInt.pointCloudColorTexture;
                    //meshRenderer.material.SetTextureScale("_MainTex", kinectManager.GetColorImageScale(sensorIndex));
                }

                // image width & height
                imageWidth = imageRes.x;
                imageHeight = imageRes.y;
                int pointCount = imageWidth * imageHeight;

                // mesh arrays
                meshVertices = new Vector3[pointCount];
                meshIndices = new int[6 * pointCount];  // 2 triangles per vertex, last row and column excluded
                meshVertUsed = new byte[pointCount];
                spaceScale = kinectManager.GetSensorSpaceScale(sensorIndex);

                // create space table
                spaceTable = sensorInt.pointCloudResolution == DepthSensorBase.PointCloudResolution.DepthCameraResolution ?
                    sensorInt.GetDepthCameraSpaceTable(sensorData) : sensorInt.GetColorCameraSpaceTable(sensorData);

                // init mesh uv array
                Vector2[] meshUv = new Vector2[pointCount];

                for (int y = 0, i = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++, i++)
                    {
                        Vector2 imagePos = new Vector2(x, y);
                        meshUv[i] = new Vector2(imagePos.x / imageWidth, imagePos.y / imageHeight);
                    }
                }

                mesh.vertices = meshVertices;
                mesh.SetIndices(meshIndices, MeshTopology.Triangles, 0);
                mesh.uv = meshUv;

                if (showAsPointCloud)
                {
                    meshIndices = new int[pointCount];
                    for (int i = 0; i < pointCount; i++)
                        meshIndices[i] = i;
                    mesh.SetIndices(meshIndices, MeshTopology.Points, 0);
                }

                bMeshInited = true;
            }
        }


        // releases mesh-related resources
        private void FinishMesh()
        {
            if (sensorInt)
            {
                if (sensorInt.pointCloudResolution == DepthSensorBase.PointCloudResolution.ColorCameraResolution)
                {
                    sensorData.sensorInterface.EnableColorCameraDepthFrame(sensorData, false);
                    sensorData.sensorInterface.EnableColorCameraBodyIndexFrame(sensorData, false);
                }

                sensorInt.pointCloudColorTexture = null;
            }

            if (colorTexture && colorTextureCreated)
            {
                colorTexture.Release();
                colorTexture = null;
            }

            if (colorTextureBuildMesh)
            {
                colorTextureBuildMesh.Release();
                colorTextureBuildMesh = null;
            }

            if (colorTextureUpdateMesh)
            {
                colorTextureUpdateMesh.Release();
                colorTextureUpdateMesh = null;
            }

            spaceTable = null;
            tDepthImage = null;
            tBodyIndexImage = null;

            meshVertices = null;
            meshIndices = null;
            meshVertUsed = null;

            bMeshInited = false;
        }


        // updates the mesh according to current depth frame
        private void UpdateMesh()
        {
            if (!bBuildMesh && (Time.time - lastMeshUpdateTime) >= updateMeshInterval)
            {
                lastMeshUpdateTime = Time.time;

                // copy the texture
                Graphics.CopyTexture(colorTexture, colorTextureBuildMesh);

                if (bColorCamRes)
                {
                    // get transformed depth & BI frames, if needed
                    ulong tDepthFrameTime = 0;
                    sensorData.sensorInterface.GetColorCameraDepthFrame(sensorData, ref tDepthImage, ref tDepthFrameTime);

                    ulong tBiFrameTime = 0;
                    sensorData.sensorInterface.GetColorCameraBodyIndexFrame(sensorData, ref tBodyIndexImage, ref tBiFrameTime);
                    //Debug.Log("UpdateMesh - color time: " + sensorData.lastColorFrameTime + ", tdepth time: " + tDepthFrameTime + ", bi time: " + tBiFrameTime);
                }
                else
                {
                    // copy depth and BI frames
                    KinectInterop.CopyBytes(sensorData.depthImage, sizeof(ushort), tDepthImage, sizeof(ushort));
                    KinectInterop.CopyBytes(sensorData.bodyIndexImage, sizeof(byte), tBodyIndexImage, sizeof(byte));
                    //Debug.Log("UpdateMesh - tcolor time: " + sensorData.lastDepthCamColorFrameTime + ", depth time: " + sensorData.lastDepthFrameTime + ", bi time: " + sensorData.lastBodyIndexFrameTime);
                }

                bBuildMesh = true;
            }

            if (bUpdateMesh)
            {
                Graphics.CopyTexture(colorTextureBuildMesh, colorTextureUpdateMesh);
                mesh.vertices = meshVertices;

                if (!showAsPointCloud)
                {
                    mesh.SetIndices(meshIndices, MeshTopology.Triangles, 0);
                }

                mesh.RecalculateBounds();
                //prevUserId = userId;

                bUpdateMesh = false;
                //Debug.Log("Mesh updated.");

                if (updateColliderInterval > 0 && (Time.time - lastColliderUpdateTime) >= updateColliderInterval)
                {
                    lastColliderUpdateTime = Time.time;
                    MeshCollider meshCollider = GetComponent<MeshCollider>();

                    if (meshCollider)
                    {
                        meshCollider.sharedMesh = null;
                        meshCollider.sharedMesh = mesh;
                    }
                }
            }

        }


        // builds the mesh according to the current 
        private void BuildMesh()
        {
            while(!bStopThread)
            {
                if(!bUpdateMesh)
                {
                    if (bMeshInited && sensorData.depthImage != null && lastSpaceCoordsTime != sensorData.lastDepthFrameTime)
                    {
                        if (bBuildMesh)
                        {
                            lastSpaceCoordsTime = sensorData.lastDepthFrameTime;

                            try
                            {
                                int imageWidth1 = imageWidth - 1, imageHeight1 = imageHeight - 1;
                                int userPosDepth = (int)(userBodyPos.z * 1000f);

                                const int maxDistMm = 200; // max distance between vertices in a triangle, in mm
                                const int maxPosDistMm = 1000;

                                if (!showAsPointCloud)
                                {
                                    System.Array.Clear(meshIndices, 0, meshIndices.Length);
                                    System.Array.Clear(meshVertUsed, 0, meshVertUsed.Length);
                                }

                                for (int y = 0, di = 0, ti = 0; y < imageHeight; y++)
                                {
                                    for (int x = 0; x < imageWidth; x++, di++, ti += 6)
                                    {
                                        ushort depth = tDepthImage != null ? tDepthImage[di] : (ushort)0;
                                        byte bodyIndex = tBodyIndexImage != null ? tBodyIndexImage[di] : (byte)0;
                                        bool isValidBodyPixel = userId != 0 && bodyIndex == userBodyIndex && Mathf.Abs(depth - userPosDepth) <= maxPosDistMm;

                                        bool bVertexSet = false;
                                        if (isValidBodyPixel)
                                        {
                                            float fDepth = (float)depth * 0.001f;
                                            Vector3 vVertex = new Vector3(spaceTable[di].x * fDepth * spaceScale.x, spaceTable[di].y * fDepth * spaceScale.y, fDepth);

                                            bool bUsedVertex = !showAsPointCloud && meshVertUsed[di] != 0;
                                            if (bUsedVertex || isValidBodyPixel)
                                            {
                                                meshVertices[di] = vVertex;

                                                if (!showAsPointCloud)
                                                {
                                                    meshVertUsed[di] = 1;

                                                    if (isValidBodyPixel && x < imageWidth1 && y < imageHeight1)
                                                    {
                                                        int tl = di; ushort tld = depth;
                                                        int tr = tl + 1; ushort trd = tDepthImage != null ? tDepthImage[tr] : (ushort)0;
                                                        int bl = tl + imageWidth; ushort bld = tDepthImage != null ? tDepthImage[bl] : (ushort)0;
                                                        int br = bl + 1; ushort brd = tDepthImage != null ? tDepthImage[br] : (ushort)0;

                                                        //float fBrDepth = (float)brd * 0.001f;
                                                        //Vector3 vVertex2 = new Vector3(spaceTable[br].x * fBrDepth * spaceScale.x, spaceTable[di].y * fBrDepth * spaceScale.y, fBrDepth);

                                                        byte trbi = tBodyIndexImage != null ? tBodyIndexImage[tr] : (byte)0;
                                                        byte blbi = tBodyIndexImage != null ? tBodyIndexImage[bl] : (byte)0;
                                                        byte brbi = tBodyIndexImage != null ? tBodyIndexImage[br] : (byte)0;

                                                        // 1st triangle
                                                        bool isValidBodyQuad =
                                                            trbi == userBodyIndex && Mathf.Abs(trd - userPosDepth) <= maxPosDistMm &&
                                                            blbi == userBodyIndex && Mathf.Abs(bld - userPosDepth) <= maxPosDistMm &&
                                                            brbi == userBodyIndex && Mathf.Abs(brd - userPosDepth) <= maxPosDistMm;

                                                        if (isValidBodyQuad &&
                                                            Mathf.Abs(trd - tld) < maxDistMm && Mathf.Abs(bld - tld) < maxDistMm)
                                                        {
                                                            meshIndices[ti] = bl;
                                                            meshIndices[ti + 1] = tr;
                                                            meshIndices[ti + 2] = tl;
                                                            meshVertUsed[tr] = meshVertUsed[bl] = 1;
                                                        }

                                                        // 2nd triangle
                                                        if (isValidBodyQuad &&
                                                            Mathf.Abs(trd - bld) < maxDistMm && Mathf.Abs(brd - bld) < maxDistMm)
                                                        {
                                                            meshIndices[ti + 3] = br;
                                                            meshIndices[ti + 4] = tr;
                                                            meshIndices[ti + 5] = bl;
                                                            meshVertUsed[bl] = meshVertUsed[tr] = meshVertUsed[br] = 1;
                                                        }
                                                    }
                                                }

                                                bVertexSet = true;
                                            }
                                        }

                                        if (!bVertexSet)
                                        {
                                            meshVertices[di] = Vector3.zero;

                                            if (!showAsPointCloud)
                                            {
                                                if (meshVertUsed[di] != 0)
                                                {
                                                    float fDepth = (float)depth * 0.001f;
                                                    Vector3 vVertex = new Vector3(spaceTable[di].x * fDepth * spaceScale.x, spaceTable[di].y * fDepth * spaceScale.y, fDepth);
                                                    meshVertices[di] = vVertex;
                                                }
                                            }
                                        }

                                    }
                                }

                                bBuildMesh = false;
                                bUpdateMesh = true;
                                //Debug.Log("Mesh built.");
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogException(ex);
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(KinectInterop.THREAD_SLEEP_TIME_MS);
                }
            }
        }

    }
}
