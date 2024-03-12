using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInterval : MonoBehaviour
{
    public float interval = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnIntervalUpdate(int index)
    {
        switch(index)
        {
            case 0: interval = 0.3f; print("Interval is changed to: " + interval); break;
            case 1: interval = 0.5f; print("Interval is changed to: " + interval); break;
            case 2: interval = 1.0f; print("Interval is changed to: " + interval); break;
            case 3: interval = 1.5f; print("Interval is changed to: " + interval); break;
            case 4: interval = 2.5f; print("Interval is changed to: " + interval); break;
        }
    }
}
