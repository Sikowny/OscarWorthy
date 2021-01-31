using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    public GameObject Player;
    public float SpawnDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Player, transform.position + transform.forward * SpawnDistance, Quaternion.Euler(0,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
