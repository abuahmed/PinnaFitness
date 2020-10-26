namespace PinnaFit.Core.Models
{
    public class MemberAssessmentMoreDTO : CommonFieldsA
    {
        //Health History Questionnaire
        public bool HeartDisease
        {
            get { return GetValue(() => HeartDisease); }
            set { SetValue(() => HeartDisease, value); }
        }
        public bool LungDisease
        {
            get { return GetValue(() => LungDisease); }
            set { SetValue(() => LungDisease, value); }
        }
        public bool Diabetes
        {
            get { return GetValue(() => Diabetes); }
            set { SetValue(() => Diabetes, value); }
        }
        public bool Alergies
        {
            get { return GetValue(() => Alergies); }
            set { SetValue(() => Alergies, value); }
        }
        public bool Asthma
        {
            get { return GetValue(() => Asthma); }
            set { SetValue(() => Asthma, value); }
        }
        public bool BackPain
        {
            get { return GetValue(() => BackPain); }
            set { SetValue(() => BackPain, value); }
        }
        public bool Arthritis
        {
            get { return GetValue(() => Arthritis); }
            set { SetValue(() => Arthritis, value); }
        }
        public bool EatingDisorder
        {
            get { return GetValue(() => EatingDisorder); }
            set { SetValue(() => EatingDisorder, value); }
        }
        public bool JointProblem
        {
            get { return GetValue(() => JointProblem); }
            set { SetValue(() => JointProblem, value); }
        }
        public bool Depression
        {
            get { return GetValue(() => Depression); }
            set { SetValue(() => Depression, value); }
        }
        public string OtherHealthIssue
        {
            get { return GetValue(() => OtherHealthIssue); }
            set { SetValue(() => OtherHealthIssue, value); }
        }

        //Answer Questions
        public bool Pregnant
        {
            get { return GetValue(() => Pregnant); }
            set { SetValue(() => Pregnant, value); }
        }
        public bool ExperiencedStroke
        {
            get { return GetValue(() => ExperiencedStroke); }
            set { SetValue(() => ExperiencedStroke, value); }
        }
        public bool Epilepsy
        {
            get { return GetValue(() => Epilepsy); }
            set { SetValue(() => Epilepsy, value); }
        }
        public bool ChronicBronchitis
        {
            get { return GetValue(() => ChronicBronchitis); }
            set { SetValue(() => ChronicBronchitis, value); }
        }
        public bool CardioHistoryBefore55
        {
            get { return GetValue(() => CardioHistoryBefore55); }
            set { SetValue(() => CardioHistoryBefore55, value); }
        }
        public bool Smoking
        {
            get { return GetValue(() => Smoking); }
            set { SetValue(() => Smoking, value); }
        }

        public string MedicationTaking
        {
            get { return GetValue(() => MedicationTaking); }
            set { SetValue(() => MedicationTaking, value); }
        }

        //Fitness Goals
        public bool Health
        {
            get { return GetValue(() => Health); }
            set { SetValue(() => Health, value); }
        }
        public bool Strength
        {
            get { return GetValue(() => Strength); }
            set { SetValue(() => Strength, value); }
        }
        public bool GoodShape
        {
            get { return GetValue(() => GoodShape); }
            set { SetValue(() => GoodShape, value); }
        }
        public bool LossWeight
        {
            get { return GetValue(() => LossWeight); }
            set { SetValue(() => LossWeight, value); }
        }
        public bool GetWeight
        {
            get { return GetValue(() => GetWeight); }
            set { SetValue(() => GetWeight, value); }
        }
        public bool GoodBreathing
        {
            get { return GetValue(() => GoodBreathing); }
            set { SetValue(() => GoodBreathing, value); }
        }
        public bool Entertainment
        {
            get { return GetValue(() => Entertainment); }
            set { SetValue(() => Entertainment, value); }
        }
        public string OtherFitnessGoal
        {
            get { return GetValue(() => OtherFitnessGoal); }
            set { SetValue(() => OtherFitnessGoal, value); }
        }


    }
}