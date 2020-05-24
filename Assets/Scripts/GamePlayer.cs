using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : Player
{
    public GamePlayer(PlayerData _dat)
    {
        data = _dat;
    }

    public void SetGamePlayerData(PlayerData _dat)
    {
        data = _dat;
    }
    public PlayerData Data
    {
        get { return data; }
        private set { }
    }
}
