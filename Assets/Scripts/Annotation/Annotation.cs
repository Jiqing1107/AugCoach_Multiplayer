using com.rfilkov.kinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Annotation : MonoBehaviour
{
    public GameObject joint;
    private GameObject skeleton;
    private JointClickManager jointClickManagerScript;
    Transform jointSelection;
    //public GameObject inputField;
    //public List<GameObject> inputs = new List<GameObject>();
    private string jointClickFileName;
    private Renderer jointRenderer;

    private Color originalColor = Color.blue;
    private Color selectedColor = Color.yellow;
    private Color snowman = new Color(251, 251, 251);
    private Color lightgreen = new Color(200, 207, 35);
    private Color orange = new Color(246, 152, 51);
    private Color peach = new Color(238, 103, 35);
    private Color teflon = new Color(85, 77, 86);

    public enum JointStatus
    {
        Selected,
        Deselected,
        Visable_one,
    }

    public JointStatus status = JointStatus.Deselected;

    // Start is called before the first frame update    
    void Start()
    {
        //Initiate an empy joint information and save it in the file
        //CheckJointClick obj = new CheckJointClick();

        //obj.isClicked = false;
        //obj.jointName = "";
        skeleton = GameObject.Find("Skeleton");
        jointClickManagerScript = skeleton.GetComponent<JointClickManager>();
        jointClickFileName = "Assets/joint_click_information.txt";
        File.Create(jointClickFileName).Dispose();

        //string output = JsonUtility.ToJson(obj);
        ///File.AppendAllText(filename, output + "\n");


        jointRenderer = joint.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void AddAnnotation()
    {
        Debug.Log("A joint is selected!");
        jointSelection = joint.transform;
        GameObject box = new GameObject();
        box = Instantiate(inputField, Vector3.zero, Quaternion.Euler(new Vector3(0, 0, 0))); //tranform.rotation to camera
                                                                                             //posit = Camera.main.WorldToScreenPoint(jointSelection.transform.position);
        box.transform.position = new Vector3(jointSelection.transform.position.x + 0.1f, jointSelection.transform.position.y, jointSelection.transform.position.z);
        //box.transform.parent = GameObject.FindGameObjectWithTag("Selectable").transform;
        box.transform.parent = jointSelection.transform;
        //textBoxPrefab.transform.SetParent(jointSelection, false);
        box.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        inputs.Add(box);
    }
    */

    public void onJointClicked()
    {
        string jointName = joint.transform.name;
        status = JointStatus.Selected;
        UpdateJointColor(jointName);

        FileInfo fileInfo = new FileInfo(jointClickFileName);
        
        if (fileInfo.Length != 0)
        {
            Debug.Log("The file was emptyed");
            File.WriteAllText(jointClickFileName, string.Empty);
        }

        File.AppendAllText(jointClickFileName, jointName);

        if (jointClickManagerScript.jointClickInfo.ContainsKey(jointName))
        {
            jointClickManagerScript.jointClickInfo[jointName] += 1;
        }

    }

    public void UpdateJointColor(string name)
    {
        switch (status)
        {
            case JointStatus.Deselected:
                jointRenderer.material.color = originalColor;
                break;
            case JointStatus.Selected:
                jointRenderer.material.color = selectedColor;
                break;
            case JointStatus.Visable_one:
                VisableOne(name);
                //jointRenderer.material.color = selectedColor;
                break;
        }
    }

    private void VisableOne(string name)
    {
        int i = jointClickManagerScript.jointClickInfo[name];
        //GameObject joint_ = GameObject.Find(name);
        //Renderer joint_Renderer = joint_.GetComponent<Renderer>();
        switch (i)
        {
            case 0:
                Debug.Log("Color changed to original");
                jointRenderer.material.color = originalColor;
                break;
            case 1:
                Debug.Log("Color changed to snowman");
                jointRenderer.material.color = snowman;
                break;
            case 2:
                Debug.Log("Color changed to lightgreen");
                jointRenderer.material.color = lightgreen;
                break;
            case 3:
                jointRenderer.material.color = orange;
                break;
            case 4:
                jointRenderer.material.color = peach;
                break;
            case 5:
                jointRenderer.material.color = teflon;
                break;
        }
    }

    private bool IsFileEmpty(string filename)
    {
        throw new NotImplementedException();
    }
}
