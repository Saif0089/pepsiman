using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public GameObject PlayButton;
    public Button PlayAgainButton;
    void Start()
    {
        Breath();
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
    void PlayAgain()
    {
        GameManager.Instance.Restart();
    }
    
}