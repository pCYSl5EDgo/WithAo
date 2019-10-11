using AoAndSugi.Result;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class GameToResult : MonoBehaviour
    {
        [Inject]
        private ZenjectSceneLoader sceneLoader;

        [SerializeField]
        public TMP_InputField NumberInputField;

        private ResultPoint point;

        public void OnClick()
        {
            if (!int.TryParse(NumberInputField.text, out var value))
            {
                return;
            }
            point = new ResultPoint(value);
            sceneLoader.LoadScene("Scenes/Result", LoadSceneMode.Single, ExtraBindings, LoadSceneRelationship.None, null);
        }

        private void ExtraBindings(DiContainer diContainer)
        {
            if (diContainer is null) return;
            diContainer.BindInstance(point);
        }
    }
}