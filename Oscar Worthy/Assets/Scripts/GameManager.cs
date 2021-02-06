using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Intended to be used as a singleton, but not enforcing it
public class GameManager : MonoBehaviour
{
    public static UpdateManager UpdateManager = new UpdateManager();

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateManager.Update(Time.deltaTime);
    }
}
