using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AnnotationBoxListener : MonoBehaviour
{
    public GameObject virtualKeyboard;

    private GameObject content;
    private GameObject input;
    private GameObject keys;
    private GameObject row4;
    private GameObject enter;

    private Text txInput;
    private Button btnEnter;

    private void Start()
    {
        content = virtualKeyboard.transform.GetChild(1).gameObject;
        input = content.transform.GetChild(0).gameObject;
        keys = content.transform.GetChild(1).gameObject;
        row4 = keys.transform.GetChild(4).gameObject;
        enter = row4.transform.GetChild(3).gameObject;

        txInput = input.GetComponent<Text>();
        btnEnter = enter.GetComponent<Button>();
    }

    void Update()
    {
        // Check if an input field is currently selected
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject != null)
        {
            TMP_InputField selectedInputField = selectedGameObject.GetComponent<TMP_InputField>();
            if (selectedInputField != null)
            {
                Debug.Log("Clicked a TMP input field!");
                virtualKeyboard.SetActive(true);
                btnEnter.onClick.AddListener(() => OnEnterButtonClick(selectedInputField));
            }
        }
    }

    public void OnEnterButtonClick(TMP_InputField field)
    {
        string annotationText = txInput.text;
        field.text = annotationText;
    }
}
