using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The Ioc Container for the application
    /// </summary>
    public static class IoC
    {
        #region Public Propeerties

        /// <summary>
        /// The kernel foe IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => IoC.Get<IUIManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ILogFactory"/>
        /// </summary>
        public static ILogFactory Logger => IoC.Get<ILogFactory>();

        /// <summary>
        /// A shortcut to access the <see cref="IFileManager"/>
        /// </summary>
        public static IFileManager File => IoC.Get<IFileManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ITaskManager"/>
        /// </summary>
        public static ITaskManager Task => IoC.Get<ITaskManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => IoC.Get<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="SettingsViewModel"/>
        /// </summary>
        public static SettingsViewModel Settings => IoC.Get<SettingsViewModel>();

        #endregion

        #region Construction

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as application starts up to ensure all
        ///       service can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();
        }

        /// <summary>
        /// Bind all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of Application View Model
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());

            // Bind to a single instance of Settings View Model
            Kernel.Bind<SettingsViewModel>().ToConstant(new SettingsViewModel());
        }

        #endregion

        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T"> The type to get </typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

    }
}
