using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class WinScreen : MonoBehaviour
{
    public GameObject PlayButton;
    public GameObject FinishTIme;
    public Button PlayAgainButton;
    void Start()
    {
        PlayAgainButton.onClick.AddListener(PlayAgain);
    }
    void PlayAgain()
    {
        GameManager.Instance.Restart();
        AuthenticationService.Instance.SignOut();
    }
    
}