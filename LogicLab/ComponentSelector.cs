using System.Collections.Immutable;
using System.Windows;

namespace LogicLab;

public static class ComponentSelector
{
    private static readonly List<LogicComponent> selectedComponents = [];
    public static ImmutableList<LogicComponent> SelectedComponents => selectedComponents.ToImmutableList();

    public static void SingleSelect(LogicComponent logicComponent)
    {
        DeselectAll(logicComponent);
        selectedComponents.Add(logicComponent);
    }
    public static void Deselect(LogicComponent logicComponent)
    {
        selectedComponents.Remove(logicComponent);
    }
    public static bool IsSelected(LogicComponent logicComponent) => selectedComponents.Contains(logicComponent);
    public static void DeselectAll(LogicComponent? ignore = null)
    {
        for (int i = selectedComponents.Count - 1; i >= 0; i--)
            if (selectedComponents[i] != ignore)
                selectedComponents[i].Deselect();
    }

    public static void ShiftSelect(LogicComponent logicComponent)
    {
        selectedComponents.Add(logicComponent);
    }
}
