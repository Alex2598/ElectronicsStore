﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;

namespace Store.Data.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<StoreDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                }
               // ServiceLifetime.Transient
            );
            services.AddScoped<Dictionary<Type, StoreDbContext>>();
            services.AddSingleton<DbContextFactory>();
            services.AddSingleton<IComponentRepository, ComponentRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
