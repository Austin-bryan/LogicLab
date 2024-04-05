namespace LogicLab.EditorUI;

public class Variant<T1, T2>
{
    private readonly object? value;

    public bool IsType1 { get; }
    public bool IsType2 => !IsType1;

    public Variant(T1 value) => (this.value, IsType1) = (value, true);
    public Variant(T2 value) => (this.value, IsType1) = (value, false);

    public T GetValue<T>()
    {
        if (value == null)
            throw new NullReferenceException("Varient value was null");
        else if (typeof(T) == typeof(T1) && IsType1)
            return (T)value;
        else if (typeof(T) == typeof(T2) && !IsType1)
            return (T)value;
        else throw new InvalidOperationException("Invalid type or variant value.");
    }
}