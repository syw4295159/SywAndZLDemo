public class CardData
{
    public CardType CardType { get; private set; }

    public bool CanUse { get; private set; }

    public int BelongId { get; private set; }//所属玩家ID

    public int CardId { get; private set; }

    public CardData(CardType _type,int _id,int _cardId, bool _canUse = true)
    {
        CardType = _type;
        CanUse = _canUse;
        CardId = _cardId;
        BelongId = _id;
    }
}