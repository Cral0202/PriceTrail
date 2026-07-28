using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Services;

public partial class NavigationService : ObservableObject
{
    private readonly Stack<ObservableObject> _history = new();

    [ObservableProperty]
    public partial ObservableObject? CurrentViewModel { get; private set; }

    [ObservableProperty]
    public partial ObservableObject? CurrentModalViewModel { get; private set; }

    /****************/
    /* PAGE ROUTING */
    /****************/

    public void NavigateTo(ObservableObject viewModel)
    {
        if (CurrentViewModel != null)
        {
            _history.Push(CurrentViewModel);
        }

        CurrentViewModel = viewModel;
    }

    public void GoBack()
    {
        if (_history.Count > 0)
        {
            CurrentViewModel = _history.Pop();
        }
    }

    public void NavigateAndClearHistory(ObservableObject viewModel)
    {
        _history.Clear();
        CurrentViewModel = viewModel;
    }

    /*****************/
    /* MODAL ROUTING */
    /*****************/

    public void OpenModal(ObservableObject modalViewModel)
    {
        CurrentModalViewModel = modalViewModel;
    }

    public void CloseModal()
    {
        CurrentModalViewModel = null;
    }
}
