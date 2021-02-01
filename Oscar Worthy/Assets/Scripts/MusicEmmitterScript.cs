using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicEmmitterScript : MonoBehaviour
{
    public AudioClip LevelMusic;
    public AudioClip MenuMusic;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");
    }

    public List<string> levelScenes = new List<string>();
    public List<string> menuScenes = new List<string>();
    void OnSceneLoaded(Scene scene, LoadSceneMode sm)
    {
        Debug.Log(audioSource.clip);
        if (levelScenes.Contains(scene.name))
        {
            if (audioSource.clip != LevelMusic)
            {
                audioSource.clip = LevelMusic;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != MenuMusic)
            {
                audioSource.clip = MenuMusic;
                audioSource.Play();
            }
        }
        
    }
}
