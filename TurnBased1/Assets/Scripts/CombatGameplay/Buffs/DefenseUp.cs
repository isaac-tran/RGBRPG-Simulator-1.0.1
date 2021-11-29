using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUp : TemporaryEffect
{
    public CombatManager combatManager;
    private float upAmount = 0;

    public float GetUpAmount() { return upAmount; }

    public void SetUpAmount(float newUpAmount)
    {
        upAmount = Mathf.Clamp(newUpAmount, 0, Mathf.Infinity);
    }

    // Start is called before the first frame update
    public override void Activate()
    {
        gameObject.GetComponent<Combatant>().defenseModifier += upAmount;
    }

    public override void Deactivate()
    {
        gameObject.GetComponent<Combatant>().defenseModifier -= upAmount;
        combatManager.UpdateNarrationLine(
           gameObject.name + "'s defense went back to " + Mathf.Round(gameObject.GetComponent<Combatant>().defenseModifier * 100) + "%!"
       );
    }

    private void OnDestroy()
    {
        Deactivate();
    }
}
