using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHPBar : MonoBehaviour
{
    public Combatant assignedCombatant;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Slider>().value = assignedCombatant.GetHP() / assignedCombatant.GetMaxHP();
    }
}
