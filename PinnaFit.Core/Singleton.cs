using System.Collections.Generic;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Core
{
    public class Singleton
    {
        private static Singleton _instance;
        private Singleton() { }

        public static Singleton Instance
        {
            get { return _instance ?? (_instance = new Singleton()); }
        }

        public static ProductActivationDTO ProductActivation { get; set; }

        public static UserDTO User { get; set; }

        public static PinnaFitEdition Edition { get; set; }

        public static string SqlceFileName { get; set; }

        public static bool SeedDefaults { get; set; }

        public static UserRolesModel UserRoles { get; set; }

        //public static SettingDTO Setting { get; set; }
        public static IList<WarehouseDTO> WarehousesList { get; set; }

        public static string ConnectionStringName { get; set; }
        public static string ProviderName { get; set; }
    }
}
