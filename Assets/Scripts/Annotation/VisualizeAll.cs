using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAll : MonoBehaviour
{
    public GameObject skeleton;
    private JointClickManager jointClickManagerScript;
    private Color originalColor = Color.blue;
    private Color selectedColor = Color.yellow;
    private Color snowman = new Color(251, 251, 251);
    private Color lightgreen = new Color(200, 207, 35);
    private Color orange = new Color(246, 152, 51);
    private Color peach = new Color(238, 103, 35);
    private Color teflon = new Color(85, 77, 86);

    // Start is called before the first frame update
    void Start()
    {
        jointClickManagerScript = skeleton.GetComponent<JointClickManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnVisualizeAllClicked()
    {
        foreach (var kvp in jointClickManagerScript.jointClickInfo)
        {
            string name = kvp.Key;
            int value = kvp.Value;

            GameObject joint_ = GameObject.Find(name);
            Renderer joint_Renderer = joint_.GetComponent<Renderer>();

            switch (value)
            {
                case 0:
                    joint_Renderer.material.color = originalColor;
                    break;
                case 1:
                    joint_Renderer.material.color = snowman;
                    break;
                case 2:
                    joint_Renderer.material.color = lightgreen;
                    break;
                case 3:
                    joint_Renderer.material.color = orange;
                    break;
                case 4:
                    joint_Renderer.material.color = peach;
                    break;
                case 5:
                    joint_Renderer.material.color = teflon;
                    break;
            }
        }
    }

    public void OnInvisableAllClicked()
    {
        foreach (var kvp in jointClickManagerScript.jointClickInfo)
        {
            string name = kvp.Key;

            GameObject joint_ = GameObject.Find(name);
            Renderer joint_Renderer = joint_.GetComponent<Renderer>();

            joint_Renderer.material.color = originalColor;
        }
    }
}
