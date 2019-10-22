using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace AoAndSugi
{
    public sealed class WaitPanel : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

        [Inject] private ConnectingPanel connectingPanel;

        [SerializeField] TextMeshProUGUI count;
            
        public void OnClickClose() => gameObject.SetActive(false);

        public void UpdatePlayerCount()
        {
            count.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
            if(PhotonNetwork.CurrentRoom.MaxPlayers <= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                //ゲームシーンに移動
                sceneLoader.LoadScene("Game");
            }
        }

        public void OnClickLeave()
        {
            StartCoroutine(LeaveRoom());
        }

        private IEnumerator LeaveRoom()
        {
            if (!PhotonNetwork.InRoom)
            {
                connectingPanel.gameObject.SetActive(true);
                while (!PhotonNetwork.InRoom)
                {
                    yield return null;
                }
                connectingPanel.gameObject.SetActive(false);
            }
            PhotonNetwork.LeaveRoom();
            this.gameObject.SetActive(false);
        }
    }
}
