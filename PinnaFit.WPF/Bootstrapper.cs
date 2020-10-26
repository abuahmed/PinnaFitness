using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.WPF.ViewModel;
using Microsoft.Practices.Unity;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Repository;
using PinnaFit.Core;

namespace PinnaFit.WPF
{
    public class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            Singleton.SqlceFileName = "PinnaFitDbProd";
            Singleton.SeedDefaults = true;

            Container.RegisterType<IDbContext, PinnaFitDBContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();

            Container.RegisterType<MainViewModel>();
        }
    }
}
