using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimations : MonoBehaviour
{
    private Transform rightHand;
    private Rigidbody rightHandRb;

    public IEnumerator PlayAttackAnimation(GameObject target)
    {
        Vector3 prevRightHandPosition = rightHand.transform.position;

        rightHand.position = target.transform.position + new Vector3(0, 15, 0);
        rightHandRb.velocity = new Vector3(0, -25, 0);
        rightHandRb.useGravity = true;

        yield return new WaitForSeconds(0.9f);

        rightHand.position = prevRightHandPosition;
        rightHandRb.useGravity = false;
        rightHandRb.velocity = Vector3.zero;

        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        rightHand = transform.Find("RightHand");
        rightHandRb = rightHand.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
