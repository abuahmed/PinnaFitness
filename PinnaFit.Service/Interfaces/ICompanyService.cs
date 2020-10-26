using System;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface ICompanyService : IDisposable
    {
        CompanyDTO GetCompany();
        string InsertOrUpdate(CompanyDTO client);
        //string Disable(CompanyDTODTO client);
        //int Delete(string companyDTOId);
    }
}