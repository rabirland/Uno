namespace UnoGame;

public class Player
{
    private readonly List<Card> cards = new List<Card>();

    public Player(string name)
    {
        this.Name = name;
    }

    public string Name { get; }

    public void AddCard(Card card)
    {
        var index = this.cards.FindIndex(c => c.Is(card));

        if (index < 0)
        {
            cards.Add(card);
        }
    }

    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
    }
}
