using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum EducationLevelTypes
    {
        [Description("Elementary")]
        Elementary = 0,
        [Description("Secondary Level")]
        SecondaryLevel = 1,
        [Description("Vocational Level")]
        VocationalLevel = 2,
        [Description("College/University Graduate")]
        CollegeLevel = 3,
        [Description("Post Graduate")]
        PostGraduateLevel = 4,
        [Description("Non Formal Education")]
        NonFormalEducation = 5
    }
    public enum BusinessPartnerTypes
    {
        All,
        Customer,
        Supplier,
        //IndividualCustomer,
        //IndividualSupplier,
        //SalesPerson
    }
    public enum BusinessPartnerCategory
    {
        Organization,//Store
        Individual
    }
}