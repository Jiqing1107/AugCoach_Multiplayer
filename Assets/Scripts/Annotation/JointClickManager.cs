using com.rfilkov.kinect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointClickManager : MonoBehaviour
{
    public Dictionary<string, int> jointClickInfo = new Dictionary<string, int>(); // Dictionary saving the times the joints are clicked
    private KinectManager kinectManager = null;
    private int jointsCount;

    // Start is called before the first frame update
    void Start()
    {
        kinectManager = KinectManager.Instance;
        jointsCount = kinectManager.GetJointCount();
        for (int i = 0; i < jointsCount; i++)
        {
            string name = ((KinectInterop.JointType)i).ToString(); // Name of the i-th joint
            jointClickInfo.Add(name, 0);
        }
    }

}
