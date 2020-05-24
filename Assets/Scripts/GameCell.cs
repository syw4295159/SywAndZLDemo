using UnityEngine;
using System.Collections;

public class GameCell : MonoBehaviour
{
    private float width = 50;
    private float height = 50;

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
}

