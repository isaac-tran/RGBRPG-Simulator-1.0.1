using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetList : MonoBehaviour
{
    public CombatManager combatManager;
    public PlayerChooseActionMenu playerChooseActionMenu;

    public Text commandNameText;
    public ScrollRect scrollView;
    public GameObject content;
    public Button buttonPrefab;
    [SerializeField]private List<GameObject> targetList;
    public bool includeEnemies = true, includeAllies = false;

    private int selectedTargetIndex;

    public void PopulateTargetList()
    {
        ScoutForTargets();
        ShowButtons();
    }

    void ScoutForTargets()
    {
        targetList.Clear();
        GameObject[] newList;

        //  Get list of enemies
        if (includeEnemies)
        {
            newList = combatManager.GetBossArray();

            targetList.AddRange(newList);
        }

        //  Get list of enemies
        if (includeAllies)
        {
            newList = combatManager.GetPartyMemberArray();

            targetList.AddRange(newList);
        }

        //  Remove all KO-ed targets from list
/*        int targetListLength = targetList.Count;
        int i = 0;
        while (i < targetListLength)
        {
            Combatant thisTarget = targetList[i].GetComponent<Combatant>();
            if (thisTarget.GetHP() <= 0)
            {
                targetList.RemoveAt(i);
                targetListLength = targetList.Count;
            }
            else
                i++;
        }*/
    }

    void ShowButtons()
    {
        Button button;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].gameObject.GetComponent<Combatant>().GetHP() > 0)
            {
                button = Instantiate(buttonPrefab, content.transform);
                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = targetList[i].name;
                button.GetComponent<TargetListButton>().SetIndexInTargetList(i);
            }
        }

        foreach (Transform childButton in content.transform)
        {
            childButton.GetComponent<Button>().onClick.AddListener(delegate { SelectTargetAndExecute(childButton.GetComponent<Button>()); });
        }
    }

    void SelectTargetAndExecute(Button clickedButton)
    {
        //  Execute command
        playerChooseActionMenu.SetPlayerFinishedChoosingBool(true);

        //  Save player's choice in combat manager
        combatManager.SetPlayerTargetChoiceIndex(clickedButton.GetComponent<TargetListButton>().GetIndexInTargetList());

        //  Disable this target list
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Awake()
    {
        targetList = new List<GameObject>();
    }

    private void OnEnable()
    {
        playerChooseActionMenu.SetPlayerFinishedChoosingBool(false);
        PopulateTargetList();
    }

    private void OnDisable()
    {
        targetList.Clear();

        foreach (Transform childButton in content.transform)
        {
            Destroy(childButton.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
