using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Combatant character;
    public Slider hpBar, mpBar;
    public Text hpText, mpText;

    public void UpdateCharacterCard()
    {
        UpdateHPBar();
        UpdateMPBar();
    }

    public void UpdateHPBar()
    {
        float newBarValue = character.GetHP() / character.GetMaxHP();
        hpBar.value = newBarValue;
        hpText.text = character.GetHP() + " / " + character.GetMaxHP();
    }

    public void UpdateMPBar()
    {
        float newBarValue = character.GetMP() / character.GetMaxMP();
        mpBar.value = newBarValue;
        mpText.text = character.GetMP() + " / " + character.GetMaxMP();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCharacterCard();
    }
}
