using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneThemeSound : MonoBehaviour
{
    [SerializeField] private string ThemeSound;

    void Start()
    {   
        SoundManager.instance.StopAll();     
        SoundManager.instance.Play(ThemeSound);
    }
}
