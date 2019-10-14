using UnityEngine;
using UnityEngine.UI;

public sealed class MatchingPanel : MonoBehaviour
{

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
