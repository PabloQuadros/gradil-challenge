
using GradilChallenge.Application.Orders.ConfirmOrderUseCase;
using GradilChallenge.Application.Quotes.CalculateQuoteUseCase;
using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Presentation.Models.Drawing;
using GradilChallenge.Presentation.Models;
using GradilChallenge.Presentation.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GradilChallenge.Presentation.ViewModels;

public sealed class QuoteViewModel : ViewModelBase
{
    private readonly ICalculateQuoteUseCase _calculateQuoteUseCase;
    private readonly IConfirmOrderUseCase _confirmOrderUseCase;
    private readonly FenceDrawingBuilder _drawingBuilder;

    public IReadOnlyList<FenceHeight> AvailableHeights { get; } = FenceHeight.List().ToList();
    public IReadOnlyList<FenceColor> AvailableColors { get; } = FenceColor.List().ToList();
    public ObservableCollection<DrawingShape> FenceDrawing { get; } = new();

    // Input Properties
    private string _desiredLengthText = "";
    public string DesiredLengthText
    {
        get => _desiredLengthText;
        set => SetField(ref _desiredLengthText, value);
    }

    private FenceHeight _selectedHeight;
    public FenceHeight SelectedHeight
    {
        get => _selectedHeight;
        set => SetField(ref _selectedHeight, value);
    }

    private FenceColor _selectedColor;
    public FenceColor SelectedColor
    {
        get => _selectedColor;
        set => SetField(ref _selectedColor, value);
    }

    // Quote State Properties
    private Quote? _quote;
    public Quote? Quote
    {
        get => _quote;
        private set
        {
            if (SetField(ref _quote, value))
                OnPropertyChanged(nameof(HasQuote));
        }
    }
    public bool HasQuote => Quote != null;

    private IReadOnlyList<ComponentQuantity> _componentsTable = Array.Empty<ComponentQuantity>();
    public IReadOnlyList<ComponentQuantity> ComponentsTable
    {
        get => _componentsTable;
        private set => SetField(ref _componentsTable, value);
    }

    // Error Notification Properties
    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetField(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Commands
    public AsyncRelayCommand CalculateCommand { get; }
    public AsyncRelayCommand ConfirmCommand { get; }

    public QuoteViewModel(ICalculateQuoteUseCase calculateQuoteUseCase, IConfirmOrderUseCase confirmOrderUseCase, FenceDrawingBuilder drawingBuilder)
    {
        _calculateQuoteUseCase = calculateQuoteUseCase;
        _confirmOrderUseCase = confirmOrderUseCase;
        _drawingBuilder = drawingBuilder;

        _selectedHeight = AvailableHeights.First();
        _selectedColor = AvailableColors.First();

        CalculateCommand = new AsyncRelayCommand(CalculateAsync, OnCommandException);
        ConfirmCommand = new AsyncRelayCommand(ConfirmAsync, OnCommandException, () => HasQuote);
    }

    private void OnCommandException(Exception ex)
    => ErrorMessage = "Ocorreu um erro inesperado. Detalhes: " + ex.Message;

    private async Task CalculateAsync()
    {
        ResetQuoteState();

        if (!TryParseLength(DesiredLengthText, out double desiredLengthInMeters))
        {
            ErrorMessage = "Informe um comprimento numérico válido.";
            ConfirmCommand.RaiseCanExecuteChanged();
            return;
        }

        var result = await _calculateQuoteUseCase.ExecuteAsync(
            desiredLengthInMeters, SelectedHeight.Id, SelectedColor.Id);

        if (result.IsSuccess)
        {
            Quote = result.Value;
            BuildComponentsTable(result.Value);
            RenderFence(result.Value);
        }
        else
        {
            ErrorMessage = result.ErrorMessage ?? "Erro desconhecido.";
        }

        ConfirmCommand.RaiseCanExecuteChanged();
    }

    private void BuildComponentsTable(Quote quote)
    {
        ComponentsTable = new List<ComponentQuantity>
            {
                new() { Name = "Poste", Quantity = quote.PostCount },
                new() { Name = "Painel", Quantity = quote.PanelCount },
                new() { Name = "Fixador", Quantity = quote.FastenerCount },
                new() { Name = "Parafuso", Quantity = quote.ScrewCount },
            };
    }

    private void RenderFence(Quote quote)
    {
        FenceDrawing.Clear();
        foreach (var shape in _drawingBuilder.Build(quote))
            FenceDrawing.Add(shape);
    }

    private async Task ConfirmAsync()
    {
        if (Quote is null) return;

        var result = await _confirmOrderUseCase.ExecuteAsync(Quote);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Erro ao confirmar o pedido.";
            return;
        }

        ResetQuoteState();
        DesiredLengthText = "";
        ConfirmCommand.RaiseCanExecuteChanged();
    }

    private void ResetQuoteState()
    {
        ErrorMessage = "";
        Quote = null;
        ComponentsTable = Array.Empty<ComponentQuantity>();
        FenceDrawing.Clear();
    }

    private bool TryParseLength(string text, out double result)
    {
        var sanitizedText = text.Replace(',', '.');
        return double.TryParse(sanitizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }
}