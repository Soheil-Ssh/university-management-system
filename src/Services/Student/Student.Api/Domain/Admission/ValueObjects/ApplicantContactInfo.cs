namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record ApplicantContactInfo(MobileNumber Mobile, PhoneNumber? Phone, Email Email, Address Address);