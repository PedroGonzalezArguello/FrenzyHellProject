using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAvatar : MonoBehaviour
{
    private SoundManager soundManager;
    public void ReloadSFX()
    {
       SoundManager.PlaySound(SoundType.RELOAD, SoundManager.Instance.GetSFXVolume());
    }
}
