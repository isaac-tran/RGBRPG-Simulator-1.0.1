using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChooseActionMenu : MonoBehaviour
{
    public CombatManager combatManager;
    public TargetList targetList;
    public SpellList spellList;

    public Button attackButton, spellButton, defendButton;
    public Image partyMemberPortrait;
    public Text partyMemberText;

    private bool playerFinishedChoosing = false;

    private void Init()
    {
        //  Add listener to all buttons
        attackButton.onClick.AddListener(OnAttackButtonClick);
        spellButton.onClick.AddListener(OnSpellButtonClick);
        defendButton.onClick.AddListener(OnDefendButtonClick);
    }

    public bool GetPlayerFinishedChoosingBool()
    {
        return playerFinishedChoosing;
    }

    public void SetPlayerFinishedChoosingBool(bool newBool)
    {
        playerFinishedChoosing = newBool;
    }

    void OnAttackButtonClick()
    {
        spellList.gameObject.SetActive(false);
        targetList.gameObject.SetActive(false);

        combatManager.SetPlayerChoicesIndices(0, 1);

        targetList.includeAllies = false;
        targetList.includeEnemies = true;
        targetList.commandNameText.text = "Attack";

        targetList.gameObject.SetActive(true);
    }

    void OnSpellButtonClick()
    {
        combatManager.SetPlayerChoicesIndices(0, 2);

        spellList.gameObject.SetActive(true);
    }

    void OnDefendButtonClick()
    {
        combatManager.SetPlayerChoicesIndices(0, 3);

        playerFinishedChoosing = true;
    }

    private void OnEnable()
    {
        GameObject currentPartyMember = combatManager.GetCurrentCombatantInAction();

        switch (currentPartyMember.name)
        {
            case "Red":
                partyMemberPortrait.sprite = Resources.Load<Sprite>("UI/Portraits/Page 1");
                break;

            case "Green":
                partyMemberPortrait.sprite = Resources.Load<Sprite>("UI/Portraits/Page 2");
                break;

            case "Blue":
                partyMemberPortrait.sprite = Resources.Load<Sprite>("UI/Portraits/Page 3");
                break;
        }

        partyMemberText.text = currentPartyMember.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
