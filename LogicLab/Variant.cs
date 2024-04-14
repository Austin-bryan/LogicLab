namespace LogicLab;

// Austin
//  
/*
 * This was an attempt to mirror the behaviour of std::variant from C++. 
 * Basically, Creation Menu contains either folders or items. Folders contain more items. 
 * Each folder can be open or closed, so that gets represented with a bool in an array.
 * However, if that folder has subfolders, then an array of bools needs to be stored in that spot, instead of a bool
 * This allows that behaviour.
 * 
 * The result is:
 * List<bool>
 *      bool
 *      bool
 * bool
 * bool
 * 
 * While there were other solutions that could have been taken, this was the most fun. 
 */
public class Variant<T1, T2>
{
    private readonly object? value;     // One object is used, to avoid wasting memory. This will get casted to the proper type later

    // Track what the type is
    public bool IsType1 { get; }
    public bool IsType2 => !IsType1;

    public Variant(T1 value) => (this.value, IsType1) = (value, true);
    public Variant(T2 value) => (this.value, IsType1) = (value, false);

    // Cast the object based on what type is expected. 
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