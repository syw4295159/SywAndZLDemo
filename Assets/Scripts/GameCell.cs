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

    public void Init(int _index,CellType _type)
    {
        index = _index;
        CellType = _type;
    }

    internal void SetClickState(bool state)
    {
        GetComponent<Button>().interactable = state;
    }
}

