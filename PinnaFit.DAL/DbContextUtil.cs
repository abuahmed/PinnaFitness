
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.DAL.Interfaces;
using PinnaFit.DAL.Mappings;

namespace PinnaFit.DAL
{
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class SqlDefaultValueAttribute : Attribute
    //{
    //    public string DefaultValue { get; set; }
    //}

    public static class DbContextUtil
    {
        public static DbModelBuilder OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductActivationMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AttachmentMap());
            modelBuilder.Configurations.Add(new CategoryMap());

            modelBuilder.Configurations.Add(new ContactPersonMap());
            modelBuilder.Configurations.Add(new SubscriptionMap());
            modelBuilder.Configurations.Add(new ServiceMap());
            modelBuilder.Configurations.Add(new FacilityMap());
            modelBuilder.Configurations.Add(new FacilityServiceMap());
            modelBuilder.Configurations.Add(new FacilitySubscriptionMap());
            modelBuilder.Configurations.Add(new MemberSubscriptionMap());
            modelBuilder.Configurations.Add(new MemberAssessmentMap());
            modelBuilder.Configurations.Add(new MemberAttendanceMap());
            modelBuilder.Configurations.Add(new AttendanceServiceMap());
            modelBuilder.Configurations.Add(new PervisitSubscriptionMap());

            modelBuilder.Configurations.Add(new MemberMap());

            modelBuilder.Configurations.Add(new TrainerMap());
            modelBuilder.Configurations.Add(new TimeTableMap());
            modelBuilder.Configurations.Add(new TrainerTimeTableMap());
            modelBuilder.Configurations.Add(new TrainerCourseMap());

            modelBuilder.Configurations.Add(new EquipmentMap());
            
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new MembershipMap());
            modelBuilder.Configurations.Add(new RoleMap());

            modelBuilder.Configurations.Add(new WarehouseMap());
            modelBuilder.Configurations.Add(new ItemMap());
            modelBuilder.Configurations.Add(new ItemQuantityMap());
            modelBuilder.Configurations.Add(new TransactionHeaderMap());
            modelBuilder.Configurations.Add(new TransactionLineMap());
            modelBuilder.Configurations.Add(new BusinessPartnerMap());
            modelBuilder.Configurations.Add(new PaymentMap());
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Conventions.Add(new AttributeToColumnAnnotationConvention<SqlDefaultValueAttribute, string>("SqlDefaultValue", (p, attributes) => attributes.Single().DefaultValue));

            return modelBuilder;
        }

        public static IDbContext Seed(IDbContext context)
        {
            //#region Setting Seeds
            //context.Set<SettingDTO>().Add(new SettingDTO()
            //{
            //    Id = 1,
            //    CheckCreditLimit = false,
            //    HandleBankTransaction = false,
            //    TaxType = TaxTypes.NoTax,
            //    TaxPercent = 0
            //});
            //#endregion

            //#region Business Partner Seeds
            //if (Singleton.Edition != PinnaFitEdition.OnlineEdition)
            //{
            //    #region Customer
            //    var bp = new BusinessPartnerDTO()
            //    {
            //        Id = 1,
            //        BusinessPartnerType = BusinessPartnerTypes.Customer,
            //        Code = "C0001",
            //        DisplayName = "_Walking Customer",
            //        CreditLimit = 0,
            //        MaxNoCreditTransactions = 0,
            //        PaymentTerm = 0,
            //        AllowCreditsWithoutCheck = false,
            //    };

            //    context.Set<BusinessPartnerAddressDTO>().Add(new BusinessPartnerAddressDTO
            //     {
            //         Address = CommonUtility.GetDefaultAddress(),
            //         BusinessPartner = bp
            //     });

            //    context.Set<BusinessPartnerContactDTO>().Add(new BusinessPartnerContactDTO
            //    {
            //        Contact = new ContactDTO()
            //        {
            //            FullName = "ibra yas",
            //            Address = CommonUtility.GetDefaultAddress(),
            //        },
            //        BusinessPartner = bp
            //    });
            //    #endregion

            //    #region Supplier

            //    var sbp = new BusinessPartnerDTO()
            //    {
            //        BusinessPartnerType = BusinessPartnerTypes.Supplier,
            //        Code = "S0001",
            //        DisplayName = "_Common Supplier",
            //        CreditLimit = 0,
            //        MaxNoCreditTransactions = 0,
            //        PaymentTerm = 0,
            //        AllowCreditsWithoutCheck = true,
            //    };

            //    context.Set<BusinessPartnerAddressDTO>().Add(new BusinessPartnerAddressDTO
            //    {
            //        Address = CommonUtility.GetDefaultAddress(),
            //        BusinessPartner = sbp

            //    });

            //    context.Set<BusinessPartnerContactDTO>().Add(new BusinessPartnerContactDTO
            //    {
            //        Contact = new ContactDTO()
            //        {
            //            FullName = "ibra yas",
            //            Address = CommonUtility.GetDefaultAddress(),
            //        },
            //        BusinessPartner = sbp
            //    });
            //    #endregion
            //}

            //#endregion

            //#region List Seeds
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "No Category" });

            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitMeasure, DisplayName = "Pcs" });
            //#endregion

            //#region Bank Seeds
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Commercial Bank of Ethiopia" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Dashen Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Awash Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "United Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Nib Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Buna International Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Wegagen Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Abyssinia Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Abay Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Birhan Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Enat Bank" });
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Bank, DisplayName = "Lion Bank" });
            //#endregion

            return context;
        }

        public static IDbContext GetDbContextInstance()
        {
            switch (Singleton.Edition)
            {
                case PinnaFitEdition.CompactEdition:
                    return new DbContextFactory().Create();
                case PinnaFitEdition.ServerEdition:
                    return new DbContextFactory().Create();
                //case PinnaFitEdition.OnlineEdition:
                //    return new ServerDbContextFactory().Create();
            }
            return new DbContextFactory().Create();
        }
    }
}