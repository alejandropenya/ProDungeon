using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class GUIManager : MonoBehaviour
    {
        private IEnumerator _generatorCoroutine;
        private bool _generationEnded;
        private bool _doNextStep;

        [SerializeField] private Button nextStepButton;
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_InputField roomNumberInputField;
        
        private void Start()
        {
            StartGeneration();
            nextStepButton.onClick.AddListener(() => { _doNextStep = true; });
        }

        private void StartGeneration()
        {
            if (_generatorCoroutine != null) StopCoroutine(_generatorCoroutine);
            _generatorCoroutine = TestCoroutine();
            StartCoroutine(_generatorCoroutine);
        }
        
        private IEnumerator TestCoroutine()
        {
            // Initialize data
            _generationEnded = false;
            var roomNumber = roomNumberInputField.text.ToInt();
            if (roomNumber == 0) roomNumber = 5;
            var generator = Resources.FindObjectsOfTypeAll<DungeonGenerator>().FirstOrDefault();
           generator.numberOfRooms = roomNumber;
            
            while (!_generationEnded)
            {
                if (_doNextStep)
                {
                    _doNextStep = false;
                    generator.GenerateFloor();
                }

                yield return null;
            }

            yield break;
        }
    }
}