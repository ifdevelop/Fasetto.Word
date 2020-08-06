using Dna;
using Fasetto.Word.Core;
using Fasetto.Word.Relational;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom startup so we load IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs
            base.OnStartup(e);

            // Setup the main application
            await ApplicationSetupAsync();

            // Log it
            IoC.Logger.Log("Application starting...", LogLevel.Debug);

            // Setup the application view model based on if we are logged in
            IoC.Application.GoToPage(
                // If we are logged in...
                await IoC.ClientDataStore.HasCredentialsAsync() ?
                // Go to chat page
                ApplicationPage.Chat :
                // Otherwise, go to login page
                ApplicationPage.Login);

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

        /// <summary>
        /// Configures our application ready for use 
        /// </summary>
        private async Task ApplicationSetupAsync()
        {
            // Setup configuration builder
            var builder = new ConfigurationBuilder();

            // Set the path to the "appsettings.json"
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0]);

            // Get configuration from appsettings.json
            builder.AddJsonFile("appsettings.json");

            // Build the configuration
            var config = builder.Build();

            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .AddClientDataStore()
                .Build();

            // Use the configuration file
            Framework.Construction.UseConfiguration(config);

            // Setup IoC
            IoC.Setup();

            // Bind a Logger
            IoC.Kernel.Bind<ILogFactory>().ToConstant(new BaseLogFactory(new[]
            {
                // TODO: Add ApplicationSettings so we can set/edit a log location
                //       For now just log to the path where this application is running
                new Core.FileLogger("log.txt")
            }));

            IoC.Kernel.Bind<ITaskManager>().ToConstant(new TaskManager());

            // Bind a file manager
            IoC.Kernel.Bind<IFileManager>().ToConstant(new FileManager());

            // Bind a UI Manager
            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            // Ensure the client data store
            await IoC.ClientDataStore.EnsureDataStoreAsync();

            // Load new settings
            //await IoC.Settings.LoadAsync();
            IoC.Task.RunAndForget(IoC.Settings.LoadAsync);
        }
    }
}
