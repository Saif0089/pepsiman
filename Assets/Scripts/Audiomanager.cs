using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    public static Audiomanager instance;

    public AudioSource audi_bg;
    public AudioSource audi_Effect;
    public AudioClip clip_collision;
    public AudioClip clip_coins;
    public AudioClip clip_bag;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void PlaySfx_Collision()
    {
        audi_Effect.PlayOneShot(clip_collision);
    }
    public void PlaySfx_Coins()
    {
        audi_Effect.PlayOneShot(clip_coins);
    }
    public void PlaySfx_bag()
    {
        audi_Effect.PlayOneShot(clip_bag,1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
