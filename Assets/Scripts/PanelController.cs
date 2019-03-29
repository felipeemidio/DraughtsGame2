using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelController : MonoBehaviour {

    private AudioSource audioSource;
    private AudioClip winSound;
    private AudioClip loseSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        winSound = (AudioClip)Resources.Load("Sounds/funwithsound_success-fanfare-trumpets");
        loseSound = (AudioClip)Resources.Load("Sounds/taranp_horn-fail-wahwah-1");
    }

    // Update is called once per frame
    void Update () {
        // Restart scene when press 'R'.
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    // Play a sound of victory or defeat.
    public void PlaySound(bool hasPlayerWin)
    {
        if (hasPlayerWin)
        {
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            audioSource.PlayOneShot(loseSound);
        }
    }
}
