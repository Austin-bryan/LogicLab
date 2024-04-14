using System.IO;

namespace LogicLab;

// AB
// Purpose: Stores a list that tracks port and signal combinations
// for the purpose of comparing signal paths to try and detect an infinite loop
public class SignalPath
{
    private readonly List<(IOPort port, bool? signal)> path = [];

    public void AddStep(IOPort port, bool? signal) => path.Add((port, signal));
    public bool Equals(SignalPath other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        return path.SequenceEqual(other.path);
    }
    public void AddRange(SignalPath signalPath) => path.AddRange(signalPath.path);

    public static bool operator ==(SignalPath lhs, SignalPath rhs) => lhs.Equals(rhs);
    public static bool operator !=(SignalPath lhs, SignalPath rhs) => lhs != rhs;

    public override bool Equals(object? obj)
    {
        if (obj is SignalPath signalPath)
             return Equals((SignalPath)obj);
        else return false;
    }
    public override int GetHashCode() => path.GetHashCode();
}
