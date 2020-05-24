public class CardData
{
    private CardType cardType;
    public CardType CardType { get => cardType; }

    private bool canUse;

    public CardData(CardType _type,bool _canUse = true)
    {
        cardType = _type;
        canUse = _canUse;
    }
}