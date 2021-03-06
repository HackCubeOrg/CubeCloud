﻿using System;
using System.Linq;
using Caliburn.Micro;
using Haris.Core.Infrastructure;
using Haris.Core.Modules;
using Haris.Core.Modules.Endpoint;
using Haris.Core.Modules.IntentRecognition.Core;
using Haris.Core.Services;
using Haris.Core.Services.Logging;
using Haris.Core.Services.Luis;
using Haris.Core.Services.Luis.Impl;
using Haris.DataModel;
using Haris.DataModel.Repositories;
using Haris.DataModel.Repositories.Implementation;

namespace Haris.Core
{
    public class AppCoreBootstrapper
    {
        public static SimpleInjector.Container Container { get; private set; }

        public void Run()
        {
            Container = new SimpleInjector.Container();
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            ConfigureContainer();
            InitializeMappings();
            RunInitializers();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Logger.LogError("Unhandled exception: {0}\n{1}", exception?.Message, exception?.StackTrace);
            Shutdown();
        }

        private void InitializeMappings()
        {
        }

        private void ConfigureContainer()
        {
            Container.RegisterSingleton<IEventAggregator>(new EventAggregator {PublicationThreadMarshaller = QueueAsync});
            Container.RegisterSingleton<ILuisUrlProvider, LuisUrlProvider>();
            Container.RegisterSingleton<ILuisClient, LuisClient>();
            Container.RegisterSingleton<IIntentRecognizer, LuisIntentRecognizer>();
            Container.RegisterSingleton<ILuisResponseParser, LuisResponseParser>();
            Container.RegisterSingleton<ILuisIntentToActionMappingRepository, LuisIntentToActionMappingRepository>();
            Container.RegisterSingleton<IIntentToActionConversionService, IntentToActionConversionService>();
            Container.RegisterSingleton<ICubeRepository, CubeRepository>();
            Container.RegisterSingleton<HarisDbContext>();
            Container.RegisterSingleton<EngineService>();

            var types =
                GetType()
                    .Assembly.GetTypes()
                    .Where(RegistrationPredicate)
                    .ToList();
            Container.RegisterCollection<IHarisModule>(types);
        }

        private bool RegistrationPredicate(Type t)
        {
            return t.IsAbstract == false 
                && t.GetCustomAttributes(false).All(a => a is DisableModuleAttribute == false)
                && t.IsClass && t.GetInterfaces().Any(i => i == typeof (IHarisModule));
        }

        private void QueueAsync(Action action)
        {
            AsyncActionsQueue.Enqueue(action);
        }

        private void RunInitializers()
        {
            foreach (var module in Container.GetAllInstances<IHarisModule>())
            {
                module.Init();
            }
        }

        public void Shutdown()
        {
            if (Container != null)
            {
                foreach (var module in Container.GetAllInstances<IHarisModule>())
                {
                    module.Dispose();
                }
                Container.Dispose();
            }
        }

    }
}