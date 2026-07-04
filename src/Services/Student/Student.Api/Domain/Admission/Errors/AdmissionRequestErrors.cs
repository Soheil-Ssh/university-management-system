namespace Student.Api.Domain.Admission.Errors;

public class AdmissionRequestErrors
{
        public static readonly Error NotFound = new("AdmissionRequest.NotFound", "Admission request not found.", ErrorType.NotFound);
        public static readonly Error InvalidRegistrationToken = 
            new("AdmissionRequest.InvalidRegistrationToken", "Registration token is invalid or expired.", ErrorType.Unauthorized);
        public static readonly Error CannotModifySubmittedRequest = 
            new("AdmissionRequest.CannotModifySubmittedRequest", "Admission request has been submitted and cannot be modified.", ErrorType.Conflict);
        public static readonly Error InvalidStep = 
            new("AdmissionRequest.InvalidStep", "Admission request is not at a valid step for personal information registration.", ErrorType.Conflict);

        public static readonly Error ApplicantPersonalInfoRequired = new("AdmissionRequest.ApplicantPersonalInfo.Required",
            "Applicant personal information must be completed before contact information.", ErrorType.Conflict);

        public static readonly Error ApplicantContactInfoRequired = new("AdmissionRequest.ApplicantContactInfo.Required",
            "Applicant contact information must be completed before parents information.", ErrorType.Conflict);

        public static readonly Error ParentsNationalCodesMustBeDifferent = new("AdmissionRequest.ParentsInfo.ParentsNationalCodesMustBeDifferent",
            "Father and mother national codes must be different.", ErrorType.Validation);
}