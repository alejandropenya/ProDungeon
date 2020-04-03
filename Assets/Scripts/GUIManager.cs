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
        private bool _endGeneration;

        [SerializeField] private Button nextStepButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button endGenerationButton;
        [SerializeField] private TMP_InputField roomNumberInputField;
        [SerializeField] private DungeonGenerator dungeonGenerator;

        private void Start()
        {
            startButton.onClick.AddListener(StartGeneration);
            nextStepButton.onClick.AddListener(() => { _doNextStep = true; });
            endGenerationButton.onClick.AddListener(() => { _endGeneration = true; });
            nextStepButton.interactable = false;
            endGenerationButton.interactable = false;
        }

        private void StartGeneration()
        {
            if (_generatorCoroutine != null) StopCoroutine(_generatorCoroutine);
            _generatorCoroutine = TestCoroutine();
            StartCoroutine(_generatorCoroutine);
            nextStepButton.interactable = true;
            endGenerationButton.interactable = true;
            dungeonGenerator.InitializeFloor();
        }

        private void StopGeneration()
        {
            _generatorCoroutine = null;
            nextStepButton.interactable = false;
            endGenerationButton.interactable = false;
        }

        private IEnumerator TestCoroutine()
        {
            // Initialize data
            _generationEnded = false;
            var roomNumber = roomNumberInputField.text.ToInt();
            if (roomNumber == 0)
            {
                roomNumberInputField.text = "5";
                roomNumber = 5;
                dungeonGenerator.maxNumberOfRooms = roomNumber;
            }

            dungeonGenerator.maxNumberOfRooms = roomNumber;

            while (!_generationEnded)
            {
                if (_doNextStep || _endGeneration)
                {
                    _doNextStep = false;
                    dungeonGenerator.NextStep();
                    if (dungeonGenerator.HasGenerationEnded())
                    {
                        nextStepButton.interactable = false;
                        endGenerationButton.interactable = false;
                        _generationEnded = true;
                        _endGeneration = false;
                        dungeonGenerator.PaintTest();
                        StopGeneration();
                        yield break;
                    }
                    if(!_endGeneration) dungeonGenerator.PaintTest();
                }
                yield return null;
            }
            yield break;
        }
    }
}