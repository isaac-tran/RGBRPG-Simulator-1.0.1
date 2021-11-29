using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPRegen : TemporaryEffect
{
    public CombatManager combatManager;
    private float healAmount = 0;

    public float GetHealAmount() { return healAmount; }

    public void SetHealAmount(float newUpAmount)
    {
        healAmount = Mathf.Clamp(newUpAmount, 0, Mathf.Infinity);
    }

    // Start is called before the first frame update
    public override void Activate()
    {
        float amount = Mathf.Round(healAmount * Random.Range(1, 1.1f));

        gameObject.GetComponent<Combatant>().RestoreHP(amount);
        combatManager.UpdateNarrationLine(
            gameObject.name + " recovered " + amount + " HP!"
        );
    }

    public override void Deactivate()
    {
        combatManager.UpdateNarrationLine(
           gameObject.name + "'s HP regeneration effect wore off!"
       );
    }

    private void OnDestroy()
    {
        Deactivate();
    }
}
