using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugTurret : MonoBehaviour
{
    Vector2 rotationVector;
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        rotationVector = new Vector2();
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rotationVector = player.transform.position - transform.position;
        transform.up = rotationVector;
    }
}
