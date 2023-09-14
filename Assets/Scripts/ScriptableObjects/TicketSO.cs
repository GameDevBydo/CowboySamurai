using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Ticket", menuName = "Game/Levels/Ticket", order = 0)]
public class TicketSO : ScriptableObject
{
    public GameObject ticketModel;
    public Sprite ticketSprite, HighlightedSprite;
    public string ticketName, ticketDescription, ticketLevel;
    [HideInInspector]
    public enum TicketType{COMBAT, NPC, BOSS};
    public TicketType ticketType;
}
