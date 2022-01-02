using FileEncryptor.WPF.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace FileEncryptor.WPF.Services
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
           .AddTransient<IUserDialog, UserDialog>()
           .AddTransient<IEncryptor, Rfc2898Encryptor>()
        ;
    }
}