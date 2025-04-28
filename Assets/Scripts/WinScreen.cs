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
        Breath();
        BreathTime();
        PlayAgainButton.onClick.AddListener(PlayAgain);
    }
    void Breath()
    {
        if (PlayButton != null)
        {
            PlayButton.transform.DOScale(1.2f, 0.8f) 
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo) 
                .SetUpdate(true); 
        }
    }
    void BreathTime()
    {
        if (FinishTIme != null)
        {
            FinishTIme.transform.DOScale(1.2f, 0.8f) 
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true); 
        }
    }
    void PlayAgain()
    {
        GameManager.Instance.Restart();
        AuthenticationService.Instance.SignOut();
    }
    
}