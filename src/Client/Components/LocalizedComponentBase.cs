using MarksBaseballCards.Client.Services;
using Microsoft.AspNetCore.Components;

namespace MarksBaseballCards.Client.Components;

/// <summary>Refreshes copy without resetting forms, filters or open dialogs.</summary>
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected UiPreferences L { get; set; } = default!;
    protected override void OnInitialized() => L.Changed += RefreshCopy;
    private void RefreshCopy() => _ = InvokeAsync(StateHasChanged);
    public virtual void Dispose() => L.Changed -= RefreshCopy;
}
