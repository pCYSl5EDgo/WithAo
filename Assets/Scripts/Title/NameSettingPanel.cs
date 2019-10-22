using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;
using System.Collections;

namespace AoAndSugi
{
    public sealed class NameSettingPanel : MonoBehaviour
    {
        [Inject] private MatchingPanel matchingPanel;

        [Inject] private InputValidation inputValidation;

        [Inject] private ConnectingPanel connectingPanel;

        [SerializeField] TMP_InputField field;

        public void OnClickClose() => gameObject.SetActive(false);

        public void OnClickNext() {
            StartCoroutine(OnEndEdit());
        }
        
        public IEnumerator OnEndEdit()
        {
            var correctText = inputValidation.CheckInputString(field.text, this.gameObject);
            if (!(string.IsNullOrEmpty(correctText)))
            {
                if (!PhotonNetwork.IsConnected)
                {
                    connectingPanel.gameObject.SetActive(true);
                    while (!PhotonNetwork.IsConnected)
                    {
                        yield return null;
                    }
                    connectingPanel.gameObject.SetActive(false);
                }
                PhotonNetwork.LocalPlayer.NickName = correctText;
                matchingPanel.gameObject.SetActive(true);
            }
        }
    }
}

