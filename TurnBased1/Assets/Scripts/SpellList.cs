using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellList : MonoBehaviour
{
    [SerializeField] public List<Spell> spellGlossary;

    public CombatManager combatManager;
    public PlayerChooseActionMenu playerChooseActionMenu;
    public TargetList targetList;

    public ScrollRect scrollView;
    public GameObject content;
    public Button buttonPrefab;
    [SerializeField] private List<Spell> spellList;

    public void PopulateSpellList()
    {
        GetSpellListFromCurrentCombatant();
        ShowButtons();
    }

    void GetSpellListFromCurrentCombatant()
    {
        Combatant currentCombatant = combatManager.GetCurrentCombatantInAction().GetComponent<Combatant>();
        List<int> listOfSpellIndices = currentCombatant.availableSpells;

        spellList.Clear();
        for (int i = 0; i < listOfSpellIndices.Count; i++)
        {
            spellList.Add(spellGlossary[listOfSpellIndices[i]]);
        }
    }

    void ShowButtons()
    {
        Button button;
        float currentCombatantMP = combatManager.GetCurrentCombatantInAction().GetComponent<Combatant>().GetMP();

        for (int i = 0; i < spellList.Count; i++)
        {
            button = Instantiate(buttonPrefab, content.transform);
            button.GetComponent<SpellButton>().SetSpellInfo(spellList[i]);

            //  Check if the user has enough mp to cast that spell
            if (currentCombatantMP < spellList[i].spellCost)
                button.interactable = false;
        }

        foreach (Transform childButton in content.transform)
        {
            //  Add listener to each button spell
            childButton.GetComponent<Button>().onClick.AddListener(delegate { SelectTargetAndExecute(childButton.GetComponent<Button>()); });  
        }
    }

    void SelectTargetAndExecute(Button clickedButton)
    {
        //  Which spell was selected?
        Spell clickedButtonSpell = clickedButton.GetComponent<SpellButton>().GetSpellInfo();
        string clickedButtonSpellName = clickedButtonSpell.spellName;
        int foundIndex = 0;
        for (int i = 0; i < spellGlossary.Count; i++)
        {
            if (spellGlossary[i].spellName.Equals(clickedButtonSpellName))
            {
                foundIndex = i;
                break;
            }
        }

        //  Save player's choice in combat manager
        combatManager.SetPlayerChoicesIndices(1, foundIndex);

        //  Show target list
        targetList.gameObject.SetActive(false);
        targetList.includeEnemies = clickedButtonSpell.affectEnemies;
        targetList.includeAllies = clickedButtonSpell.affectAllies;
        targetList.gameObject.SetActive(true);
        targetList.commandNameText.text = clickedButtonSpellName;
    }

    // Start is called before the first frame update
    void Awake()
    {
        spellList = new List<Spell>();
    }

    private void OnEnable()
    {
        playerChooseActionMenu.SetPlayerFinishedChoosingBool(false);
        PopulateSpellList();
    }

    private void OnDisable()
    {
        spellList.Clear();

        foreach (Transform childButton in content.transform)
        {
            Destroy(childButton.gameObject);
        }
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
