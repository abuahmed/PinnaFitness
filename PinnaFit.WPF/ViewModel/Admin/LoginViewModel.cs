using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinnaFit.Core.Common;
using PinnaFit.DAL;
using PinnaFit.Repository;
using PinnaFit.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Repository.Interfaces;
using PinnaFit.WPF.Views;
using WebMatrix.WebData;

namespace PinnaFit.WPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields
        private static IUnitOfWork _unitOfWork;
        private UserDTO _user;
        private ICommand _loginCommand, _closeLoginView;
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            CleanUp();
            InitializeWebSecurity();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            User = new UserDTO();
            //User.UserName = "PinnaFit01";

        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties

        public UserDTO User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged<UserDTO>(() => User);
            }
        }
        #endregion

        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand<object>(ExcuteLoginCommand, CanSave));
            }
        }
        private void ExcuteLoginCommand(object obj)
        {
            var values = (object[])obj;
            var psdBox = values[0] as PasswordBox;

            var user2 = new UserService().GetUser(1);

            //Do Validation if not handled on the UI
            if (psdBox != null && psdBox.Password == "")
            {
                psdBox.Focus();
                return;
            }

            if (psdBox != null)
            {
                var us = Membership.ValidateUser(User.UserName, psdBox.Password);

                if (!us)
                {
                    MessageBox.Show("IncorrectUserId", "Error Logging",
                                                            MessageBoxButton.OK,
                                                            MessageBoxImage.Error);
                    User.Password = "";
                    return;
                }

                int userId = WebSecurity.GetUserId(User.UserName);
                var user = new UserService().GetUser(userId);

                if (user == null)
                {
                    MessageBox.Show("Incorrect UserId", "Error Logging",
                                                            MessageBoxButton.OK,
                                                            MessageBoxImage.Error);
                    User.Password = "";
                }
                else
                {

                    Singleton.User = user;
                    Singleton.User.Password = psdBox.Password;
                    Singleton.UserRoles = new UserRolesModel();

                    #region Warehouse Filter
                    var warehouseList = new WarehouseService(true).GetWarehousesPrevilegedToUser(user.UserId).ToList();

                    if (warehouseList.Count > 1)
                        warehouseList.Insert(warehouseList.Count, new WarehouseDTO
                        {
                            DisplayName = "All",
                            Id = -1
                        });

                    Singleton.WarehousesList = warehouseList;
                    #endregion

                    switch (user.Status)
                    {
                        case UserTypes.Waiting:
                            new ChangePassword(psdBox.Password).Show();
                            break;
                        case UserTypes.Active:
                            new MainWindow().Show();
                            break;
                        default:
                            MessageBox.Show("Your account is blocked, Contact your system administrator",
                                "Acount blocked",MessageBoxButton.OK,MessageBoxImage.Error);
                            break;
                    }

                    CloseWindow(values[1]);
                }
            }
        }

        public ICommand CloseLoginView
        {
            get
            {
                return _closeLoginView ?? (_closeLoginView = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            //MessageBox.Show(DateTime.Now.TimeOfDay.Hours.ToString());
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        private void InitializeWebSecurity()
        {
            var dbContext2 = DbContextUtil.GetDbContextInstance();
            try
            {
                if (!WebSecurity.Initialized)
                    WebSecurity.InitializeDatabaseConnection(Singleton.ConnectionStringName, Singleton.ProviderName, "Users",
                        "UserId", "UserName", autoCreateTables: false);

                /*************************/
                IList<RoleDTO> listOfRoles = CommonUtility.GetRolesList();
                var lofRoles2 = new UserService(true).GetAllRoles().ToList();
                if (listOfRoles.Count != lofRoles2.Count)
                {
                    foreach (var role in listOfRoles)
                    {
                        var roleFound = lofRoles2.Any(role2 => role2.RoleName == role.RoleName);
                        if (!roleFound)
                            dbContext2.Set<RoleDTO>().Add(role);
                    }
                    dbContext2.SaveChanges();
                }
                /*************************/

                if (!new UserService(true).GetAll().Any())
                {
                    #region Seed Default Roles and Users
                    WebSecurity.CreateUserAndAccount("superadmin", "P@ssw0rd1!",
                        new
                        {
                            Status = 1,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "superadmin@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    WebSecurity.CreateUserAndAccount("adminuser", "P@ssw0rd",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "adminuser@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    WebSecurity.CreateUserAndAccount("PinnaFit01", "PinnaFit02",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "PinnaFit@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    
                    //add row guid for membership table members
                    var members = new UserService().GetAllMemberShips();
                    foreach (var membershipDTO in members)
                    {
                        membershipDTO.RowGuid = Guid.NewGuid();
                        membershipDTO.Enabled = true;
                        membershipDTO.CreatedByUserId = 1;
                        membershipDTO.DateRecordCreated = DateTime.Now;
                        membershipDTO.ModifiedByUserId = 1;
                        membershipDTO.DateLastModified = DateTime.Now;
                        dbContext2.Set<MembershipDTO>().Add(membershipDTO);
                        dbContext2.Entry(membershipDTO).State = EntityState.Modified;
                    }
                    dbContext2.SaveChanges();

                    var lofRoles = new UserService().GetAllRoles().ToList();
                    foreach (var role in lofRoles)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("superadmin")
                        });
                    }

                    foreach (var role in lofRoles.Skip(0))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("adminuser")
                        });
                    }

                    foreach (var role in lofRoles.Skip(0))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("PinnaFit01")
                        });
                    }

                    dbContext2.SaveChanges();
           
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem on InitializeWebSecurity" +
                    Environment.NewLine + ex.Message +
                    Environment.NewLine + ex.InnerException);
            }
            finally
            {
                dbContext2.Dispose();
            }
        }

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        #endregion
    }
}
