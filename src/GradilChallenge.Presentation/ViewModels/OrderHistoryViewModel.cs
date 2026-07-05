using GradilChallenge.Application.Orders.GetOrderHistoryUseCase;
using GradilChallenge.Domain.Entities;
using GradilChallenge.Presentation.Models.Drawing;
using GradilChallenge.Presentation.Services;
using System.Collections.ObjectModel;

namespace GradilChallenge.Presentation.ViewModels;

public sealed class OrderHistoryViewModel : ViewModelBase
{
    private readonly IGetOrderHistoryUseCase _getOrderHistoryUseCase;
    private readonly FenceDrawingBuilder _drawingBuilder;
    public ObservableCollection<DrawingShape> FenceDrawing { get; } = new();
    public bool HasSelectedOrder => SelectedOrder != null;

    public ObservableCollection<Order> Orders { get; } = new();
    public bool HasOrders => Orders.Count > 0;

    private Order? _selectedOrder;
    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (SetField(ref _selectedOrder, value))
            {
                OnPropertyChanged(nameof(HasSelectedOrder));
                FenceDrawing.Clear();
                if (value is not null)
                    foreach (var shape in _drawingBuilder.Build(value.Quote))
                        FenceDrawing.Add(shape);
            }
        }
    }

    public OrderHistoryViewModel(IGetOrderHistoryUseCase getOrderHistoryUseCase,
                                 FenceDrawingBuilder drawingBuilder)
    {
        _getOrderHistoryUseCase = getOrderHistoryUseCase;
        _drawingBuilder = drawingBuilder;
    }

    public async Task LoadAsync()
    {
        var orders = await _getOrderHistoryUseCase.ExecuteAsync();

        Orders.Clear();
        foreach (var order in orders)
            Orders.Add(order);

        SelectedOrder = null;
        OnPropertyChanged(nameof(HasOrders));
    }
}
