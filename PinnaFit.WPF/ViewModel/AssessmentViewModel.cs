#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class AssessmentViewModel : ViewModelBase
    {
        #region Fields

        private static IMemberAssessmentService _assessmentService;
        private static IMemberService _memberService;
        private IEnumerable<MemberAssessmentDTO> _memberAssessments;
        private ObservableCollection<MemberDTO> _members;
        private IEnumerable<MemberDTO> _memberList;
        private MemberDTO _selectedMember;
        private ObservableCollection<MemberAssessmentDTO> _filteredMemberAssessments;
        private MemberAssessmentDTO _selectedMemberAssessment;

        private ICommand _addNewMemberAssessmentViewCommand,
            _saveMemberAssessmentViewCommand,
            _deleteMemberAssessmentViewCommand;

        private string _progressBarVisibility;
        
        #endregion

        #region Constructor

        public AssessmentViewModel()
        {
            CleanUp();
            _assessmentService = new MemberAssessmentService();
            _memberService = new MemberService();

            CheckRoles();
            GetLiveMembers();
        }

        public static void CleanUp()
        {
            if (_assessmentService != null)
                _assessmentService.Dispose();
            if (_memberService != null)
                _memberService.Dispose();
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
        //public string TotalNumberOfAssessments
        //{
        //    get { return _totalNumberOfAssessments; }
        //    set
        //    {
        //        _totalNumberOfAssessments = value;
        //        RaisePropertyChanged<string>(() => TotalNumberOfAssessments);
        //    }
        //}
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
            }
        }
        //public ObservableCollection<MemberDTO> MemberListWithAssessment
        //{
        //    get { return _membersWithAssessment; }
        //    set
        //    {
        //        _membersWithAssessment = value;
        //        RaisePropertyChanged<ObservableCollection<MemberDTO>>(() => MemberListWithAssessment);

        //        if (MemberListWithAssessment != null)
        //            TotalNumberOfAssessments = MemberListWithAssessment.Count.ToString();
        //    }
        //}
        public MemberDTO SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMember);
                if (SelectedMember != null)
                {
                    EmployeeShortImage = new BitmapImage();
                    var photoAttachment = new AttachmentService(true).Find(SelectedMember.PhotoId.ToString());
                    if (photoAttachment != null)
                        EmployeeShortImage = ImageUtil.ToImage(photoAttachment.AttachedFile);

                    GetLiveMemberAssessments();

                    if (SelectedMember.Age != null && SelectedMember.Age > 10)
                    {
                        SelectedMember.DateOfBirth = DateTime.Now.AddYears((int) -SelectedMember.Age);
                        SelectedMember.AgeFromBirthDate = DateTime.Now.Year - BirthDate.Year;
                    }

                    BirthDate = SelectedMember.DateOfBirth;

                    MemberAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedMember.Address
                    };

                    MemberContactPersonDetail = new ObservableCollection<ContactPersonDTO>();
                    if (SelectedMember.ContactPerson != null && SelectedMember.ContactPerson.Address != null)
                        MemberContactPersonDetail.Add(SelectedMember.ContactPerson);

                    MemberAssessmentMoreDetail = new ObservableCollection<MemberAssessmentMoreDTO>();
                    if (SelectedMember.MemberAssessmentMore != null)
                        MemberAssessmentMoreDetail.Add(SelectedMember.MemberAssessmentMore);
                }
            }
        }
        public void GetLiveMembers()
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
                ProgressBarVisibility = "Collapsed";
                Members = new ObservableCollection<MemberDTO>(MemberList);
            }
            catch
            {
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetLiveMembersBk();
            }
            catch
            {
            }
        }
        public void GetLiveMembersBk()
        {
            var criteria = new SearchCriteria<MemberDTO>
            {
                ShortForm = false
            };
            criteria.FiList.Add(m=>m.LastSubscription!=null);
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            criteria.FiList.Add(m=>m.LastSubscription.EndDate>=today);

            MemberList = _memberService.GetAll(criteria).OrderByDescending(i => i.Id).ToList();
        }

        public MemberAssessmentDTO SelectedMemberAssessment
        {
            get { return _selectedMemberAssessment; }
            set
            {
                _selectedMemberAssessment = value;
                RaisePropertyChanged<MemberAssessmentDTO>(() => SelectedMemberAssessment);
                if (SelectedMemberAssessment != null)
                {
                    AssessmentDate = SelectedMemberAssessment.AssessmentTime;
                }
            }
        }

        public IEnumerable<MemberAssessmentDTO> MemberAssessmentList
        {
            get { return _memberAssessments; }
            set
            {
                _memberAssessments = value;
                RaisePropertyChanged<IEnumerable<MemberAssessmentDTO>>(() => MemberAssessmentList);
            }
        }

        public ObservableCollection<MemberAssessmentDTO> MemberAssessments
        {
            get { return _filteredMemberAssessments; }
            set
            {
                _filteredMemberAssessments = value;
                RaisePropertyChanged<ObservableCollection<MemberAssessmentDTO>>(() => MemberAssessments);

                if (MemberAssessments != null && MemberAssessments.Any())
                {
                    SelectedMemberAssessment = MemberAssessments.FirstOrDefault();
                    
                }
                else
                    AddNewMemberAssessment();
            }
        }

        #endregion

        #region Filter List

        private ICommand _birthDateViewCommand,_assessmentDateViewCommand;
        private string _birthDateText, _assessmentDateText;//, _totalNumberOfAssessments;
        private DateTime _birthDate,_assessmentDate;

        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                RaisePropertyChanged<DateTime>(() => BirthDate);
                if (BirthDate.Year > 1900)
                {
                    BirthDateText = ReportUtility.GetEthCalendarFormated(BirthDate, "-") +
                                    "(" + BirthDate.ToString("dd-MM-yyyy") + ")";
                }
            }
        }

        public string BirthDateText
        {
            get { return _birthDateText; }
            set
            {
                _birthDateText = value;
                RaisePropertyChanged<string>(() => BirthDateText);
            }
        }

        public ICommand BirthDateViewCommand
        {
            get
            {
                return _birthDateViewCommand ??
                       (_birthDateViewCommand = new RelayCommand(ExcuteBirthDate));
            }
        }

        public void ExcuteBirthDate()
        {
            var bd = BirthDate;
            if (bd.Year < 1900)
                bd = DateTime.Now;

            var calConv = new CalendarConvertor(bd);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                {
                    BirthDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
                    if (SelectedMember != null)
                    {
                        SelectedMember.Age = DateTime.Now.Year - BirthDate.Year;
                        SelectedMember.AgeFromBirthDate = DateTime.Now.Year - BirthDate.Year;
                    }
                }
            }
        }


        public DateTime AssessmentDate
        {
            get { return _assessmentDate; }
            set
            {
                _assessmentDate = value;
                RaisePropertyChanged<DateTime>(() => AssessmentDate);
                if (AssessmentDate.Year > 1900)
                {
                    AssessmentDateText = ReportUtility.GetEthCalendarFormated(AssessmentDate, "-") +
                                    "(" + AssessmentDate.ToString("dd-MM-yyyy") + ")";
                }
            }
        }

        public string AssessmentDateText
        {
            get { return _assessmentDateText; }
            set
            {
                _assessmentDateText = value;
                RaisePropertyChanged<string>(() => AssessmentDateText);
            }
        }

        public ICommand AssessmentDateViewCommand
        {
            get
            {
                return _assessmentDateViewCommand ??
                       (_assessmentDateViewCommand = new RelayCommand(ExcuteAssessmentDate));
            }
        }

        public void ExcuteAssessmentDate()
        {
            var bd = AssessmentDate;
            if (bd.Year < 1900)
                bd = DateTime.Now;

            var calConv = new CalendarConvertor(bd);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                {
                    AssessmentDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
                    SelectedMemberAssessment.AssessmentTime = AssessmentDate;
                }
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewMemberAssessmentViewCommand
        {
            get
            {
                return _addNewMemberAssessmentViewCommand ??
                       (_addNewMemberAssessmentViewCommand = new RelayCommand(AddNewMemberAssessment));
            }
        }

        private void AddNewMemberAssessment()
        {
            decimal? height = null;
            if (MemberAssessments.Any())
            {
                var memberAssessmentDto = MemberAssessments.FirstOrDefault();
                if (memberAssessmentDto != null) height = memberAssessmentDto.Height;
            }

            if (SelectedMember != null)
                SelectedMemberAssessment = new MemberAssessmentDTO
                {
                    MemberId = SelectedMember.Id,
                    AssessmentTime = DateTime.Now,
                    Height = height
                };
        }

        public ICommand SaveMemberAssessmentViewCommand
        {
            get
            {
                return _saveMemberAssessmentViewCommand ??
                       (_saveMemberAssessmentViewCommand = new RelayCommand<Object>(SaveMemberAssessment, CanSave));
            }
        }

        private void SaveMemberAssessment(object obj)
        {
            try
            {
                if (SelectedMember == null)
                    return;

                SelectedMember.DateOfBirth = BirthDate;
                var statMem = _memberService.InsertOrUpdate(SelectedMember);

                if (statMem != string.Empty)
                    MessageBox.Show("Can't save Member Data"
                                    + Environment.NewLine + statMem, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                if(SelectedMemberAssessment.Height==null || SelectedMemberAssessment.Weight==null)
                    return;

                var newObject = SelectedMemberAssessment.Id;
                var stat = _assessmentService.InsertOrUpdate(SelectedMemberAssessment);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save Assessment Data"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    MemberAssessments.Insert(0, SelectedMemberAssessment);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteMemberAssessmentViewCommand
        {
            get
            {
                return _deleteMemberAssessmentViewCommand ??
                       (_deleteMemberAssessmentViewCommand = new RelayCommand<Object>(DeleteMemberAssessment, CanSave));
            }
        }

        private void DeleteMemberAssessment(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?", "PinnaFit",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedMemberAssessment.Enabled = false;
                    var stat = _assessmentService.Disable(SelectedMemberAssessment);
                    if (stat == string.Empty)
                    {
                        MemberAssessments.Remove(SelectedMemberAssessment);
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

        #endregion

        public void GetLiveMemberAssessments()
        {
            var criteria = new SearchCriteria<MemberAssessmentDTO>();
            criteria.FiList.Add(f => f.MemberId == SelectedMember.Id);

            MemberAssessmentList = _assessmentService.GetAll(criteria).OrderBy(i => i.Id).ToList();
            MemberAssessments = new ObservableCollection<MemberAssessmentDTO>(MemberAssessmentList);
        }

        #region More Windows
        private ICommand _memberAddressViewCommand;
        private ObservableCollection<AddressDTO> _memberAddressDetail;
        public ObservableCollection<AddressDTO> MemberAdressDetail
        {
            get { return _memberAddressDetail; }
            set
            {
                _memberAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => MemberAdressDetail);
            }
        }
        public ICommand MemberAddressViewCommand
        {
            get { return _memberAddressViewCommand ?? (_memberAddressViewCommand = new RelayCommand(MemberAddress)); }
        }
        public void MemberAddress()
        {
            if (SelectedMember != null) new AddressEntry(SelectedMember.Address).ShowDialog();
        }

        private ICommand _memberContactPersonViewCommand;
        private ObservableCollection<ContactPersonDTO> _memberContactPersonDetail;
        public ObservableCollection<ContactPersonDTO> MemberContactPersonDetail
        {
            get { return _memberContactPersonDetail; }
            set
            {
                _memberContactPersonDetail = value;
                RaisePropertyChanged<ObservableCollection<ContactPersonDTO>>(() => MemberContactPersonDetail);
            }
        }
        public ICommand MemberContactPersonViewCommand
        {
            get { return _memberContactPersonViewCommand ?? (_memberContactPersonViewCommand = new RelayCommand(MemberContactPerson)); }
        }
        public void MemberContactPerson()
        {
            if (SelectedMember != null) new ContactPersonEntry(SelectedMember.ContactPerson).ShowDialog();
        }

        private ICommand _memberAssessmentMoreViewCommand;
        private ObservableCollection<MemberAssessmentMoreDTO> _memberAssessmentMoreDetail;
        public ObservableCollection<MemberAssessmentMoreDTO> MemberAssessmentMoreDetail
        {
            get { return _memberAssessmentMoreDetail; }
            set
            {
                _memberAssessmentMoreDetail = value;
                RaisePropertyChanged<ObservableCollection<MemberAssessmentMoreDTO>>(() => MemberAssessmentMoreDetail);
            }
        }
        public ICommand MemberAssessmentMoreViewCommand
        {
            get { return _memberAssessmentMoreViewCommand ?? (_memberAssessmentMoreViewCommand = new RelayCommand(MemberAssessmentMore)); }
        }
        public void MemberAssessmentMore()
        {
            if (SelectedMember != null)
            {
                if (SelectedMember.MemberAssessmentMore == null)
                    SelectedMember.MemberAssessmentMore = new MemberAssessmentMoreDTO();
                new MemberAssessmentMoreEntry(SelectedMember.MemberAssessmentMore).ShowDialog();
            }
        } 
        #endregion
        
        #region short photo
        private BitmapImage _employeeShortImage;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
            }
        }
        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
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