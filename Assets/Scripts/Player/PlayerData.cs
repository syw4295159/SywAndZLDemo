using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    public int Hp = 3;

    public CellType CellType;

    public PlayerType PlayerType;

    public List<CardData> CardsList = new List<CardData>();

    public int Index;

    public int Id;

    public int BehaviorIndex = 2;//行为次数
}
