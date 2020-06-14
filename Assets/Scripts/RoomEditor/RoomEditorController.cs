using System.Collections.Generic;
using Extensions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace RoomEditor
{
    public class RoomEditorController : MonoBehaviour
    {
#if UNITY_EDITOR
        private Room _currentRoom;
        private List<Image> _currentObjects;
        private RoomScriptable _roomScriptableObject;
        private string _assetsPath;
        private List<RoomScriptable> _roomsToAdd;
        [SerializeField] private TileTypeScriptable selectedTileType;

        [SerializeField] private int cols;
        [SerializeField] private int rows;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private CustomInputField roomName;
        [SerializeField] private CustomInputField colsField;
        [SerializeField] private CustomInputField rowsField;
        [SerializeField] private Button createNewRoomButton;
        [SerializeField] private Button saveRoomButton;
        [SerializeField] private Transform brushesContainer;
        [SerializeField] private TMP_Dropdown loadRoomDropdown;

        private void Start()
        {
            _currentRoom = ScriptableObject.CreateInstance<Room>();
            createNewRoomButton.onClick.AddListener(CreateNewRoom);
            saveRoomButton.onClick.AddListener(SaveRoom);
            saveRoomButton.interactable = false;
            _assetsPath = "Assets/ScriptableObjects/Rooms";
            loadRoomDropdown.onValueChanged.AddListener(LoadRoomFromList);
            _roomsToAdd = new List<RoomScriptable>();
            RefreshRoomList();
            var roomNames = _roomsToAdd.Select(room => room.name).ToList();
            PopulateDropdown(roomNames);
            InstantiateBrushes();
        }

        private void InstantiateBrushes()
        {
            var brushes = AssetDatabaseUtils.LoadAllAssetsContainingName<TileTypeScriptable>("", "TileTypeScriptable")
                .ToList();
            brushes.ForEach(scriptable =>
            {
                var brush = new GameObject(scriptable.name);
                var brushImage = brush.AddComponent<Image>();
                brushImage.color = scriptable.color;
                var clickListener = brush.AddComponent<ClickListener>();
                clickListener.onClicked = new UnityEvent();
                clickListener.onClicked.AddListener(() => ChangeBrush(scriptable));
                brushImage.transform.SetParent(brushesContainer);
            });
        }

        private void ChangeBrush(TileTypeScriptable tileType)
        {
            selectedTileType = tileType;
        }

        private void RefreshRoomList()
        {
            _roomsToAdd = AssetDatabaseUtils.LoadAllAssetsContainingName<RoomScriptable>("", "RoomScriptable").ToList();
        }

        private void PopulateDropdown(List<string> roomNames)
        {
            loadRoomDropdown.ClearOptions();
            loadRoomDropdown.AddOptions(roomNames);
        }

        private void LoadRoomFromList(int index)
        {
            _currentRoom = _roomsToAdd[index].ToRoom();
            cols = _currentRoom.cols;
            RenderTest();
        }

        private void CreateNewRoom()
        {
            _roomScriptableObject = null;
            _roomScriptableObject = ScriptableObject.CreateInstance<RoomScriptable>();
            gridLayout.transform.GetAllChildren().ForEach(Destroy);
            cols = colsField.GetCurrentValue().ToInt();
            cols = Mathf.Clamp(cols, 0, 20);
            rows = rowsField.GetCurrentValue().ToInt();
            rows = Mathf.Clamp(rows, 0, 20);
            colsField.SetValue(cols.ToString());
            rowsField.SetValue(rows.ToString());
            var roomMatrix = new Matrix<TileTypeScriptable>(cols, rows);
            _currentRoom = ScriptableObject.CreateInstance<Room>();
            roomMatrix.FillMatrix((col, row) =>
                AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Empty", "TileTypeScriptable"));
            _currentRoom.roomMatrix = roomMatrix;
            _currentRoom.cols = cols;
            _currentRoom.rows = rows;
            saveRoomButton.interactable = true;
            RenderTest();
        }

        private void OnTileClicked(GameObject tile, int col, int row)
        {
            _currentRoom.roomMatrix.SetValue(selectedTileType, col, row);
            tile.GetComponent<Image>().color = selectedTileType.color;
        }

        private void SaveRoom()
        {
            if (!_roomScriptableObject)
            {
                Debug.Log("No info to save");
                return;
            }

            var roomTextName = _assetsPath;

            if (roomName.GetCurrentValue() != "")
            {
                roomTextName += "/" + roomName.GetCurrentValue() + ".asset";
            }
            else
            {
                var roomNumber = AssetDatabaseUtils.LoadAllAssetsContainingName<RoomScriptable>("", "RoomScriptable")
                    .Count() + 1;
                roomTextName += "/" + "CustomRoom" + roomNumber + ".asset";
            }

            _roomScriptableObject.cols = _currentRoom.cols;
            _roomScriptableObject.rows = _currentRoom.rows;
            _roomScriptableObject.roomList = _currentRoom.roomMatrix.ToList();

            if (AssetDatabaseUtils.LoadAssetWithName<RoomScriptable>(_roomScriptableObject.name, "RoomScriptable"))
            {
                AssetDatabaseUtils.SetDirty(_roomScriptableObject);
                AssetDatabaseUtils.SaveAssets();
                if (_roomScriptableObject.name != roomName.GetCurrentValue())
                {
                    AssetDatabaseUtils.FindAssetWithExactName(_roomScriptableObject.name, out var foundPath);
                    _roomScriptableObject.name = roomName.GetCurrentValue();
                    AssetDatabase.CopyAsset(foundPath, roomTextName);
                    AssetDatabase.DeleteAsset(foundPath);
                    AssetDatabaseUtils.SaveAssets();
                    _roomScriptableObject =
                        AssetDatabaseUtils.LoadAssetWithName<RoomScriptable>(roomName.GetCurrentValue(),
                            "RoomScriptable");
                }
            }
            else
            {
                _roomScriptableObject.name = roomName.GetCurrentValue();
                AssetDatabase.CreateAsset(_roomScriptableObject, roomTextName);
            }

            AssetDatabase.SaveAssets();
            RefreshRoomList();
            PopulateDropdown(_roomsToAdd.Select(room => room.name).ToList());
        }

        private void RenderTest()
        {
            gridLayout.transform.DetachChildren();
            gridLayout.constraintCount = cols;
            _currentRoom.roomMatrix.ForEach((col, row, item) =>
            {
                var tile = new GameObject("Tile");
                var tileImage = tile.AddComponent<Image>();
                tileImage.color = item.color;
                var clickListener = tile.AddComponent<ClickListener>();
                clickListener.onClicked = new UnityEvent();
                clickListener.onClicked.AddListener(() => OnTileClicked(tile, col, row));

                var tileImageTransform = tileImage.transform;
                tileImageTransform.SetParent(transform);
                tileImageTransform.localScale = new Vector3(1f, 1f, 0f);
            });
            var max = Mathf.Max(cols, rows);
            if (max > 7)
            {
                var size = ((1 - (max - 7) / 13f) * 40) + 30;
                gridLayout.cellSize = new Vector2(size, size);
            }
            else
            {
                gridLayout.cellSize = new Vector2(100, 100);
            }
        }
#endif
    }
}