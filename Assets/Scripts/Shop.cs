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

    //public string[] descriptionTicket;

    public int lifeRegenAmount;
    public int gainXP;

    public static Shop instance;

    void Start()
    {
        instance = this;
    }

    public void BuyLife(){
        if(Controller.instance.money>priceLife){
            Player.instance.hitPoints += lifeRegenAmount;
            if(Player.instance.hitPoints>=200){
                Player.instance.hitPoints = 200;
            }
            Controller.instance.UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
            Controller.instance.money -= priceLife;
        }
    }
    public void BuyXP(){
        if(Controller.instance.money>priceXP){
            Player.instance.exp += gainXP;
            Controller.instance.money -= priceXP;
        }
    }

    public void BuyTicket(){
        
    }
}
