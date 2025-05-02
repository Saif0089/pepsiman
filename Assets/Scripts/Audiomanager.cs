using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Audiomanager : MonoBehaviour
{
    public static Audiomanager instance;

    public AudioSource audi_bg;
    public AudioSource audi_Effect;
    public AudioSource SkateBoard_Source;
    public AudioSource Player_Source;
    public AudioClip bg_2;
    public AudioClip Running;
    public AudioClip clip_collision;
    public AudioClip clip_coins;
    public AudioClip clip_bag;
    public AudioClip Jump_Clip;
    public AudioClip Slide_Clip;
    public AudioClip CountDown_Clip;
    public AudioClip Stumble_Clip;
    public AudioClip PickSkate_Clip;
    public AudioClip Skate_JumpClip;
    public AudioClip Skate_Land;
    public AudioClip Speed_1;
    public AudioClip Speed_2;
    public AudioClip Win;
    private void Awake()
    {
        instance = this;
    }
    
    public void Play_Win()
    {
        if (Win != null)
        {
            audi_Effect.PlayOneShot(Win,1f);
        }
    }
    public void Play_SkateJump()
    {
        if (Skate_JumpClip != null)
        {
            audi_Effect.PlayOneShot(Skate_JumpClip,1f);
        }
    }
    public void Play_SkateLand()
    {
        if (Skate_Land != null)
        {
            audi_Effect.PlayOneShot(Skate_Land,1f);
        }
    }

    public void PlaySkatePickClip()
    {
        if (PickSkate_Clip != null)
        {
            audi_Effect.PlayOneShot(PickSkate_Clip,1f);
        }
    }

    public void Play_StumbleClip()
    {
        if (Stumble_Clip != null)
        {
            audi_Effect.PlayOneShot(Stumble_Clip, 1f);
        }
    }

    public void PlayCountDown_Time()
    {
        if (CountDown_Clip != null)
        {
            audi_Effect.PlayOneShot(CountDown_Clip,1f);
        }
    }

    public void PlayJump_Sfx()
    {
        if (Jump_Clip != null)
        {
            audi_Effect.PlayOneShot(Jump_Clip,1f);
        }
    }

    public void PlaySlide_Sfx()
    {
        if (Slide_Clip != null)
        {
            audi_Effect.PlayOneShot(Slide_Clip,1f);
        }
    }

    public void PlaySfx_Collision()
    {
        if (clip_collision != null)
        {
            audi_Effect.PlayOneShot(clip_collision,1f);
        }
    }

    public void PlaySfx_Coins()
    {
        if (clip_coins != null)
        {
            audi_Effect.PlayOneShot(clip_coins,1f);
        }
    }

    public void PlaySfx_bag()
    {
        if (clip_bag != null)
        {
            audi_Effect.PlayOneShot(Speed_1, 1f);

            Player_Source.clip = Speed_2;
            Player_Source.Play();
        }
    }
}