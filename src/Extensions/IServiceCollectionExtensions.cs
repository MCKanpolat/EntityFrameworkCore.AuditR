using System;
using EntityFrameworkCore.AuditR.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.AuditR.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAuditR<TCurrentUser>(this IServiceCollection services,
        AuditRConfiguration auditRConfiguration)
        where TCurrentUser : class, ICurrentUser
    {
        ArgumentNullException.ThrowIfNull(auditRConfiguration);

        services.AddScoped<ICurrentUser, TCurrentUser>();
        services.AddSingleton(auditRConfiguration);
        return services;
    }
}