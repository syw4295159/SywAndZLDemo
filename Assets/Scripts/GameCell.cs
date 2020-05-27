using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameCell : MonoBehaviour
{
    private float width = 50;
    private float height = 50;

    public int row;
    public int column;

    [SerializeField]
    private int index;
    public int Index { get => index; }

    [SerializeField]
    private CellType cellType;
    public CellType CellType { get => cellType; set => cellType = value; }

    public bool CanClick { get; private set; }

    public void Init(int _index,CellType _type)
    {
        index = _index;
        CellType = _type;
        GetComponentInChildren<Text>().text = (_type == CellType.Up ? "↑" : "↓") + index.ToString();
    }

    internal void SetClickState(bool state)
    {
        CanClick = state;
        GetComponent<Button>().interactable = state;
    }

    public void OnClick()
    {
        MessageCenter<int, int, CellType>.SendMessage((int)MsgEnum.ClickCell, 0, index, cellType);
        Debug.LogFormat("玩家{0},点击了{1},类型{2}", 0, index, cellType);
    }
}

