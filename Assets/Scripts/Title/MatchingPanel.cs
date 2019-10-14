using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AoAndSugi
{
    public sealed class MatchingPanel : MonoBehaviour
    {
        [Inject] private CreateNewRoomPanel matchingPanel;

        public void OnClickCreateNewRoom() => matchingPanel.gameObject.SetActive(true);

        public void OnClickClose() => gameObject.SetActive(false);
    }
}
