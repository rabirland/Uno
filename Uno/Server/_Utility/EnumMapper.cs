using Uno.Shared;

namespace Uno.Server;

public static class EnumMapper
{
    public static TNew Map<TOld, TNew>(TOld old)
        where TNew : unmanaged, Enum
        where TOld : unmanaged, Enum
    {
        return Enum.Parse<TNew>(old.ToString());
    }

    /// <summary>
    /// Checks two enums and ensures that both has the same values, and each value (by name) has a corresponding value in the other
    /// and each value's numerical value matches the other enum's value's numerical value.
    /// </summary>
    /// <typeparam name="TEnum1">The first enum type.</typeparam>
    /// <typeparam name="TEnum2">The second enum type.</typeparam>
    public static void EnsureValueMatching<TEnum1, TEnum2>()
        where TEnum1 : unmanaged, Enum
        where TEnum2 : unmanaged, Enum
    {
        var values1 = Enum.GetValues<TEnum1>();
        var values2 = Enum.GetValues<TEnum2>();

        if (Enum.GetUnderlyingType(typeof(TEnum1)) != typeof(int))
        {
            throw new Exception($"Unsupported enum: {typeof(TEnum1).Name}, must have {typeof(int).Name} underlying type");
        }

        if (Enum.GetUnderlyingType(typeof(TEnum2)) != typeof(int))
        {
            throw new Exception($"Unsupported enum: {typeof(TEnum2).Name}, must have {typeof(int).Name} underlying type");
        }

        if (values1.Length != values2.Length)
        {
            throw new Exception("Invalid enum pair, length mismatch");
        }

        foreach (var value in values1)
        {
            var pair = values2.FirstOrNull(v => v.ToString() == value.ToString());

            if (pair == null)
            {
                throw new Exception($"Invalid enum pair, {typeof(TEnum2).Name} is missing the entry {value}");
            }

            var intValue = (int)(object)value;
            var pairInt = (int)(object)pair.Value;

            if (intValue != pairInt)
            {
                throw new Exception($"Invalid enum pair, {typeof(TEnum1).Name}.{value} numeric value ({intValue}) does not equals {typeof(TEnum2).Name}.{pair.Value} numerical value ({pairInt})");
            }
        }
    }

    // Optimized variants
    public static class CardColor
    {
        public static GameMessages.CardColor ToGameMessageResponse(UnoGame.CardColor color)
        {
            return (GameMessages.CardColor)(int)(color);
        }

        public static UnoGame.CardColor ToUno(GameMessages.CardColor type)
        {
            return (UnoGame.CardColor)(int)(type);
        }
    }

    public static class CardType
    {
        public static GameMessages.CardType ToGameMessageResponse(UnoGame.CardType type)
        {
            return (GameMessages.CardType)(int)(type);
        }

        public static UnoGame.CardType ToUno(GameMessages.CardType type)
        {
            return (UnoGame.CardType)(int)(type);
        }
    }

    public static class RoundPhase
    {
        public static GameMessages.RoundPhase ToGameMessageResponse(UnoGame.RoundPhase type)
        {
            return (GameMessages.RoundPhase)(int)(type);
        }

        public static UnoGame.RoundPhase ToUno(GameMessages.RoundPhase type)
        {
            return (UnoGame.RoundPhase)(int)(type);
        }
    }
}
