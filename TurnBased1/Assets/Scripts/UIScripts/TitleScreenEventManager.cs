using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenEventManager : MonoBehaviour
{
    public TransitionOverlay transitionOverlay;
    public AudioManager audioManager;

    public IEnumerator LoadMainScene()
    {
        transitionOverlay.gameObject.GetComponent<Image>().color =
            new Color(0.5f, 0.5f, 0.5f, 0.6f);

        transitionOverlay.gameObject.SetActive(true);

        audioManager.StopMusic("TitleScreenMusic");

        yield return new WaitForSeconds(2f);

        transitionOverlay.gameObject.SetActive(false);

        SceneManager.LoadScene("SampleScene");

        yield return null;
    }

    IEnumerator Init()
    {
        yield return StartCoroutine(transitionOverlay.FadeIn(1.5f));
        transitionOverlay.gameObject.SetActive(false);
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
