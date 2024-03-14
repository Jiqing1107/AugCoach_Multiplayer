using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointRenderer : MonoBehaviour
{
    public GameObject receiver;
    private JointReceiver jointReceiverScript;
    private JointReceiver.Joint[] jointArray;

    [Tooltip("Game object used to overlay the joints.")]
    public GameObject jointPrefab;

    [Tooltip("Line object used to overlay the bones.")]
    public LineRenderer linePrefab;

    private int jointsCount_;
    private GameObject[] joints_ = null;
    private LineRenderer[] lines_ = null;

    public class JointInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    Dictionary<string, string> jointRelation = new Dictionary<string, string>();
    Dictionary<string, JointInfo> jointInformation = new Dictionary<string, JointInfo>();

    // Start is called before the first frame update
    void Start()
    {
        jointReceiverScript = receiver.GetComponent<JointReceiver>();
        jointArray = jointReceiverScript.JointArray;

        jointsCount_ = 25;
        if (jointPrefab)
        {
            // array holding the skeleton joints
            joints_ = new GameObject[jointsCount_];

            for (int i = 0; i < joints_.Length; i++)
            {
                joints_[i] = Instantiate(jointPrefab) as GameObject;
                joints_[i].transform.parent = transform;
                joints_[i].name = jointReceiverScript._jointNames[i];
                joints_[i].SetActive(false);
            }

        }
        lines_ = new LineRenderer[jointsCount_];
        for (int i = 0; i < joints_.Length; i++)
        {
            lines_[i] = Instantiate(linePrefab);
            lines_[i].gameObject.SetActive(false);
        }
        SaveJointRelation();
    }

    // Update is called once per frame
    void Update()
    {
        //jointReceiverScript.PrintJointArray();
        SaveJointInfo();
        readJointInformation();
    }

    public void readJointInformation()
    {
        for (int i = 0; i < jointArray.Length; i++)
        {
            joints_[i].SetActive(true);
            joints_[i].transform.position = jointArray[i].Position;
            joints_[i].transform.rotation = jointArray[i].Rotation;
            Debug.Log("Printing" + jointArray[i].Name + "at position:" + jointArray[i].Position);

            string parent_name = jointRelation[jointArray[i].Name];

            Vector3 parent_position = jointInformation[parent_name].position;
            lines_[i].gameObject.SetActive(true);
            lines_[i].SetPosition(0, parent_position);
            lines_[i].SetPosition(1, jointArray[i].Position);
        }
    }

    private void SaveJointRelation()
    {
        jointRelation.Add("pelvis", "pelvis");
        jointRelation.Add("torso_a", "pelvis");
        jointRelation.Add("torso_b", "torso_a");
        jointRelation.Add("neck", "torso_b");
        jointRelation.Add("head", "neck");
        jointRelation.Add("clavicle_lt", "torso_b");
        jointRelation.Add("shoulder_lt", "clavicle_lt");
        jointRelation.Add("elbow_lt", "shoulder_lt");
        jointRelation.Add("hand_lt", "elbow_lt");
        jointRelation.Add("clavicle_rt", "torso_b");
        jointRelation.Add("shoulder_rt", "clavicle_rt");
        jointRelation.Add("elbow_rt", "shoulder_rt");
        jointRelation.Add("hand_rt", "elbow_rt");
        jointRelation.Add("hip_lt", "pelvis");
        jointRelation.Add("knee_lt", "hip_lt");
        jointRelation.Add("foot_lt", "knee_lt");
        jointRelation.Add("toe_lt", "foot_lt");
        jointRelation.Add("hip_rt", "pelvis");
        jointRelation.Add("knee_rt", "hip_rt");
        jointRelation.Add("foot_rt", "knee_rt");
        jointRelation.Add("toe_rt", "foot_rt");
        jointRelation.Add("index_alt", "hand_lt");
        jointRelation.Add("thumb_alt", "hand_lt");
        jointRelation.Add("index_art", "hand_rt");
        jointRelation.Add("thumb_art", "hand_rt");
    }

    public void SaveJointInfo()
    {
        for (int i = 0; i < jointsCount_; i++)
        {
            JointInfo jointInfo = new JointInfo();
            jointInfo.position = jointArray[i].Position;
            jointInfo.rotation = jointArray[i].Rotation;

            if (jointInformation.TryGetValue(jointArray[i].Name, out var value))
            {
                jointInformation[jointArray[i].Name] = jointInfo;
            }
            else
            {
                jointInformation.Add(jointArray[i].Name, jointInfo);
            }
        }
    }
}
