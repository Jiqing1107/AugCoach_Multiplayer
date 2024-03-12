using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadCalibration : MonoBehaviour
{
    string path;
    string jsonString;

    [System.Serializable]
    public class CamInfo
    {
        public int sensorType;
        public int sensorIndex;
        public string sensorId;
        public Vector3 position;
        public Vector3 rotation;
    }

    public class CalibrationData
    {
        public int version;
        public string[] settings;
        public List<CamInfo> camPose;
        public ulong estimatedAtTime;
        public string estimatedDateTime;
    }
   
    List<CamInfo> campose = new List<CamInfo>();
    CalibrationData calibrationData = new CalibrationData();
    CamInfo caminfo = new CamInfo();
    float worldScale;
    Vector3 offsetPosition; //This is basiclly the position of the parent object

    void Start()
    {
        path = "multicam_config_Yasser.json";
        jsonString = File.ReadAllText(path);
        calibrationData = JsonUtility.FromJson<CalibrationData>(jsonString);
        campose = calibrationData.camPose;
        worldScale = transform.lossyScale[0];

        /*
        offsetPosition = GameObject.Find("PointCloudParent").transform.position;
        for (int i = 0; i < 3; i++)
        {
            string name = "SceneMeshS" + i.ToString();
            try
            {
                GameObject pointCloud = GameObject.Find(name);
                pointCloud.transform.position = campose[i].position * worldScale;
                pointCloud.transform.rotation = Quaternion.Euler(campose[i].rotation);
                Debug.Log("Adjusted scene mesh: " + name);
            }
            catch
            {
                Debug.Log("Cannot find specified scene mesh: " + name);
            }
        }
        */
        int i = 0;
        foreach (Transform child in transform)
        {
            // Access child GameObjects or do something with them
            GameObject childObject = child.gameObject;
            childObject.transform.localPosition = campose[i].position;
            childObject.transform.rotation = Quaternion.Euler(campose[i].rotation);
            i++;
        }
    }

    private void Update()
    {



        /*
        float worldScale = transform.lossyScale[0];
        for (int i = 0; i < 3; i++)
        {
            string name = "SceneMeshS" + i.ToString();
            try
            {
                GameObject pointCloud = GameObject.Find(name);
                pointCloud.transform.position += campose[i].position*worldScale;
                pointCloud.transform.rotation *= campose[i].rotation;
                Debug.Log("Adjusted scene mesh: " + name);
            }
            catch
            {
                Debug.Log("Cannot find specified scene mesh: " + name);
            }
        }
        */
    }


}
