using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public CombatManager combatManager;

    /// <summary>
    /// Returns an int representing which action the boss chose.
    /// 1 for attack single target
    //  2 for attack ALL target
    //  3 for heal
    /// </summary>
    /// <returns>An int representing which action the boss chose</returns>
    public int ChooseAction()
    {
        //  1 for attack single target
        //  2 for attack ALL target
        //  3 for heal
        return 1;
    }

    public int ChooseTarget(bool targetEnemies, bool targetAllies, bool skipKOedTargets)
    {
        int index = (int)Random.Range(0, 3);

        if (skipKOedTargets)
        {
            GameObject[] partyMemberArray = combatManager.GetPartyMemberArray();
            float targetHP = partyMemberArray[index].GetComponent<Combatant>().GetHP();

            //  If target is KO-ed, switch to new target
            while (targetHP <= 0)
            {
                //  Roll new target
                index = (int)Random.Range(0, 3);

                //  Get new target's HP
                targetHP = partyMemberArray[index].GetComponent<Combatant>().GetHP();
            }
        }

        return index;
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
