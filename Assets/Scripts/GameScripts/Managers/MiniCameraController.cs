using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    Vector3 playerLastPosition;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CorrectPosition();
    }

    private void CorrectPosition()
    {
        Vector3 moveVector = player.transform.position - playerLastPosition;
        if (moveVector == Vector3.zero)
            return;
        playerLastPosition = player.transform.position;
        gameObject.transform.position += moveVector;
    }
}
