using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace AoAndSugi
{
    public sealed class WaitPanel : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;

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
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            this.gameObject.SetActive(false);
        }
    }
}
