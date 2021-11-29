using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public PlayerChooseActionMenu playerChooseActionMenu;
    public SpellList spellList;
    public TargetList targetList;
    public Text narrationText;

    private string previousNarrationLine = "";
    private string currentNarrationLine = "";

    GameObject[] bossArray;
    GameObject[] partyMemberArray;
    List<GameObject> combatantTurnQueue;

    int[] playerChoicesIndices = new int[3];
    int playerTargetChoiceIndex;    //  0 for ALL targets

    public AudioManager audioManager;

    //  Specific for computing turns
    Combatant currentCombatantInAction;
    bool isPlayerTurn = true;

    public GameObject[] GetBossArray()
    {
        return (GameObject[])bossArray.Clone();
    }

    public GameObject[] GetPartyMemberArray()
    {
        return (GameObject[])partyMemberArray.Clone();
    }

    public GameObject GetCurrentCombatantInAction()
    {
        return currentCombatantInAction.gameObject;
    }

    public int[] GetPlayerChoicesIndices()
    {
        return (int[])playerChoicesIndices.Clone();
    }

    /// <summary>
    /// Saves the player's choice into the array.
    /// </summary>
    /// <param name="layer"> Layer start from 0 </param>
    /// <param name="choiceIndex"></param>
    public void SetPlayerChoicesIndices(int layer, int choiceIndex)
    {
        //  Clear all layers that comes after layer
        for (int i = 2; i > layer; i--)
            playerChoicesIndices[i] = 0;

        playerChoicesIndices[layer] = choiceIndex;
    }

    public int GetPlayerTargetChoiceIndex()
    {
        return playerTargetChoiceIndex;
    }

    public void SetPlayerTargetChoiceIndex(int newIndex)
    {
        playerTargetChoiceIndex = newIndex;
    }

    public int GetNumberOfAlivePartyMembersInPlay()
    {
        int alivePartyMembers = 0;

        for (int i = 0; i < partyMemberArray.Length; i++)
        {
            if (partyMemberArray[i].GetComponent<Combatant>().GetHP() > 0)
                alivePartyMembers++;
        }

        return alivePartyMembers;
    }

    public int GetNumberOfAliveBossesInPlay()
    {
        int aliveBosses = 0;

        for (int i = 0; i < bossArray.Length; i++)
        {
            if (bossArray[i].GetComponent<Combatant>().GetHP() > 0)
                aliveBosses++;
        }

        return aliveBosses;
    }

    public void UpdateNarrationLine(string newNarrationLine)
    {
        previousNarrationLine = currentNarrationLine;
        currentNarrationLine = newNarrationLine;

        narrationText.text = previousNarrationLine + "\n" + currentNarrationLine;
    }

    void CountDownTemporaryEffects()
    {
        TemporaryEffect[] tempEffects = currentCombatantInAction.GetComponents<TemporaryEffect>();

        for (int i = 0; i < tempEffects.Length; i++)
            tempEffects[i].CountTurnDown();
    }

    void InitiateNewTurn()
    {
        if (isPlayerTurn)
            StartCoroutine(PlayPlayerTurn());
        else
            StartCoroutine(PlayBossTurn());
    }

    IEnumerator PlayPlayerTurn()
    {
        InitiatePlayerTurn();

        yield return new WaitUntil(() => playerChooseActionMenu.GetPlayerFinishedChoosingBool() == true);

        //Debug.Log("finished choosing");
        playerChooseActionMenu.gameObject.SetActive(false);
        spellList.gameObject.SetActive(false);
        targetList.gameObject.SetActive(false);

        yield return StartCoroutine(ExecutePlayerCommand());

        EndPlayerTurn();

        yield return null;
    }

    IEnumerator PlayBossTurn()
    {
        InitiateBossTurn();

        yield return ExecuteBossCommand();

        EndBossTurn();

        yield return null;
    }

    void InitiatePlayerTurn()
    {
        currentCombatantInAction.ResetPositionAndMomentum();

        //  Reset defense status
        currentCombatantInAction.SetDefendingStatus(false);

        //  Count down buff durations
        CountDownTemporaryEffects();

        //  Show UI for player to choose action
        if (isPlayerTurn)
            playerChooseActionMenu.gameObject.SetActive(true);

        //  Set condition to wait for player input
        playerChooseActionMenu.SetPlayerFinishedChoosingBool(false);

        audioManager.PlaySound("ShowPlayerActionChoiceMenu");
    }

    void InitiateBossTurn()
    {

    }

    public IEnumerator ExecutePlayerCommand()
    {
        switch (playerChoicesIndices[0])
        {
            //  Attack
            case 1:
                yield return StartCoroutine(ExecuteAttack());
                break;

            //  Spell
            case 2:
                yield return StartCoroutine(ExecuteSpell());
                break;

            //  Defend
            case 3:
                yield return StartCoroutine(ExecuteDefend());
                break;
        }

        yield return null;
    }

    public IEnumerator ExecuteBossCommand()
    {
        //  Pick command
        BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
        int actionChoiceIndex = bossAI.ChooseAction();

        //  Execute command
        switch (actionChoiceIndex)
        {
            case 1:
                yield return StartCoroutine(ExecuteAttack());
                break;

            case 2: break;

            case 3: break;
        }

        yield return null;
    }

    public IEnumerator ExecuteAttack()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();
        float damageDealt;

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = bossArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(true, false, true);

            target = partyMemberArray[targetIndex].GetComponent<Combatant>();

            yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        //  Damage dealt is modified to be 90% - 110% effective
        damageDealt = currentCombatantInAction.attack * (100 / (target.defense * target.defenseModifier));
        damageDealt *= Random.Range(0.9f, 1.1f);

        //  If target is defending, cuts damage to 40%
        if (target.GetDefendingStatus())
            damageDealt *= 0.4f;

        target.ReduceHP(Mathf.RoundToInt(damageDealt));

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " attacks! " +
            target.name + " takes " + Mathf.RoundToInt(damageDealt) + " damage!"
        );

        MPRegenBasedOnDamageDealt[] siphonEffects = currentCombatantInAction.GetComponents<MPRegenBasedOnDamageDealt>();
        foreach (MPRegenBasedOnDamageDealt effect in siphonEffects)
        {
            effect.SetDamageDealt(damageDealt);
            effect.Activate();
        }

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    public IEnumerator ExecuteSpell()
    {
        Spell spellChosen = spellList.spellGlossary[playerChoicesIndices[1]];
        string spellName = spellChosen.spellName;

        currentCombatantInAction.ReduceMP(spellChosen.spellCost);

        switch (spellName)
        {
            case "Red Comet":
                yield return StartCoroutine(ExecuteRedComet());
                break;

            case "Raise Attack":
                yield return StartCoroutine(ExecuteRaiseAttack());
                break;

            case "Heal":
                yield return StartCoroutine(ExecuteHeal());
                break;

            case "Repair":
                yield return StartCoroutine(ExecuteRepair());
                break;

            case "Siphon":
                yield return StartCoroutine(ExecuteSiphon());
                break;

            case "Raise Defense":
                yield return StartCoroutine(ExecuteRaiseDefense());
                break;
        }

        yield return null;
    }

    private IEnumerator ExecuteRedComet()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();
        float damageDealt;

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = bossArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(true, false, true);

            target = partyMemberArray[targetIndex].GetComponent<Combatant>();

            yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        //  Damage dealt is modified to be 90% - 110% effective
        //  Red comet uses 280% of attack 
        damageDealt = currentCombatantInAction.attack * 2.8f * (100 / (target.defense * target.defenseModifier));
        damageDealt *= Random.Range(0.9f, 1.1f);

        //  If target is defending, cuts damage to 40%
        if (target.GetDefendingStatus())
            damageDealt *= 0.4f;

        target.ReduceHP(Mathf.RoundToInt(damageDealt));

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Red Comet! " +
            target.name + " takes " + Mathf.RoundToInt(damageDealt) + " damage!"
        );

        MPRegenBasedOnDamageDealt[] siphonEffects = currentCombatantInAction.GetComponents<MPRegenBasedOnDamageDealt>();
        foreach (MPRegenBasedOnDamageDealt effect in siphonEffects)
        {
            effect.SetDamageDealt(damageDealt);
            effect.Activate();
        }

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    private IEnumerator ExecuteHeal()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();
        float healAmount;

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = partyMemberArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            //yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(false, true, true);

            target = bossArray[targetIndex].GetComponent<Combatant>();

            //yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        //  Heal amount is modified to be 100% - 110% effective
        healAmount = 250;
        healAmount *= Random.Range(1f, 1.1f);

        target.RestoreHP(Mathf.RoundToInt(healAmount));

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Heal! " +
            target.name + " is healed for " + Mathf.RoundToInt(healAmount) + " HP!"
        );

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    private IEnumerator ExecuteRaiseAttack()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = partyMemberArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            //yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(false, true, true);

            target = bossArray[targetIndex].GetComponent<Combatant>();

            //yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        AttackUp atkUp = target.gameObject.AddComponent<AttackUp>();
        atkUp.combatManager = this;
        atkUp.SetUpAmount(0.3f);    //  Ups by 30%
        atkUp.SetTurnRemaining(4);
        atkUp.Activate();

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Raise Attack! " +
            target.name + " attack is increased to " + Mathf.Round(target.GetComponent<Combatant>().attackModifier * 100) + "%!"
        );

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    private IEnumerator ExecuteRaiseDefense()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = partyMemberArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            //yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(false, true, true);

            target = bossArray[targetIndex].GetComponent<Combatant>();

            //yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        DefenseUp defUp = target.gameObject.AddComponent<DefenseUp>();
        defUp.combatManager = this;
        defUp.SetUpAmount(0.3f);    //  Ups by 30%
        defUp.SetTurnRemaining(4);
        defUp.Activate();

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Raise Defense! " +
            target.name + " defense is increased to " + Mathf.Round(target.GetComponent<Combatant>().defenseModifier * 100) + "%!"
        );

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    private IEnumerator ExecuteRepair()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = partyMemberArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            //yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(false, true, true);

            target = bossArray[targetIndex].GetComponent<Combatant>();

            //yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        HPRegen hpRegenEffect = target.gameObject.AddComponent<HPRegen>();
        hpRegenEffect.combatManager = this;
        hpRegenEffect.continuousActivationEffect = true;
        hpRegenEffect.SetHealAmount(100f);    //  Ups by 30%
        hpRegenEffect.SetTurnRemaining(4);

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Repair! " +
            target.name + " now recovers HP over time!"
        );

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    private IEnumerator ExecuteSiphon()
    {
        Combatant target = bossArray[0].GetComponent<Combatant>();

        //  If player attacks boss
        if (currentCombatantInAction.gameObject.tag == "Party Member")
        {
            target = partyMemberArray[playerTargetChoiceIndex].GetComponent<Combatant>();
            //yield return StartCoroutine(currentCombatantInAction.GetComponent<PlayerAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //  If boss attacks player
        else if (currentCombatantInAction.gameObject.tag == "Boss")
        {
            BossAI bossAI = currentCombatantInAction.GetComponent<BossAI>();
            int targetIndex = bossAI.ChooseTarget(false, true, true);

            target = bossArray[targetIndex].GetComponent<Combatant>();

            //yield return StartCoroutine(currentCombatantInAction.GetComponent<BossAnimations>().PlayAttackAnimation(target.gameObject));
        }

        //Debug.Log("Target Name: " + target.name + " | Target HP: " + target.GetComponent<Combatant>().GetHP());

        MPRegenBasedOnDamageDealt siphonEffect = target.gameObject.AddComponent<MPRegenBasedOnDamageDealt>();
        siphonEffect.combatManager = this;
        siphonEffect.continuousActivationEffect = false;
        siphonEffect.SetHealPercentage(0.24f);    //  Ups by 30%
        siphonEffect.SetTurnRemaining(4);

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " casts Repair! " +
            target.name + " now recovers MP based on damage dealt to enemies!"
        );

        //Debug.Log(currentCombatantInAction.name + " attacks! " +
        //    target.name + " took " + Mathf.RoundToInt(damageDealt) + " damage!");

        yield return null;
    }

    public IEnumerator ExecuteDefend()
    {
        currentCombatantInAction.SetDefendingStatus(true);

        //  Narration line
        UpdateNarrationLine(
            currentCombatantInAction.name + " is defending! "
        );

        yield return null;
    }

    void EndPlayerTurn()
    {
        playerChooseActionMenu.gameObject.SetActive(false);

        bool gameOver = CheckForGameOverCondition();
        if (!gameOver)
        {
            SwitchToNextCombatant();
            InitiateNewTurn();
        }
        else
            EndGame();
    }

    void EndBossTurn()
    {
        bool gameOver = CheckForGameOverCondition();
        if (!gameOver)
        {
            SwitchToNextCombatant();
            InitiateNewTurn();
        }
        else
            EndGame();
    }

    private void SwitchToNextCombatant()
    {
        //  Shift first combatant (one that just made a turn) to end of queue
        combatantTurnQueue.Add(combatantTurnQueue[0]);
        combatantTurnQueue.RemoveAt(0);

        //  Skips over all combatants with 0 HP (aka KO-ed) until found one with > 0 HP
        float combatantHP = combatantTurnQueue[0].GetComponent<Combatant>().GetHP();
        while (combatantHP <= 0)
        {
            combatantTurnQueue.Add(combatantTurnQueue[0]);
            combatantTurnQueue.RemoveAt(0);

            combatantHP = combatantTurnQueue[0].GetComponent<Combatant>().GetHP();
        }

        //  Update current combatant in action to be used for next turn
        currentCombatantInAction = combatantTurnQueue[0].GetComponent<Combatant>();
        if (currentCombatantInAction.gameObject.tag == "Party Member")
            isPlayerTurn = true;
        else
            isPlayerTurn = false;
    }

    private void EndGame()
    {
        //  player loses
        if (GetNumberOfAlivePartyMembersInPlay() <= 0)
        {
            SceneManager.LoadScene("GameLostScene");
        }
        else
        {
            SceneManager.LoadScene("GameWonScene");
        }
    }

    bool CheckForGameOverCondition()
    {
        if (GetNumberOfAliveBossesInPlay() > 0
            && GetNumberOfAlivePartyMembersInPlay() > 0)
        {
            return false;
        }
        else
        {
            Debug.Log("Game over");
            return true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager.PlayMusic("BattleMusic");

        bossArray = GameObject.FindGameObjectsWithTag("Boss");
        partyMemberArray = GameObject.FindGameObjectsWithTag("Party Member");

        combatantTurnQueue = new List<GameObject>();
        combatantTurnQueue.AddRange(partyMemberArray);
        combatantTurnQueue.AddRange(bossArray);

        currentCombatantInAction = combatantTurnQueue[0].GetComponent<Combatant>();

        InitiateNewTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
