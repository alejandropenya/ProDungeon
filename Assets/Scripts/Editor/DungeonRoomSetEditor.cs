using DungeonGenerator;
using Extensions;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CustomEditor(typeof(DungeonRoomSet))]
    public class DungeonRoomSetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Add rooms to RoomSet from this folder"))
            {
                var path = AssetDatabase.GetAssetPath(target).Replace(target.name + ".asset", "");
                
                var roomsToAdd = AssetDatabaseUtils.LoadAllAssetsContainingName<RoomScriptable>("", nameof(RoomScriptable))
                    .Where(x => AssetDatabase.GetAssetPath(x).StartsWith(path)).ToList();
                
                ((DungeonRoomSet)target).SetFieldValue("roomSet", roomsToAdd);
            }
        }
    }
}