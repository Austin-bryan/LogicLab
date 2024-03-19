namespace LogicLab;

public readonly struct ELogicGate
{
    // The goal of this class is to be used like an enum. 
    // Even numbers are BUFFER, AND OR, XOR
    // Odd numbers are NOT NAND NOT XNOR
    // Adding a 1 makes a gate negative
    // Subtracting a 1 makes a negative gate positive.
    // This class behaves like a enum, but with extra methods and functional overload, allowing you to do 
    // !ELogicGate.AND which returns ELogicGate.NAND
    private enum LogicGateValue
    {
        Buffer = 0, NOT = 1, AND = 2, OR = 4, XOR = 6,
        NAND = NOT | AND, NOR = NOT | OR, XNOR = NOT | XOR
    }

    public static readonly ELogicGate Buffer = new((int)LogicGateValue.Buffer);
    public static readonly ELogicGate NOT    = new((int)LogicGateValue.NOT);
    public static readonly ELogicGate AND    = new((int)LogicGateValue.AND);
    public static readonly ELogicGate OR     = new((int)LogicGateValue.OR);
    public static readonly ELogicGate XOR    = new((int)LogicGateValue.XOR);
    public static readonly ELogicGate NAND   = new((int)LogicGateValue.NAND);
    public static readonly ELogicGate NOR    = new((int)LogicGateValue.NOR);
    public static readonly ELogicGate XNOR   = new((int)LogicGateValue.XNOR);

    private readonly int value;
    private ELogicGate(int value) => this.value = value;

    public static bool operator ==(ELogicGate lhs, ELogicGate rhs) => lhs.value == rhs.value;
    public static bool operator !=(ELogicGate lhs, ELogicGate rhs) => !(lhs == rhs);    

    public static ELogicGate operator |(ELogicGate lhs, ELogicGate rhs)
    {
        if (lhs != NOT && rhs != NOT)
            throw new InvalidOperationException("One of the logic gates in a | operation must be the NOT gate");

        var other = lhs == NOT ? rhs : lhs;
        return other.value % 2 == 0 ? new(other.value + 1) : new(other.value - 1);
    }
    public static ELogicGate operator !(ELogicGate logicGate) => NOT | logicGate;
    public readonly ELogicGate NegativeGate() => IsNegative()  ? this : !this;
    public readonly ELogicGate PositiveGate() => !IsNegative() ? this : !this;
    public readonly bool IsNegative()         => value % 2 == 1;
    public readonly bool IsSingleInput()      => value <= 1;

    public override readonly bool Equals(object? obj) => obj is ELogicGate other && value == other.value;
    public override readonly string ToString() => ((LogicGateValue)value).ToString();

    public bool ApplyGate(List<bool?> signals) => (LogicGateValue)value switch
    {
        LogicGateValue.Buffer => signals[0] == true,
        LogicGateValue.AND    => signals.All(b => b == true),
        LogicGateValue.OR     => signals.Any(b => b == true),
        LogicGateValue.XOR    => signals.Aggregate((acc, curr) => acc ^ curr) == true,
        LogicGateValue.NOT    => !Buffer.ApplyGate(signals),
        LogicGateValue.NAND   => !AND.ApplyGate(signals),
        LogicGateValue.NOR    => !OR.ApplyGate(signals),
        LogicGateValue.XNOR   => !XOR.ApplyGate(signals),
        _ => throw new InvalidOperationException("Invalid logic gate")
    };

    public override readonly int GetHashCode() => HashCode.Combine(value);
}