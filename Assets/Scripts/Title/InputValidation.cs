using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;

namespace AoAndSugi
{
    public sealed class InputValidation : MonoBehaviour
    {
        [SerializeField] MessagePanel messagePanel;

        public string CheckInputString(string inputText, GameObject deployer)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return null;
            }

            //取り敢えず英数字のみ許容、文字数は1～7
            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[a-z0-9]+$");
            var match = pattern.Match(inputText);
            if (!(1 <= match.Length && match.Length <= 7))
            {
                var panel = Instantiate(messagePanel, deployer.transform);
                panel.Initialized("Please enter between 1 and 7 characters", null);
                return null;
            }
            return match.ToString();
        }
    }
}

