﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextSolution.Core.Extensions.EmailSender;
using NextSolution.Core.Extensions.FileStorage;
using NextSolution.Core.Extensions.SmsSender;
using NextSolution.Infrastructure.EmailSender.MailKit;
using NextSolution.Infrastructure.SmsSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextSolution.Infrastructure.FileStorage.Local
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, Action<LocalFileStorageOptions> options)
        {
            services.Configure(options);
            services.AddLocalFileStorage();
            return services;
        }

        public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LocalFileStorageOptions>(configuration);
            services.AddLocalFileStorage();
            return services;
        }

        public static IServiceCollection AddLocalFileStorage(this IServiceCollection services)
        {
            services.AddTransient<IFileStorage, LocalFileStorage>();
            return services;
        }
    }
}
