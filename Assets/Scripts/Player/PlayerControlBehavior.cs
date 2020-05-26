using System.Collections.Generic;

public class PlayerControlBehavior
{
    public CardType CardType { get; set; }
    public int BehaviorPlayerId { get; set; }
    public CellType CellType { get; set; }

    public int DefaultIndex { get; set; }

    public int NextIndex { get; set; }

    //当前破坏的区域
    public List<GameCell> AttackAreas;

    public int ControlIndex = 0;//操作步骤序号
}