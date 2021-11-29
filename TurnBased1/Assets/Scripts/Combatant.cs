using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    [SerializeField] private float hp, maxHp = 100, mp, maxMp = 100;
    public float attack, defense, magic, resistance, speed;
    public float attackModifier = 1, defenseModifier = 1, 
        magicModifier = 1, resistanceModifier = 1, speedModifier = 1;

    private bool isDefending = false;

    //  Misc
    Vector3 origPosition;

    public List<int> availableSpells;

    public float GetHP()
    {
        return hp;
    }
    public float GetMP()
    {
        return mp;
    }

    public float GetMaxHP()
    {
        return maxHp;
    }

    public float GetMaxMP()
    {
        return maxMp;
    }

    public void SetHP(float newHP)
    {
        hp = Mathf.Clamp(newHP, 0, maxHp);
    }
    public void SetMP(float newHP)
    {
        mp = Mathf.Clamp(newHP, 0, maxMp);
    }

    public void ReduceHP(float byValue)
    {
        hp = Mathf.Clamp(hp - byValue, 0, maxHp);
    }

    public void RestoreHP(float byValue)
    {
        hp = Mathf.Clamp(hp + byValue, 0, maxHp);
    }

    public void ReduceMP(float byValue)
    {
        mp = Mathf.Clamp(mp - byValue, 0, maxMp);
    }

    public void RestoreMP(float byValue)
    {
        mp = Mathf.Clamp(mp + byValue, 0, maxMp);
    }

    public bool GetDefendingStatus() { return isDefending; }

    public void SetDefendingStatus(bool newDefendingStatus)
    {
        isDefending = newDefendingStatus;
    }
    public void ResetPositionAndMomentum()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = origPosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        origPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
