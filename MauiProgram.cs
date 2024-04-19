using Microsoft.Extensions.Logging;
using EthicsQA.Data;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Radzen;


namespace EthicsQA;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<WeatherForecastService>();
		builder.Services.AddRadzenComponents();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton<AuthenticationStateProvider, ExternalAuthStateProvider>();
		builder.Services.AddSingleton<EthicsQA.Services.EthicsAPIClient>();
        builder.Services.AddAuthorizationCore();


        return builder.Build();


    }
}
