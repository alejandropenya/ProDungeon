using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType", order = 0)]
    public class TileTypeScriptable : ScriptableObject
    {
        public Color color;
    }
}