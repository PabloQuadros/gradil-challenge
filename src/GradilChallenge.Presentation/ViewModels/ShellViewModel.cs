namespace GradilChallenge.Presentation.ViewModels;

public sealed class ShellViewModel : ViewModelBase
{
    private readonly QuoteViewModel _quoteViewModel;
    private readonly OrderHistoryViewModel _orderHistoryViewModel;

    private object _currentViewModel;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (SetField(ref _currentViewModel, value))
                OnPropertyChanged(nameof(IsHistoryVisible));
        }
    }

    public bool IsHistoryVisible => CurrentViewModel is OrderHistoryViewModel;

    public RelayCommand ShowHistoryCommand { get; }
    public RelayCommand ShowNewQuoteCommand { get; }

    public ShellViewModel(QuoteViewModel quoteViewModel, OrderHistoryViewModel orderHistoryViewModel)
    {
        _quoteViewModel = quoteViewModel;
        _orderHistoryViewModel = orderHistoryViewModel;

        _currentViewModel = _quoteViewModel;

        ShowHistoryCommand = new RelayCommand(async _ => await ShowHistoryAsync());
        ShowNewQuoteCommand = new RelayCommand(_ => ShowNewQuote());
    }

    private async Task ShowHistoryAsync()
    {
        await _orderHistoryViewModel.LoadAsync();
        CurrentViewModel = _orderHistoryViewModel;
    }

    private void ShowNewQuote()
    {
        CurrentViewModel = _quoteViewModel;
    }
}
