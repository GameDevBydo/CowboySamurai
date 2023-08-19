using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int priceLife;
    public int priceXP;
    //public int[] priceTickect;

    public Button life;
    public Button xp;
    //public Button[] ticket;

    public string[] descriptionTicket;

    public int lifeRegenAmount;
    public int gainXP;

    void Start()
    {
        VerifyPurchase();
    }

    public void VerifyPurchase(){
        if(Controller.instance.money<priceLife){
            life.interactable = false;
        }
        if(Controller.instance.money<priceXP){
            xp.interactable = false;
        }
    }

    public void BuyLife(){
        Player.instance.hitPoints += lifeRegenAmount;
        if(Player.instance.hitPoints>=200){
            Player.instance.hitPoints = 200;
        }
        Controller.instance.UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
        Controller.instance.money -= priceLife;
    }
    public void BuyXP(){
        Player.instance.exp += gainXP;
        Controller.instance.money -= priceXP;
    }

    public void BuyTicket(){
        
    }

}
