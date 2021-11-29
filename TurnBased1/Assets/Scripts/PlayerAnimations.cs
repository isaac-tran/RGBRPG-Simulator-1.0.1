using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public IEnumerator PlayAttackAnimation(GameObject target)
    {
        Vector3 prevPosition = transform.position;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        Vector3 direction = target.transform.position - transform.position;
        rb.AddForce(target.transform.position * 500);

        yield return new WaitForSeconds(0.5f);

        rb.velocity = Vector3.down * 3;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = prevPosition + new Vector3(0, 3, 0);

        yield return new WaitForSeconds(0.3f);

        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
