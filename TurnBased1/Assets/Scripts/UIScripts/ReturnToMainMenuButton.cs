using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMainMenuButton : MonoBehaviour
{
    public GameWonEventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        StartCoroutine(eventManager.LoadMainScene());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
