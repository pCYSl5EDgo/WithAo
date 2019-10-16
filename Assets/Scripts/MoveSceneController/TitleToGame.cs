using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AoAndSugi.MoveSceneController
{
    public sealed class TitleToGame : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

        [Inject] private ConfigPanel configPanel;

        [Inject] private NameSettingPanel nameSettingPanel;

        public void OnClickToGame() => sceneLoader.LoadScene("Game");

        public void OnClickSettings() => configPanel.gameObject.SetActive(true);

        public void OnClickMatching() => nameSettingPanel.gameObject.SetActive(true);
    }
}