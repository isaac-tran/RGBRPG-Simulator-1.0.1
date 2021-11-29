using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporaryEffect : MonoBehaviour
{
    public int turnsRemaining = 1;
    public bool continuousActivationEffect = false;

    public abstract void Activate();
    public abstract void Deactivate();

    public void SetTurnRemaining(int newTurnRemaining)
    {
        turnsRemaining = newTurnRemaining;
    }

    public void CountTurnDown()
    {
        if (continuousActivationEffect)
            Activate();

        turnsRemaining--;
        if (turnsRemaining == 0)
            Destroy(this);

    }
}
