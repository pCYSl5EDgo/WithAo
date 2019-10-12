using UnityEngine;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class TitleToGame : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

        [Inject] private ConfigPanel configPanel;

        public void OnClickToGame() => sceneLoader.LoadScene("Game");

        public void OnClickSettings() => configPanel.gameObject.SetActive(true);
    }
}