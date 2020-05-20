using TMPro;
using UnityEngine;

public class CustomInputField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TMP_InputField inputField;

    public void Init(string variableName, string defaultValue = "")
    {
        labelText.text = variableName;
        inputField.text = defaultValue;
    }

    public void SetValue(string value)
    {
        inputField.text = value;
    }
    
    public string GetCurrentValue()
    {
        return inputField.text;
    }
        
}