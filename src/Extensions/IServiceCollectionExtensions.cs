using System;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.AuditR.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditR(this IServiceCollection services, Func<AuditUser> funcCurrentUser, AuditRConfiguration auditRConfiguration)
        {
            if (funcCurrentUser == null)
            {
                throw new ArgumentNullException(nameof(funcCurrentUser));
            }

            if (auditRConfiguration == null)
            {
                throw new ArgumentNullException(nameof(auditRConfiguration));
            }

            services.AddScoped((s) => funcCurrentUser);
            services.AddSingleton(auditRConfiguration);
            return services;
        }
    }
}
