namespace Faculty.Api.Domain.Professor.Errors;

public class ProfessorCodeErrors
{

    public static readonly Error Empty = new("ProfessorCode.Empty", "Professor code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = 
        new("ProfessorCode.InvalidFormat", "Professor code format is invalid. Expected format is UMS_FAC_PROF_000001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("ProfessorCode.NumberOutOfRange", "Professor code number must be between 1 and 999999.", ErrorType.Validation);
}