using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Space2Double : MonoBehaviour
{
    [SerializeField] private Animation Space2Anim;
    [SerializeField] private Image SpaceImage;

    private void Awake()
    {
        SpaceImage.enabled = false;
    }

    public void SpaceAnim()
    {
        SpaceImage.enabled = true;
        Space2Anim.Play();
        Destroy(gameObject);
        SoundManager.PlaySound(SoundType.GRAPPLEON, SoundManager.Instance.GetSFXVolume());
    }
}