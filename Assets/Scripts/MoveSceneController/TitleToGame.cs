using UnityEngine;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class TitleToGame : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

        public void OnClick()
        {
            sceneLoader.LoadScene("Game");
        }
    }
}