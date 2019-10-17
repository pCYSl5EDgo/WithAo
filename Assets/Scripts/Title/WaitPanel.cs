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

        //自分が入ってきたとき
        private void Start()
        {
            //UpdatePlayerCount();
        }

        public void UpdatePlayerCount()
        {
            var a = count.text;
            var b = PhotonNetwork.CurrentRoom.PlayerCount;
            var c = PhotonNetwork.CurrentRoom.MaxPlayers;
            count.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
            if(PhotonNetwork.CurrentRoom.MaxPlayers <= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                //ゲームシーンに移動
                sceneLoader.LoadScene("Game");
            }
        }
    }
}
