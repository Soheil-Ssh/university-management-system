namespace Student.Api.Domain.Admission.Enums;

public enum AdmissionRequestStep
{
    NotStarted = 0,
    PersonalInfoCompleted = 1,
    ContactInfoCompleted = 2,
    ParentsInfoCompleted = 3,
    EmergencyContactInfoCompleted = 4,
    DiplomaInfoCompleted = 5,
    EntranceInfoCompleted = 6,
    AttachmentsCompleted = 7,
    Completed = 8
}