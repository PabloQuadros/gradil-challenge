using GradilChallenge.Application.Orders.ConfirmOrderUseCase;
using GradilChallenge.Application.Orders.GetOrderHistoryUseCase;
using GradilChallenge.Application.Quotes.CalculateQuoteUseCase;
using Microsoft.Extensions.DependencyInjection;

namespace GradilChallenge.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICalculateQuoteUseCase, CalculateQuoteUseCase>();
        services.AddScoped<IConfirmOrderUseCase, ConfirmOrderUseCase>();
        services.AddScoped<IGetOrderHistoryUseCase, GetOrderHistoryUseCase>();

        return services;
    }
}
