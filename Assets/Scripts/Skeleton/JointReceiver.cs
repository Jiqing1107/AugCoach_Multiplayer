using System;
using UnityEngine;

public class JointReceiver : MonoBehaviour
{
    /// <summary>
    /// Data structure used to access joint data
    /// </summary>
    public struct Joint
    {
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        
        public Joint(string name, Vector3 position, Quaternion rotation)
        {
            Name = name;
            Position = position;
            Rotation = rotation;
        }
    }

    [TextArea] public string description =
        "This component receives and parses the joint data from the server. Access the joint data via the JointArray property.";
    
    private Joint[] _jointArray = new Joint[25];
    public readonly string[] _jointNames = {
        "pelvis",
        "torso_a",
        "torso_b",
        "neck",
        "head",
        "clavicle_lt",
        "shoulder_lt",
        "elbow_lt",
        "hand_lt",
        "clavicle_rt",
        "shoulder_rt",
        "elbow_rt",
        "hand_rt",
        "hip_lt",
        "knee_lt",
        "foot_lt",
        "toe_lt",
        "hip_rt",
        "knee_rt",
        "foot_rt",
        "toe_rt",
        "index_alt",
        "thumb_alt",
        "index_art",
        "thumb_art",
    };
    
    /// <summary>
    /// Getter for the joint array
    /// </summary>
    public Joint[] JointArray => _jointArray;

    private void Start()
    {
        description =
            "This component receives and parses the joint data from the server. Access the joint data via the JointArray property.";
        
        float[] jointData = new float[] {
            2f, 1.065582f, 3.9875f, -0.00414706f, 0.7070947f, -0.00414706f, 0.7070946f,
            2f, 1.075582f, 3.9875f, -0.02955926f, 0.7064888f, -0.02955926f, 0.7064887f,
            2f, 1.182548f, 3.978534f, 0.01657646f, 0.7069126f, 0.01657646f, 0.7069125f,
            2f, 1.458913f, 4.022044f, -0.06072733f, 0.7044989f, -0.06067505f, 0.7044943f,
            1.999992f, 1.568243f, 4.003063f, 0.05985356f, 0.7045736f, 0.05980286f, 0.7045691f,
            2.036533f, 1.406251f, 4.014633f, 0.4913681f, 0.5085453f, 0.5065146f, -0.4933381f,
            2.13002f, 1.406625f, 4.017471f, 0.5070958f, 0.4927827f, 0.5077509f, -0.4921468f,
            2.356915f, 1.399835f, 4.017764f, 0.508998f, 0.4937533f, 0.5058439f, -0.4911731f,
            2.58995f, 1.392861f, 4.017651f, 0.508998f, 0.4937533f, 0.5058439f, -0.4911731f,
            1.963467f, 1.406251f, 4.014633f, 0.4933384f, 0.5065143f, -0.5085452f, 0.4913681f,
            1.86998f, 1.406625f, 4.017471f, 0.4921468f, 0.5077509f, -0.4927827f, 0.5070958f,
            1.643085f, 1.399835f, 4.017764f, 0.4911731f, 0.5058439f, -0.4937534f, 0.508998f,
            1.41005f, 1.392861f, 4.017651f, 0.4911731f, 0.5058439f, -0.4937534f, 0.508998f,
            2.068352f, 0.9805003f, 3.999876f, -0.7069545f, -0.01479393f, -0.706952f, -00.01468101f,
            2.068422f, 0.5422801f, 4.018155f, -0.7065966f, -0.02587321f, -0.7066371f, -0.02676298f,
            2.067912f, 0.1389471f, 4.048241f, -0.7070867f, -2.756109E-07f, -0.7071272f, 2.756715E-07f,
            2.068412f, 0.05189383f, 3.917528f, -0.5863804f, 0.4084636f, -0.5771996f, 0.3951667f,
            1.931649f, 0.9805003f, 3.999876f, -0.01468101f, 0.7069522f, -0.01479391f, 0.7069544f,
            1.931579f, 0.5422798f, 4.018155f, -0.02676312f, 0.7066334f, -0.02587307f, 0.7066002f,
            1.932088f, 0.1389468f, 4.048241f, -4.247305E-09f, 0.7071235f, -4.656611E-10f, 0.7070902f,
            1.93159f, 0.05189359f, 3.917528f, 0.3951645f, 0.5771965f, 0.4084659f, 0.5863832f,
            2.702607f, 1.378183f, 3.980522f, -0.02613744f, 0.002359475f, 0.8881633f, -0.4587786f,
            2.61974f, 1.389091f, 3.995682f, 0.542421f, -0.1495802f, 0.5577272f, -0.6102016f,
            1.297393f, 1.378183f, 3.980522f, 0.4587786f, 0.8881633f, -0.002359563f, -0.02613742f,
            1.380259f, 1.389091f, 3.995682f, 0.6102015f, 0.5577273f, 0.14958f, 0.542421f
        };
        
        for (int i = 0; i < 25; i++)
        {
            _jointArray[i].Name = _jointNames[i];
            _jointArray[i].Position = new Vector3(jointData[i * 7], jointData[i * 7 + 1], jointData[i * 7 + 2]);
            _jointArray[i].Rotation = new Quaternion(jointData[i * 7 + 3], jointData[i * 7 + 4], jointData[i * 7 + 5], jointData[i * 7 + 6]);
        }
    }


    public void InitJointArray()
    {
        // _jointArray = new Joint[_jointNames.Length];
        for (var i = 0; i < _jointNames.Length; i++)
        {
            _jointArray[i] = new Joint();
            // _jointArray[i].Name = _jointNames[i];
            // _jointArray[i].Position = Vector3.zero;
            // _jointArray[i].Rotation = Quaternion.identity;
        }
    }
    
    /// <summary>
    /// Converts a byte array to a joint array
    /// </summary>
    /// <param name="bytes"></param>
    public void ConvertBytesToJointArray(byte[] bytes)
    {
        // InitJointArray();
        // var jointCount = bytes.Length / 28;
        for (var i = 0; i < 25; i++)
        {
            var joint = new Joint();
            joint.Name = _jointNames[i];
            joint.Position = new Vector3(
                BitConverter.ToSingle(bytes, i * 28),
                BitConverter.ToSingle(bytes, i * 28 + 4),
                BitConverter.ToSingle(bytes, i * 28 + 8)
            );
            joint.Rotation = new Quaternion(
                BitConverter.ToSingle(bytes, i * 28 + 12),
                BitConverter.ToSingle(bytes, i * 28 + 16),
                BitConverter.ToSingle(bytes, i * 28 + 20),
                BitConverter.ToSingle(bytes, i * 28 + 24)
            );
            _jointArray[i] = joint;
        }
        
        PrintJointArray();
    }
    
    /// <summary>
    /// Prints the joint array to the console
    /// </summary>
    public void PrintJointArray()
    {
        string jointString = "Received Joint Data: \n";
        foreach (var joint in _jointArray)
        {
            jointString += joint.Name + " " + joint.Position + " " + joint.Rotation + "\n";
        }
        Debug.Log(jointString);
    }
    
    
}
