using UnityEngine;

namespace AoAndSugi.Game.IO
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "AoAndSugi/ColorSettings")]
#endif
    public sealed class ColorSettings : ScriptableObject
    {
        public Color[] BaseColors;
    }
}
