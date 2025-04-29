using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    public static Audiomanager instance;

    public AudioSource audi_bg;
    public AudioSource audi_Effect;
    public AudioClip clip_collision;
    public AudioClip clip_coins;
    public AudioClip clip_bag;
    public AudioClip Jump_Clip;
    public AudioClip Slide_Clip;
    public AudioClip CountDown_Clip;
    public AudioClip Stumble_Clip;
    public AudioClip PickSkate_Clip;
    public AudioClip Skate_JumpClip;
    public AudioClip Skate_SlideClip;
    private void Awake()
    {
        instance = this;
    }
    public void Play_SkateJump()
    {
        if (Skate_JumpClip != null)
        {
            audi_Effect.PlayOneShot(Skate_JumpClip);
        }
    }
    
    public void Play_SkateSlideClip()
    {
        if (Skate_SlideClip != null)
        {
            audi_Effect.PlayOneShot(Skate_SlideClip);
        }
    }
    public void PlaySkatePickClip()
    {
        if (PickSkate_Clip != null)
        {
            audi_Effect.PlayOneShot(PickSkate_Clip);
        }
    }
    public void Play_StumbleClip()
    {
        if (Stumble_Clip != null)
        {
            audi_Effect.PlayOneShot(Stumble_Clip);
        }
    }
    public void PlayCountDown_Time()
    {
        if (CountDown_Clip != null)
        {
            audi_Effect.PlayOneShot(CountDown_Clip);
        }
    }
    public void PlayJump_Sfx()
    {
        if (Jump_Clip != null)
        {
            audi_Effect.PlayOneShot(Jump_Clip);
        }
    }
    public void PlaySlide_Sfx()
    {
        if (Slide_Clip != null)
        {
            audi_Effect.PlayOneShot(Slide_Clip);
        }
    }
    public void PlaySfx_Collision()
    {
        if (clip_collision != null)
        {
            audi_Effect.PlayOneShot(clip_collision);
        }
    }
    public void PlaySfx_Coins()
    {
        if (clip_coins != null)
        {
            audi_Effect.PlayOneShot(clip_coins);
        }
    }

    public void PlaySfx_bag()
    {
        if (clip_bag != null)
        {
            audi_Effect.PlayOneShot(clip_bag, 1f);
        }
    }
}