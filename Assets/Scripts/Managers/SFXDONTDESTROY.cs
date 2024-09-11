using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXDONTDESTROY : MonoBehaviour
{

    public static SFXDONTDESTROY instance;
    [SerializeField] public float SFXValue;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public float GetSFXValue
    {
        get
        {
            return SFXValue;
        }
        set
        {
            SFXValue = value;
        }
    }
}
