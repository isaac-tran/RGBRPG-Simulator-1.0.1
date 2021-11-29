using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPRegenBasedOnDamageDealt : TemporaryEffect
{
    public CombatManager combatManager;
    private float healPercentage = 0;
    private float damageDealt;

    public float GetHealPercentage() { return healPercentage; }

    public void SetHealPercentage(float newUpAmount)
    {
        healPercentage = Mathf.Clamp(newUpAmount, 0, Mathf.Infinity);
    }

    public float GetDamageDealt() { return damageDealt; }

    public void SetDamageDealt(float newDamageDealt)
    {
        damageDealt = Mathf.Clamp(newDamageDealt, 0, Mathf.Infinity);
    }

    // Start is called before the first frame update
    public override void Activate()
    {
        float amount = Mathf.Round(healPercentage * damageDealt);

        gameObject.GetComponent<Combatant>().RestoreMP(amount);
        combatManager.UpdateNarrationLine(
            gameObject.name + " recovered " + amount + " MP!"
        );
    }

    public override void Deactivate()
    {
        combatManager.UpdateNarrationLine(
           gameObject.name + "'s Siphon effect wore off!"
       );
    }

    private void OnDestroy()
    {
        Deactivate();
    }
}
