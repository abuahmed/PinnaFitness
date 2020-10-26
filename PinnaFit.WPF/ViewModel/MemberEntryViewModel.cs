#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Reports;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class MemberEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IMemberService _memberService;
        private static IAttachmentService _attachmentService;
        private IEnumerable<MemberDTO> _memberList;
        private ObservableCollection<MemberDTO> _members;
        private MemberDTO _selectedMember, _selectedMemberForFilter;
        private AttachmentDTO _photoAttachment;

        private ObservableCollection<AddressDTO> _memberAddressDetail;
        private ObservableCollection<AddressDTO> _contactAddressDetail;
        private ObservableCollection<MemberSubscriptionDTO> _memberSubscriptionDetail;

        private ICommand _addNewMemberViewCommand,
            _saveMemberViewCommand,
            _deleteMemberViewCommand,
            _memberAddressViewCommand,
            _memberSubscriptionViewCommand,
            _memberSubscriptionRenewViewCommand,
            _refreshCommand,
            _staffContactAddressViewCommand;

        private string _searchText,
            _memberText,
            _progressBarVisibility,
            _packageCommandVisibility,
            _packageRenewCommandVisibility,
            _totalNumberOfEmployees;

        private bool _editingCommandVisibility;

        #endregion

        #region Constructor

        public MemberEntryViewModel()
        {
            ProgressBarVisibility = "Collapsed";
            MemberText = "ፓኬጅ አስገባ";
            FillMemberTypes();
            CheckRoles();
            Load();
        }

        public void Load()
        {
            CleanUp();
            _memberService = new MemberService();
            _attachmentService = new AttachmentService();

            GetLiveMembersBk();
        }

        public static void CleanUp()
        {
            if (_memberService != null)
                _memberService.Dispose();
            if (_attachmentService != null)
                _attachmentService.Dispose();
        }

        #endregion

        #region Public Properties

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (MemberList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Members = new ObservableCollection<MemberDTO>
                                (MemberList.Where(c => c.MemberDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Members = new ObservableCollection<MemberDTO>(MemberList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching Member");
                        Members = new ObservableCollection<MemberDTO>(MemberList);
                    }
                }
            }
        }

        public string MemberText
        {
            get { return _memberText; }
            set
            {
                _memberText = value;
                RaisePropertyChanged<string>(() => MemberText);
            }
        }

        public string TotalNumberOfMembers
        {
            get { return _totalNumberOfEmployees; }
            set
            {
                _totalNumberOfEmployees = value;
                RaisePropertyChanged<string>(() => TotalNumberOfMembers);
            }
        }

        public string PackageCommandVisibility
        {
            get { return _packageCommandVisibility; }
            set
            {
                _packageCommandVisibility = value;
                RaisePropertyChanged<string>(() => PackageCommandVisibility);
            }
        }

        public string PackageRenewCommandVisibility
        {
            get { return _packageRenewCommandVisibility; }
            set
            {
                _packageRenewCommandVisibility = value;
                RaisePropertyChanged<string>(() => PackageRenewCommandVisibility);
            }
        }

        public bool EditingCommandVisibility
        {
            get { return _editingCommandVisibility; }
            set
            {
                _editingCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditingCommandVisibility);
            }
        }

        public MemberDTO SelectedMemberForFilter
        {
            get { return _selectedMemberForFilter; }
            set
            {
                _selectedMemberForFilter = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMemberForFilter);
                if (SelectedMemberForFilter != null)
                {
                    SelectedMember = SelectedMemberForFilter;
                }
                
            }
        }

        public MemberDTO SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMember);
                if (SelectedMember != null)
                {
                    //_photoAttachment = _attachmentService.Find(SelectedMember.PhotoId.ToString());
                    //EmployeeShortImage = _photoAttachment != null
                    //    ? ImageUtil.ToImage(_photoAttachment.AttachedFile)
                    //    : new BitmapImage();
                    var photoAttachment2 = new AttachmentService(true).Find(SelectedMember.PhotoId.ToString());
                    EmployeeShortImage = photoAttachment2 != null
                        ? ImageUtil.ToImage(photoAttachment2.AttachedFile)
                        : new BitmapImage();

                    if (MemberTypeList != null)
                        SelectedMemberType = MemberTypeList.FirstOrDefault(s => s.Value == (int) SelectedMember.Type);

                    MemberAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedMember.Address
                    };

                    ContactAdressDetail = new ObservableCollection<AddressDTO>();
                    if (SelectedMember.ContactPerson != null && SelectedMember.ContactPerson.Address != null)
                        ContactAdressDetail.Add(SelectedMember.ContactPerson.Address);

                    EditingCommandVisibility = true;
                    if (UserRoles.MemberEdit != "Visible")
                        EditingCommandVisibility = false;
                    LoadSubscriptions();


                }else{AddNewMember();}
            }
        }

        public void LoadSubscriptions()
        {
            MemberSubscriptionDetail = new ObservableCollection<MemberSubscriptionDTO>();
            var member = new MemberService(true).Find(SelectedMember.Id.ToString());
            var lastSubId = member != null ? member.LastSubscriptionId : 0;

            var criteria = new SearchCriteria<MemberSubscriptionDTO>();
            criteria.FiList.Add(m => m.Id == lastSubId); //SelectedMember.LastSubscriptionId

            var lastSub = new MemberSubscriptionService(true).GetAll(criteria).FirstOrDefault();
            
            PackageCommandVisibility = "Collapsed";
            PackageRenewCommandVisibility = "Collapsed";

            if (lastSub == null)
            {
                MemberText = "ፓኬጅ አስገባ";

                if (UserRoles.PackageAdd == "Visible")
                    PackageCommandVisibility = "Visible";
            }
            else
            {
                TotalNumberOfMembers = lastSub.DaysLeft +" "+ lastSub.CurrentStatus;

                MemberText = "ፓኬጅ አስተካክል";
                if (UserRoles.PackageEdit == "Visible")
                    PackageCommandVisibility = "Visible";

                if (lastSub.EndDate != null)
                {
                    var daysleft = lastSub.EndDate.Value.Subtract(DateTime.Now).Days;
                    if (daysleft < 7) //lastSub.SubscriptionExpired
                    {
                        if (UserRoles.PackageRenewal == "Visible")
                            PackageRenewCommandVisibility = "Visible";
                    }
                }

                MemberSubscriptionDetail.Add(lastSub);
            }
        }

        public IEnumerable<MemberDTO> MemberList
        {
            get { return _memberList; }
            set
            {
                _memberList = value;
                RaisePropertyChanged<IEnumerable<MemberDTO>>(() => MemberList);
            }
        }

        public ObservableCollection<MemberDTO> Members
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged<ObservableCollection<MemberDTO>>(() => Members);

                TotalNumberOfMembers = "";
                if (Members != null && Members.Any())
                {
                    TotalNumberOfMembers = Members.Count.ToString() + " members found";
                }
            }
        }

        public ObservableCollection<AddressDTO> MemberAdressDetail
        {
            get { return _memberAddressDetail; }
            set
            {
                _memberAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => MemberAdressDetail);
            }
        }

        public ObservableCollection<AddressDTO> ContactAdressDetail
        {
            get { return _contactAddressDetail; }
            set
            {
                _contactAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactAdressDetail);
            }
        }

        public ObservableCollection<MemberSubscriptionDTO> MemberSubscriptionDetail
        {
            get { return _memberSubscriptionDetail; }
            set
            {
                _memberSubscriptionDetail = value;
                RaisePropertyChanged<ObservableCollection<MemberSubscriptionDTO>>(() => MemberSubscriptionDetail);
            }
        }

        #endregion

        #region Filter List

        private List<ListDataItem> _memberTypeList;
        private ListDataItem _selectedMemberType;

        public List<ListDataItem> MemberTypeList
        {
            get { return _memberTypeList; }
            set
            {
                _memberTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => MemberTypeList);
            }
        }

        public ListDataItem SelectedMemberType
        {
            get { return _selectedMemberType; }
            set
            {
                _selectedMemberType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedMemberType);
            }
        }

        private List<ListDataItem> _memberTypeListForFilter;
        private ListDataItem _selectedMemberTypeForFilter;

        public List<ListDataItem> MemberTypeListForFilter
        {
            get { return _memberTypeListForFilter; }
            set
            {
                _memberTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => MemberTypeListForFilter);
            }
        }

        public ListDataItem SelectedMemberTypeForFilter
        {
            get { return _selectedMemberTypeForFilter; }
            set
            {
                _selectedMemberTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedMemberTypeForFilter);
                //GetLiveMembersBk();
            }
        }

        public void FillMemberTypes()
        {
            var memberTypes = (List<ListDataItem>) CommonUtility.GetList(typeof (MemberTypes));

            MemberTypeListForFilter = memberTypes.ToList();
            SelectedMemberTypeForFilter = MemberTypeListForFilter.FirstOrDefault();

            if (memberTypes != null && memberTypes.Count > 1)
            {
                MemberTypeList = memberTypes.Skip(1).ToList();
                SelectedMemberType = MemberTypeList.FirstOrDefault();
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewMemberViewCommand
        {
            get { return _addNewMemberViewCommand ?? (_addNewMemberViewCommand = new RelayCommand(AddNewMember)); }
        }

        private void AddNewMember()
        {
            try
            {
                SelectedMember = new MemberDTO
                {
                    Type = MemberTypes.Normal,
                    DateOfBirth = DateTime.Now,
                    IsActive = true,
                    Sex = Sex.Male,
                    Address = new AddressDTO
                    {
                        Country = "ኢትዮጲያ",
                        City = "አዲስ አበባ"
                    },
                    Photo = new AttachmentDTO(),
                    ContactPerson = new ContactPersonDTO()
                    {
                        DisplayName = "-",
                        Sex = Sex.Male,
                        Address = new AddressDTO
                        {
                            Country = "ኢትዮጲያ",
                            City = "አዲስ አበባ"
                        }
                    },
                    MemberAssessmentMore = new MemberAssessmentMoreDTO()
                };

                EmployeeShortImage = new BitmapImage();
                EditingCommandVisibility = true;
                PackageRenewCommandVisibility = "Collapsed";
            }
            catch (Exception)
            {
                //MessageBox.Show("Problem on adding new Member");
            }
        }

        public ICommand SaveMemberViewCommand
        {
            get
            {
                return _saveMemberViewCommand ?? (_saveMemberViewCommand = new RelayCommand<Object>(SaveMem, CanSave));
            }
        }

        private void SaveMem(object obj)
        {
            SaveMember(obj);
        }

        public bool SaveMember(object obj)
        {
            try
            {
                if (SelectedMember.Address != null)
                {
                    if (string.IsNullOrEmpty(SelectedMember.Address.Mobile))
                    {
                        MessageBox.Show("Fill Member Mobile Number!!");
                        MemberAddress();
                        return false;
                    }
                }

                var newObject = SelectedMember.Id;
                SelectedMember.Type = (MemberTypes) SelectedMemberType.Value;

                var stat = _memberService.InsertOrUpdate(SelectedMember);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                {
                    SelectedMember.Number = _memberService.GetMemberNumber(SelectedMember.Id);
                    stat = _memberService.InsertOrUpdate(SelectedMember);
                    if (stat != string.Empty)
                    {
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return false;
                    }
                    else
                    {
                        var criteria = new SearchCriteria<MemberDTO>();
                        criteria.FiList.Add(m => m.Id == SelectedMember.Id);
                        SelectedMember = _memberService.GetAll(criteria).FirstOrDefault();
                        Members.Insert(0, SelectedMember);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        public ICommand DeleteMemberViewCommand
        {
            get
            {
                return _deleteMemberViewCommand ??
                       (_deleteMemberViewCommand = new RelayCommand<Object>(DeleteMember, CanSave));
            }
        }

        private void DeleteMember(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedMember.Enabled = false;
                    var stat = _memberService.Disable(SelectedMember);
                    if (stat == string.Empty)
                    {
                        Members.Remove(SelectedMember);
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                                        + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand MemberAddressViewCommand
        {
            get { return _memberAddressViewCommand ?? (_memberAddressViewCommand = new RelayCommand(MemberAddress)); }
        }

        public void MemberAddress()
        {
            new AddressEntry(SelectedMember.Address).ShowDialog();
        }

        public ICommand StaffContactAddressViewCommand
        {
            get
            {
                return _staffContactAddressViewCommand ??
                       (_staffContactAddressViewCommand = new RelayCommand(StaffContactAddress));
            }
        }

        public void StaffContactAddress()
        {
            if (SelectedMember != null && SelectedMember.ContactPerson != null &&
                SelectedMember.ContactPerson.Address != null)
                new AddressEntry(SelectedMember.ContactPerson.Address).ShowDialog();
        }

        public ICommand MemberSubscriptionViewCommand
        {
            get
            {
                return _memberSubscriptionViewCommand ??
                       (_memberSubscriptionViewCommand = new RelayCommand(MemberSubscriptionAddEdit));
            }
        }

        public void MemberSubscriptionAddEdit()
        {
            MemberSubscription(false);
        }

        public ICommand MemberSubscriptionRenewViewCommand
        {
            get
            {
                return _memberSubscriptionRenewViewCommand ??
                       (_memberSubscriptionRenewViewCommand = new RelayCommand(MemberSubscriptionRenew));
            }
        }

        public void MemberSubscriptionRenew()
        {
            MemberSubscription(true);
        }

        public void MemberSubscription(bool isRenewal)
        {
            bool showDialogue = true;
            var memberSub = new Window();// MemberSubscriptions(SelectedMember);/
            try
            {
                if (!isRenewal)
                {
                    if (!SaveMember(null))
                        showDialogue = false;
                    else memberSub = new MemberSubscriptions(SelectedMember);
                }
                else
                {
                    memberSub = new MemberSubscriptions(SelectedMember, true);
                }

                if (showDialogue)
                {
                    _selectedMemberId = SelectedMember.Id;
                    memberSub.ShowDialog();
                    if (memberSub.DialogResult != null && (bool) memberSub.DialogResult)
                    {
                        if (Members != null) SelectedMember = Members.FirstOrDefault(m => m.Id == _selectedMemberId); 
                        
                        var criteria = new SearchCriteria<MemberSubscriptionDTO>();
                        criteria.FiList.Add(m => m.MemberId == SelectedMember.Id);
                        var memSub = new MemberSubscriptionService(true)
                            .GetAll(criteria)
                            .OrderByDescending(m=>m.Id)
                            .FirstOrDefault();

                        if (memSub != null && SelectedMember != null)
                                SelectedMember.LastSubscriptionId = memSub.Id; //Convert.ToInt32(memberSub.TxtId);
                    }
                }
            }
            catch
            {
                MessageBox.Show("got problem while processing member package, try again");
            }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        public void ExcuteRefreshCommand()
        {
            //Load();
        }

        private ICommand _viewLogCommandView;

        public ICommand ViewLogCommandView
        {
            get { return _viewLogCommandView ?? (_viewLogCommandView = new RelayCommand<Object>(ViewLog)); }
        }

        private void ViewLog(object obj)
        {
            try
            {
                MessageBox.Show(
                    "የፈጠረው: " + SelectedMember.CreatedByUser.UserName + Environment.NewLine +
                    "የተፈጠረበት ቀን: " + SelectedMember.DateRecordCreatedStringAndAmharic + Environment.NewLine +
                    "መጨረሻ የቀየረው: " + SelectedMember.ModifiedByUser.UserName + Environment.NewLine +
                    "መጨረሻ የተቀየረበት ቀን: " + SelectedMember.DateLastModifiedStringAndAmharic
                    , "Log", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't Get Log Data" + Environment.NewLine + ex.Message);
            }
        }

        private int _selectedMemberId;

        #endregion

        //private static object _obj;
        public void GetLiveMembersBk()
        {
            ProgressBarVisibility = "Visible";

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Members = new ObservableCollection<MemberDTO>(MemberList);
                ProgressBarVisibility = "Collapsed";
            }
            catch
            {
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetLiveMembers();
            }
            catch
            {
            }
        }

        public void GetLiveMembers()
        {
            var criteria = new SearchCriteria<MemberDTO>
            {
                ShortForm = true
                //PageSize = 10, Page = 1
            };

            var stafType = (MemberTypes) SelectedMemberTypeForFilter.Value;
            if (stafType != MemberTypes.All)
                criteria.FiList.Add(b => b.Type == stafType);

            MemberList = _memberService.GetAll(criteria).ToList();
        }

        #region Short Photo

        private BitmapImage _employeeShortImage;
        private ICommand _showEmployeeShortImageCommand;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
            }
        }

        public ICommand ShowEmployeeShortImageCommand
        {
            get
            {
                return _showEmployeeShortImageCommand ??
                       (_showEmployeeShortImageCommand = new RelayCommand(ExecuteShowEmployeeShortImageViewCommand));
            }
        }

        private void ExecuteShowEmployeeShortImageViewCommand()
        {
            var file = new OpenFileDialog {Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg"};
            var result = file.ShowDialog();
            if (result != null && ((bool) result && File.Exists(file.FileName)))
            {
                EmployeeShortImage = new BitmapImage(new Uri(file.FileName, true));
                if (!SaveMember(null))
                    MessageBox.Show("Can't Save Member Data, try again later");

                if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null)
                {
                    _photoAttachment = new AttachmentDTO
                    {
                        AttachedFile = ImageUtil.ToBytes(EmployeeShortImage)
                    };
                    SelectedMember.Photo = _photoAttachment;
                    var stat = _memberService.InsertOrUpdate(SelectedMember);
                    if (!string.IsNullOrEmpty(stat))
                    {
                        MessageBox.Show("Can't Add Photo, try again later");
                    }
                }

                ////if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null)
                ////{
                ////    _photoAttachment = _attachmentService.Find(SelectedMember.PhotoId.ToString());
                ////    if (_photoAttachment != null)
                ////    {
                ////        _photoAttachment.AttachedFile = ImageUtil.ToBytes(EmployeeShortImage);
                ////        SelectedMember.Photo = _photoAttachment;
                ////    }
                ////    else
                ////    {
                ////        _photoAttachment = new AttachmentDTO
                ////        {
                ////            AttachedFile = ImageUtil.ToBytes(EmployeeShortImage)
                ////        };
                ////        SelectedMember.Photo = _photoAttachment;
                ////    }
                ////}
            }
        }

        #endregion

        #region Print List

        private ICommand _printListCommandView;

        public ICommand PrintListCommandView
        {
            get { return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList)); }
        }

        public void PrintList(object obj)
        {
            ReportDocument myReport4 = null;

            if (SelectedMember.LastSubscription == null || SelectedMember.LastSubscription.FacilitySubscription == null)
                return;

            //if (SelectedMember.LastSubscription.FacilitySubscription.FacilityId == 8) //Full
            myReport4 = new Reports.MemberID5();
            //else if (SelectedMember.LastSubscription.FacilitySubscription.FacilityId == 2) //Gym With Sauna
            //    myReport4 = new Reports.MemberID5();
            //else if (SelectedMember.LastSubscription.FacilitySubscription.FacilityId == 6) //Gym and Aerobics
            //    myReport4 = new Reports.MemberID3();

            if (myReport4 != null)
            {
                myReport4.SetDataSource(GetListDataSet());

                //MenuItem menu = obj as MenuItem;
                //if (menu != null)
                //    new ReportUtility().DirectPrinter(myReport4);
                //else
                //{
                var report = new ReportViewerCommon(myReport4);
                report.ShowDialog();
            }
            //}
        }

        public FitnessDataSet GetListDataSet()
        {
            var myDataSet = new FitnessDataSet();

            var brCode = new BarcodeProcess();
            var memberNumberbarcode = ImageToByteArray(brCode.GetBarcode(SelectedMember.Number.ToString(), 1200, 40, false),
                    ImageFormat.Bmp);
                //ImageToByteArray(brCode.GetBarcode(SelectedMember.Id.ToString() + "11850908", 1200, 40, false),
                //    ImageFormat.Bmp);

            var memberPhoto = new AttachmentService(true).Find(SelectedMember.PhotoId.ToString())
                              ?? new AttachmentDTO();

            var memberSub = MemberSubscriptionDetail.FirstOrDefault();
            //if (SelectedMember.LastSubscription != null)
            myDataSet.MemberDetail.Rows.Add(
                SelectedMember.DisplayName,
                memberPhoto.AttachedFile,
                SelectedMember.Number,
                memberNumberbarcode,
                SelectedMember.SexAmharic,
                SelectedMember.Address.AddressDetail,
                "", "", "", "",
                SelectedMember.Address.Mobile,
                "", "", "", "",
                memberSub != null ? memberSub.StartDateStringAndAmharic : null,
                memberSub != null ? memberSub.EndDateStringAndAmharic : null,
                memberSub != null ? memberSub.FacilitySubscription.PackageName : null,
                "");

            //SelectedMember.LastSubscription.SubscribedDateStringAndAmharic,
            //        SelectedMember.LastSubscription.EndDateString,
            //        SelectedMember.LastSubscription.FacilitySubscription.PackageName,

            return myDataSet;
        }

        public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }

        #endregion

        #region Get Attachment

        private ICommand _printTransactionCommand;

        public ICommand PrintTransactionCommand
        {
            get
            {
                return _printTransactionCommand ??
                       (_printTransactionCommand = new RelayCommand<Object>(GetAttachment));
            }
        }

        public void GetAttachment(object obj)
        {
            var myReport = new SingleTransaction();
            var dSet = GetAttachmentDataSet();

            if (dSet == null)
                MessageBox.Show("No Data found to display!");
            else
            {
                myReport.SetDataSource(dSet);
                var report = new ReportViewerCommon(myReport);
                report.Show();
            }

        }

        public TransactionDataSet GetAttachmentDataSet()
        {
            var myDataSet = new TransactionDataSet();
            var selectedCompany = new CompanyService(true).GetCompany();

            var cri = new SearchCriteria<MemberSubscriptionDTO>();
            cri.FiList.Add(s=>s.Id==SelectedMember.LastSubscriptionId);
            var selectedTransaction = new MemberSubscriptionService(true).GetAll(cri).FirstOrDefault();
            if (selectedTransaction == null) return null;

            try
            {
                #region Fields

                var brCode = new BarcodeProcess();
                var tranNumberbarcode =
                    ImageToByteArray(brCode.GetBarcode(selectedTransaction.SubscriptionNumber, 320, 40, true),
                        ImageFormat.Bmp);

                var subTotal = selectedTransaction.AmountPaid/(decimal) 1.15;
                var tax = Convert.ToDecimal((subTotal*((decimal) 0.15)).ToString("N2"));

                string reciptNo = selectedTransaction.ReceiptNumber, prefix = "";
                int recLen = 10 - reciptNo.Length;
                while (recLen != 0)
                {
                    prefix = prefix + "0";
                    recLen--;
                }
                reciptNo = prefix + reciptNo;

                #endregion

                #region Header

                myDataSet.TransactionHeader.Rows.Add(
                    reciptNo,
                    tranNumberbarcode,
                    SelectedMember.TinNumber,
                    SelectedMember.DisplayName,
                    SelectedMember.Number,
                    SelectedMember.SexAmharic,
                    selectedTransaction.SubscribedDateString + "(" +
                    ReportUtility.GetEthCalendarFormated(selectedTransaction.SubscribedDate, "/") + ")",
                    "",
                    subTotal,
                    "VAT (" + 15 + "%)",
                    tax,
                    subTotal + tax,
                    "linknumber1"
                    );

                #endregion

                #region Client Address

                myDataSet.ClientDetail.Rows.Add(
                    selectedCompany.Header,
                    selectedCompany.Footer,
                    selectedCompany.Address.AddressDetail,
                    selectedCompany.Address.SubCity,
                    selectedCompany.Address.Kebele,
                    selectedCompany.Address.HouseNumber,
                    selectedCompany.Address.Mobile,
                    selectedCompany.Address.AlternateMobile,
                    selectedCompany.Address.Fax,
                    selectedCompany.Address.PrimaryEmail,
                    selectedCompany.Address.AlternateEmail,
                    selectedCompany.TinNumber,
                    selectedCompany.VatNumber,
                    "CASH", "CASH", Singleton.User.FullName.ToUpper(), "linknumber1");

                #endregion

                #region BPAddress

                myDataSet.BPAddress.Rows.Add(
                    SelectedMember.Address.AddressDetail,
                    SelectedMember.Address.SubCity,
                    SelectedMember.Address.Kebele,
                    SelectedMember.Address.HouseNumber,
                    SelectedMember.Address.Mobile,
                    SelectedMember.Address.AlternateMobile,
                    SelectedMember.Address.Fax,
                    SelectedMember.Address.PrimaryEmail,
                    SelectedMember.Address.AlternateEmail,
                    "linknumber1");

                #endregion

                #region Lines

                myDataSet.TransactionLine.Rows.Add(
                    "1",
                    "00" + selectedTransaction.FacilitySubscription.Facility.Id.ToString(),
                    selectedTransaction.FacilitySubscription.PackageName,
                    "",
                    "Gym",
                    "Pcs",
                    subTotal,
                    1,
                    subTotal,
                    0,
                    "linknumber1");

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't get data for the report"
                                + Environment.NewLine + exception.Message, "Can't get data", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return myDataSet;
        }

        #endregion

        #region Previlege Visibility

        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}