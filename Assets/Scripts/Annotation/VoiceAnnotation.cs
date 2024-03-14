using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class VoiceAnnotation : MonoBehaviour
{
    private string recordingFileName = "recording.wav";
    private string jointClickFileName = "Assets/joint_click_information.txt";
    private string jointName = " ";
    private AudioClip clip;
    private AudioSource source;
    private Annotation annotationScript;
    private GameObject jointSelected;

    private List<AudioClip> clips = new List<AudioClip>();
    private int clipIndex = 0;
    private bool isRecording = false;

    //public MyVideoPlayer player = new MyVideoPlayer(); // the video player we are using to sync the timestamps

    public GameObject jointAnnotationPrefab; // the prefab of the button to be added
    public AudioSource audioSource; // the audio source to play the audio clip
    public GameObject voiceCanvas;
    //private Transform btnVisableAll;
    //private Transform btnInvisableAll;
    //private Transform btnDeleteAll;
    private Transform controlPanel;

    private List<AudioClip> audioClipList; // the list to store the added audio clips
    //private List<GameObject> audioMemoList; // the list to store the added buttons



    private class JointMemo
    {
        AudioClip clip;
        int clipIndex;
        GameObject jointAnnotationCanvas;
    }

    private List<JointMemo> jointMemoList;

    void Start()
    {
        //endRecord.enabled = false;
        audioClipList = new List<AudioClip>(100);
        jointMemoList = new List<JointMemo>();

        //btnVisableAll = voiceCanvas.transform.GetChild(0);
        //btnInvisableAll = voiceCanvas.transform.GetChild(1);
        //btnDeleteAll = voiceCanvas.transform.GetChild(2);
        controlPanel = voiceCanvas.transform.GetChild(3);
        //buttonList = new List<GameObject>();

        //LoadAudioClips(); // load the audio clips from PlayerPrefs

        // populate the scroll view with the audio clips
        /*
        foreach (AudioClip clip in audioClipList)
        {
            AddAudioClipButton(clip);
        }
        */
    }

    public void RecordManager()
    {
        if (!isRecording) //!player.IsPlaying() && !isRecording)
        {
            Debug.Log("Recording Audio");
            clip = Microphone.Start(Microphone.devices[0].ToString(), false, 30, 44100);
            source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.Play();
            isRecording = true;
            //startRecord.enabled = false;
            //endRecord.enabled = true;
        }
        else if (isRecording)
        {
            Debug.Log("Recording Stopped");
            Microphone.End(null);
            audioClipList.Add(clip);
            clipIndex++;
            PlayerPrefs.SetInt("clipIndex", clipIndex);
            SaveClip();

            //Check if there's a joint clicked: if there's a joint clicked, then attach the joint-specific voice memo; otherwise, then attach the general voice memo.
            StreamReader reader = new StreamReader(jointClickFileName);
            jointName = reader.ReadToEnd();
            if (jointName == " ")
            {

            }
            else
            {
                AddJointAudioMemo(jointName);
                jointSelected = GameObject.Find(jointName);
                annotationScript = jointSelected.GetComponent<Annotation>();
            }

            //float timestamp = player.GetTimeStamp();
            PlayerPrefs.SetFloat((clipIndex.ToString() + recordingFileName), 0f);

            Destroy(source);
            isRecording = false;

            annotationScript.status = Annotation.JointStatus.Deselected; //Set the color of the joint to be the original color
            annotationScript.UpdateJointColor(jointName);
            //File.WriteAllText(jointClickFileName, string.Empty); //Clear the file content
            //startRecord.enabled = true;
            //endRecord.enabled = false;
        }

    }
    /*
    public void StartRecording()
    {
        if(!isRecording) //!player.IsPlaying() && !isRecording)
        {
            Debug.Log("Recording Audio");
            clip = Microphone.Start(Microphone.devices[0].ToString(), false, 30, 44100);
            source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.Play();
            isRecording = true;
            //startRecord.enabled = false;
            //endRecord.enabled = true;
        }
    }

    public void StopRecording()
    {
        
        if(isRecording)
        {
            Debug.Log("Recording Stopped");
            Microphone.End(null);
            clips.Add(source.clip);
            clipIndex++;
            PlayerPrefs.SetInt("clipIndex", clipIndex);
            SaveClip();
            
            //float timestamp = player.GetTimeStamp();
            PlayerPrefs.SetFloat((clipIndex.ToString()+filename), 0f);

            Destroy(source);
            isRecording = false;

            //startRecord.enabled = true;
            //endRecord.enabled = false;
        }
    }
    */
    void SaveClip()
    {
        string filepath = Application.dataPath + "/Recordings/" + clipIndex.ToString() + recordingFileName;
        clip.name = clipIndex.ToString() + recordingFileName;
        SavWav.Save(filepath, clip);
        Debug.Log("Recording saved to: " + filepath);
    }

    private void AddJointAudioMemo(string name)
    {
        GameObject audioMemoCanvas = Instantiate(jointAnnotationPrefab);
        audioMemoCanvas.transform.SetParent(controlPanel);
        float newMemoPosZ = -10f;
        float newMemoPosY = 270f - 100 * clipIndex;
        audioMemoCanvas.transform.localPosition = new Vector3(0, newMemoPosY, newMemoPosZ);

        GameObject btnMemo = audioMemoCanvas.transform.GetChild(0).gameObject;
        GameObject btnVisable = audioMemoCanvas.transform.GetChild(1).gameObject;
        GameObject btnInvisable = audioMemoCanvas.transform.GetChild(2).gameObject;
        GameObject btnDelete = audioMemoCanvas.transform.GetChild(3).gameObject;

        TextMeshProUGUI textMeshPro = btnMemo.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = name; 

        btnMemo.GetComponent<Button>().onClick.AddListener(delegate { PlayAudioClip(clip); });
        btnVisable.GetComponent<Button>().onClick.AddListener(delegate { VisableOne(); });
        btnInvisable.GetComponent<Button>().onClick.AddListener(delegate { InvisableOne(); });
    }

    public AudioClip GetRecording(int index)
    {
        if(index >= 0)
        {
            return clips[index];
        } else {
            Debug.LogError("Invalid clip index: "+index);
            return null;
        }
    }

    public int GetNumRecordings()
    {
        return clips.Count;
    }

    public float GetTimestamp(string filename)
    {
        return PlayerPrefs.GetFloat(filename, 0f);
    }


    //PLAYBACK-------------------------------------------------------------------------------
    // Call this method to add a new audio clip and button to the scroll view
    public void AddAudioClip(AudioClip newClip)
    {
        

        //SaveAudioClips(); // save the audio clips to PlayerPrefs
    }

    // Call this method to remove all the audio clips and buttons from the scroll view
    /*
    public void ClearAudioClips()
    {
        foreach (GameObject memo in )
        {
            Destroy(memo);
        }
        audioClipList.Clear();
        //buttonList.Clear();

        SaveAudioClips(); // save the audio clips to PlayerPrefs
    }
    */
    // This method is called when the corresponding button is clicked, and plays the corresponding audio clip
    private void PlayAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void VisableOne()
    {
        Debug.Log("Visable button was clicked!");
        if (annotationScript.status != Annotation.JointStatus.Visable_one)
        {
            annotationScript.status = Annotation.JointStatus.Visable_one;
            annotationScript.UpdateJointColor(jointName);
        }
    }

    private void InvisableOne()
    {
        Debug.Log("Invisable button was clicked!");
        if (annotationScript.status == Annotation.JointStatus.Visable_one)
        {
            annotationScript.status = Annotation.JointStatus.Deselected;
            annotationScript.UpdateJointColor(jointName);
        }
    }

    // This method creates a new button for the given audio clip, and adds it to the scroll view
    /*
    private void AddAudioClipButton(AudioClip clip)
    {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.transform.SetParent(contentPanel);
        float newButtonPosY = 100f - 30 * clipIndex;
        newButton.transform.localPosition = new Vector3(0,newButtonPosY, 0);
        newButton.transform.localRotation = new Quaternion(0,0,0,0);
        newButton.transform.localScale = new Vector3(1,1,1);

        newButton.GetComponentInChildren<TextMeshProUGUI>().text = clip.name; // set the button text to the name of the new audio clip
        newButton.GetComponent<Button>().onClick.AddListener(delegate { PlayAudioClip(clip); }); // add a listener to the button to play the corresponding audio clip
        buttonList.Add(newButton);
    }
    */
    // This method saves the audio clips to PlayerPrefs as a JSON string
    private void SaveAudioClips()
    {
        string audioClipJson = JsonUtility.ToJson(audioClipList.ToArray());
        PlayerPrefs.SetString("audioClipList", audioClipJson);
    }

    // This method loads the audio clips from PlayerPrefs and adds them to the audioClipList
    private void LoadAudioClips()
    {
        if (PlayerPrefs.HasKey("audioClipList"))
        {
            string audioClipJson = PlayerPrefs.GetString("audioClipList");
            AudioClip[] clips = JsonUtility.FromJson<AudioClip[]>(audioClipJson);
            audioClipList.AddRange(clips);
        }
    }

}