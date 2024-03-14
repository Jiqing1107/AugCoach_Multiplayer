using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointTrajectory : MonoBehaviour
{
    public GameObject joint;
    public Material materialHighlighted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateJointTrajectory()
    {
        string jointName = joint.name;
        for (int i = 0; i < 5; i++)
        {
            string parentName = "SK" + i.ToString();
            Debug.Log("Addressing:" + parentName);
            GameObject parentObject = GameObject.Find(parentName);
            Transform childObject = parentObject.transform.Find(jointName);

            if (childObject != null)
            {
                MeshRenderer meshRenderer = childObject.GetComponent<MeshRenderer>();
                meshRenderer.material = materialHighlighted;
            }
        }
    }
}
