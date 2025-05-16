using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class SummlingManagerUI : MonoBehaviour
{
    public GameObject summlingUI;
    public GameObject page1;
    public GameObject page2;

    public GameObject summonConfirmationTab;

    public SummlingManager manager;

    public TMP_Text summonCost;

    public void Awake()
    {
        float cost = manager.GetSummonCost();
        summonCost.text = cost.ToString();
    }

    public void SummonButton()
    {
        if (manager.canSummon)
        {
           Debug.Log("OPening confirmation page");
           summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
           manager.GenerateSummling();

        }
        
    }

    public void ConfirmSummon()
    {
        //Apply effects and close YesNo, take away SE
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        manager.ConfirmSummon();
    }
    public void CancelSummon()
    {
        //Close YesNo, take away SE
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        manager.DeclineSummon();
    }

    public void SelectSummling()
    {
        //Get Summling Data
    }

    public void TransmuteButton()
    {
        //Gain SE delete effects and delete Summling
        //transmute selected true
    }

    public void MergeButton()
    {
        //Combine both Summlings
        //Merge selected true
    }

    public void ConfirmButtion()
    {
        //Apply merge or transmute
    }
    public void HoverIcon()
    {
        //Show description of Summling
    }

    public void HoverTransmute()
    {
        //Explain how to transmute
    }

    public void HoverMerge()
    {
        //Explain how to merge
    }

    public void ChangePage()
    {
        page1.SetActive(!page1.activeInHierarchy);
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        page2.SetActive(!page2.activeInHierarchy);
    }

}
