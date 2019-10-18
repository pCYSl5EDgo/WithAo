using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AoAndSugi
{
    public sealed class MatchingPanel : MonoBehaviour
    {
        [Inject] private CreateNewRoomPanel matchingPanel;

        [Inject] private CurrentRoomListPanel currentRoomListPanel;

        [Inject] private EnterPrivateRoomPanel enterPrivateRoomPanel;

        public void OnClickCreateNewRoom() => matchingPanel.gameObject.SetActive(true);

        public void OnClickCurrentRoom() => currentRoomListPanel.gameObject.SetActive(true);

        public void OnClickEnterPrivateRoom() => enterPrivateRoomPanel.gameObject.SetActive(true);

        public void OnClickClose() => gameObject.SetActive(false);
    }
}
