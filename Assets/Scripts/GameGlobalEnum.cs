
public enum CellType
{
    Up,
    Down
}
public enum PlayerType
{
    Player,
    Enemy,
}

public enum CardType
{
    Move,
    Attack,
}

public enum MsgEnum
{
    StepOne = 0,
    StepOneOver,
    StepTime,
    StepTimeOver,

    //Behavior
    UseCard,
    ClickCell,
    BehaviorLimit,//当前回合操作上限
}
