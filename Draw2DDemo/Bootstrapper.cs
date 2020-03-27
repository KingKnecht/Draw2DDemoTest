using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Draw2DDemo.ViewModels;
using IContainer = Autofac.IContainer;

namespace Draw2DDemo
{

    public class AutofacCaliburnBootStrapper : BootstrapperBase
    {
        #region Fields
        private readonly ILog _logger = LogManager.GetLog(typeof(AutofacCaliburnBootStrapper));
        private IContainer _container;
        #endregion

        public AutofacCaliburnBootStrapper()
        {
            Initialize();
        }

        #region Properties
        protected IContainer Container
        {
            get { return _container; }
        }
        #endregion

        #region Overrides
        protected override void Configure()
        { //  configure container
            var builder = new ContainerBuilder();

            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              //  must be a type that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))
              //  must be in a namespace ending with ViewModels
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("ViewModels"))
              //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
              .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
              //  registered as self
              .AsSelf()
              //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              //  must be a type that ends with View
              .Where(type => type.Name.EndsWith("View"))
              //  must be in a namespace that ends in Views
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("Views"))
              //  registered as self
              .AsSelf()
              //  always create a new one
              .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            ConfigureContainer(builder);
            
            _container = builder.Build();


            var defaultLocator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                var viewType = defaultLocator(modelType, displayLocation, context);
                while (viewType == null && modelType != typeof(object))
                {
                    modelType = modelType.BaseType;
                    viewType = defaultLocator(modelType, displayLocation, context);
                }
                return viewType;
            };
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.IsRegistered(serviceType))
                    return Container.Resolve(serviceType);
            }
            else
            {
                if (Container.IsRegistered( serviceType))
                    return Container.Resolve(serviceType);
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? serviceType.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }
        #endregion

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
