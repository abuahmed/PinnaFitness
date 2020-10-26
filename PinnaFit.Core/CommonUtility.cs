using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;

namespace PinnaFit.Core
{
    public static class CommonUtility
    {
        public static string Encrypt(string stringToEncrypt)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(stringToEncrypt);
            data = x.ComputeHash(data);
            var md5Hash = System.Text.Encoding.ASCII.GetString(data);

            return md5Hash;
        }

        public static IList<RoleDTO> GetRolesList()
        {
            return Enum.GetNames(typeof(RoleTypes))
                .Select(name => (RoleTypes)Enum.Parse(typeof(RoleTypes), name))
                .Select(GetRoleDTO).ToList();
        }

        public static RoleDTO GetRoleDTO(RoleTypes roleType)
        {
            var role = new RoleDTO
             {
                 RoleName = roleType.ToString(),
                 RoleDescription = EnumUtil.GetEnumDesc(roleType),
                 RoleDescriptionShort = roleType.ToString()
             };
            return role;
        }

        public static bool UserHasRole(RoleTypes role)//int userId,
        {
            return Singleton.User.Roles.Any(u => u.Role.RoleName == role.ToString());
        }

        public static IList<ListDataItem> GetList(Type enumType)
        {
            var enumList = new List<ListDataItem>();

            var enumTypes = Enum.GetNames(enumType);
            foreach (var staffType in enumTypes)
            {
                var staffTy = new ListDataItem
                {
                    Display = EnumUtil.GetEnumDesc(Enum.Parse(enumType, staffType)),
                    Value = Convert.ToInt32(Enum.Parse(enumType, staffType))
                };
                enumList.Add(staffTy);
            }

            return enumList;
        } 

        public static AddressDTO GetDefaultAddress()
        {
            return new AddressDTO()
            {
                Country = "Ethiopia",
                City = "Addis Abeba"
            };
        }
    }
}
