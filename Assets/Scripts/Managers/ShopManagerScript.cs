using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManagerScript : MonoBehaviour
{
    public CurrencyBalanceData cbd;
    public int[,] shopItems = new int [6,6];
    public Text currencyTxt;
    private float currencyBalance;
    public CurrencyManager cm;

    void Start()
    {
        //currencyBalance = cbd.StartCurrency;
        currencyBalance = cm.Currency;
        currencyTxt.text = "Currency: $"+currencyBalance.ToString();
        
        //Sets ItemIDs in the shop
        shopItems[1,1] = 1; //Pawn
        shopItems[1,2] = 2; //Knight
        shopItems[1,3] = 3; //Bishop
        shopItems[1,4] = 4; //Rook
        shopItems[1,5] = 5; //Queen
        
        //Extracts item prices to display in the shop
        shopItems[2,1] = cbd.PawnCost;
        shopItems[2,2] = cbd.KnightCost;
        shopItems[2,3] = cbd.BishopCost;
        shopItems[2,4] = cbd.RookCost;
        shopItems[2,5] = cbd.QueenCost;     
    }
    
    //Enables purchase of new units and updating of currency balance
    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        
        if(currencyBalance >= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID])
        {
            currencyBalance -= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID];
            currencyTxt.text = "Currency: $"+currencyBalance.ToString();
            
            //TO BE DONE
            //add insertion of selected game piece on the board
            //add updating of CurrencyManager Currency
            //add exiting from Shop Menu (with or without changes)
        }
    }
}
