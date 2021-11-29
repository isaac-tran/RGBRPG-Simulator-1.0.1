using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWonEventManager : MonoBehaviour
{
    public TransitionOverlay transitionOverlay;
    public AudioManager audioManager;

    public bool gameWon;

    public IEnumerator LoadMainScene()
    {
        SceneManager.LoadScene(0);

        yield return null;
    }

    IEnumerator Init()
    {
        if (gameWon)
            audioManager.PlayMusic("TitleScreenMusic");

        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
