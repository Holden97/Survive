using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    AudioSource buttonSource;
    public AudioClip hoverClip;
    public AudioClip downClip;
    private void Awake()
    {
        buttonSource = gameObject.AddComponent<AudioSource>();
        buttonSource.playOnAwake = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonSource.PlayOneShot(downClip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonSource.PlayOneShot(hoverClip);
    }
    

}
