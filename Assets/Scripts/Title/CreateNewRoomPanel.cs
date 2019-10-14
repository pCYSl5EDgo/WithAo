using UnityEngine;
using UnityEngine.UI; 

namespace AoAndSugi
{
    public sealed class CreateNewRoomPanel : MonoBehaviour
    { 
        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }
    }
}

