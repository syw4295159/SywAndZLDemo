using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;

    FsmCore fc;
    [Space]
    public GameObject CellSpawner;
    public GameObject GameCellPrefab;
    public int UpCellRow;
    public int UpCellColumn;
    public int DownCellRow;
    public int DownCellColumn;
    private List<GameCell> gameCells = new List<GameCell>();
    [Space]
    public GameObject GamePlayerPrefab;
    private List<GamePlayer> playerlist = new List<GamePlayer>();
    public List<GamePlayer> PlayerList { get => playerlist; }
    [Space]
    private float timer;//决策计时器
    private float overTime = 15;
    private bool isGameDesign = false;
    public Action<float> GameStartAction;


    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        MessageCenter.RegisterMessage((int)MsgEnum.StepOneOver, StepOneOver);
        MessageCenter.RegisterMessage((int)MsgEnum.StepTimeOver, StepTimeOver);
    }
    private void Start()
    {
        fc = new FsmCore();
        gameCells.Clear();
        playerlist.Clear();
        SpawnCell(UpCellRow, UpCellColumn, CellType.Up);
        SpawnCell(DownCellRow, DownCellColumn, CellType.Down);

        StepOne();
        MessageCenter.SendMessage((int)MsgEnum.StepOne);
    }

    private void OnDisable()
    {
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepOneOver, StepOneOver);
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepTimeOver, StepTimeOver);
    }

    private void SpawnCell(int row,int column,CellType type)
    {
        int index = 0;
        var rect = GameCellPrefab.GetComponent<RectTransform>().rect;
        int spe = type == CellType.Up ? 1 : -1;
        int spe2 = type == CellType.Up ? 0 : -100;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var cell = Instantiate(GameCellPrefab, CellSpawner.transform);
                var gamecell = cell.GetComponent<GameCell>() ?? cell.AddComponent<GameCell>();
                index++;
                gamecell.Init(index, type);
                gamecell.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * rect.width, j * rect.height * spe + spe2);
                gameCells.Add(gamecell);
            }
        }
    }

    /// <summary>
    /// 生成人物，发牌阶段
    /// </summary>
    private void StepOne()
    {
        isGameDesign = false;
        SpawnPlayer(4, PlayerType.Player, CellType.Down);
        SpawnPlayer(4, PlayerType.Enemy, CellType.Up);
    }
    private void StepOneOver()
    {
        Debug.Log("StepOneOver");
        MessageCenter.SendMessage((int)MsgEnum.StepTime);
        //TODO
        isGameDesign = true;
    }

    private void StepTimeOver()
    {
        Debug.Log("倒计时结束");
    }

    private void SpawnPlayer(int index,PlayerType type,CellType cellType)
    {
        PlayerData data = new PlayerData();
        data.Hp = 3;
        data.CellType = cellType;
        data.PlayerType = type;
        data.Index = index;
        SpawnCard(data);        
        
        var cell = gameCells.Find(x => x.Index == index && x.CellType == cellType);
        if (cell != null)
        {
            var playerObj = Instantiate(GamePlayerPrefab, cell.transform);
            var player = playerObj.GetComponent<GamePlayer>();
            player.SetGamePlayerData(data);
            playerlist.Add(player);
        }            
    }

    private void SpawnCard(PlayerData data)
    {
        for (int i = 0; i < 6; i++)
        {
            CardData card = new CardData(i < 3 ? CardType.Move : CardType.Attack, true);
            data.CardsList.Add(card);
        }        
    }

    private void FixedUpdate()
    {
        if (isGameDesign)
        {
            timer += Time.deltaTime;
            if (GameStartAction != null)
            {
                GameStartAction(timer);
            }
            if (timer >= overTime)
            {
                timer = 0;
                isGameDesign = false;
            }

        }
        else
        {
            timer = 0;
        }

    }
}
