using TMPro;
using UnityEngine;

namespace Utils
{
    public class CustomInputField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TMP_InputField inputField;

        public void Init(string variableName, string defaultValue = "")
        {
            labelText.text = variableName;
            inputField.text = defaultValue;
        }

        public string GetCurrentValue()
        {
            return inputField.text;
        }
        
    }
}