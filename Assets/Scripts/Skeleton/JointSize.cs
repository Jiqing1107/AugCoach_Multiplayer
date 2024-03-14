using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointSize : MonoBehaviour
{
    Vector3 scale = new Vector3();
    Vector3 scale_ = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            print("Does not has a parent object.");
        }
        else
        {
            Transform parentTrans = transform.parent;
            Vector3 scale = parentTrans.localScale;
            scale_ = new Vector3(0.1f * scale.x, 0.1f * scale.y, 0.1f * scale.z);
            transform.localScale = scale_;
        }


    }
}
