using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    public int Hp;

    public CellType CellType;

    public PlayerType PlayerType;

    public List<CardData> CardsList = new List<CardData>();

    public int Index;
}
