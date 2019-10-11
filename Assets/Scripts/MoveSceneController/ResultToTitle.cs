using TMPro;
using UnityEngine;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class ResultToTitle : MonoBehaviour
    {
        [SerializeField] public TMP_Text PointText;

        [Inject] private Result.ResultPoint point;
        
        [Inject] private ZenjectSceneLoader sceneLoader;

        void Awake()
        {
            PointText.text = point.ToString();
        }

        public void OnClick()
        {
            sceneLoader.LoadScene("Title");
        }
    }
}