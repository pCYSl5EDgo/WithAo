using UnityEngine;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class TitleToGame : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

        [Inject] private ConfigPanel configPanel;

        [SerializeField] MatchingPanel matchingPanel;
        //[Inject] private MatchingPanel matchingPanel;

        public void OnClickToGame() => sceneLoader.LoadScene("Game");

        public void OnClickSettings() => configPanel.gameObject.SetActive(true);

        public void OnClickMatching() => matchingPanel.gameObject.SetActive(true);
    }
}