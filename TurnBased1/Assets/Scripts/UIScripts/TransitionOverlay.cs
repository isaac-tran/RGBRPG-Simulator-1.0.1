using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionOverlay : MonoBehaviour
{
    Image thisOverlay;

    /// <summary>
    /// Fades away the transition overlay til completely invisible
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator FadeIn(float duration)
    {
        float remainingTime = duration;

        Color c = thisOverlay.color;

        while (thisOverlay.color.a > 0)
        {
            //  Lerps alpha towards 0
            thisOverlay.color = new Color(
                c.r, c.g, c.b,
                Mathf.Lerp(0f, 1f, remainingTime / duration)
            );

            //  Deduce time.deltatime from timer
            remainingTime -= Time.deltaTime;

            //  Update c

            yield return null;
        }

        yield return null;
    }

    public IEnumerator FadeOut(float duration, Color endColor)
    {
        /*float remainingTime = duration;

        Color c = thisOverlay.color;

        while (thisOverlay.color.a > 0)
        {
            //  Lerps alpha towards 0
            thisOverlay.color = new Color(
                c.r, c.g, c.b,
                Mathf.Lerp(0f, 1f, 1 - (remainingTime / duration))
            );

            //  Deduce time.deltatime from timer
            remainingTime -= Time.deltaTime;

            //  Update c

            yield return null;
        }*/

        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisOverlay = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
