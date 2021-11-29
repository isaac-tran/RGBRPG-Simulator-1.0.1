using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    [SerializeField] private Spell spellInfo;
    [SerializeField] private Text spellNameText;
    [SerializeField] private Text spellCostText;

    public Spell GetSpellInfo() { return spellInfo; }

    public void SetSpellInfo(Spell newSpellInfo)
    {
        spellInfo = newSpellInfo;
        spellNameText.text = spellInfo.spellName;
        spellCostText.text = spellInfo.spellCost.ToString();
    }
}
