using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int lifeRegenAmount = 30;
    public int gainXP = 30;
    public Button[] itens;
    public int[] valueItens;

    // Update is called once per frame
    void Update()
    {
        Buttons();
    }

    public void Buttons(){
        for(int i = 0; i< itens.Length; i++){
            if(valueItens[i]>Controller.instance.money){
                itens[i].interactable = false;
            }else{
                itens[i].interactable = true;
            }
        }
    }
    public void BuyLife(int index){
        Player.instance.hitPoints += lifeRegenAmount;
        if(Player.instance.hitPoints>=200){
            Player.instance.hitPoints = 200;
        }
        Controller.instance.UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
        Controller.instance.money -= valueItens[index];
    }
    public void BuyXP(int index){
        Player.instance.exp += gainXP;
        Controller.instance.money -= valueItens[index];
    }

}
