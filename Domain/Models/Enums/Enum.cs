namespace Domain.Models.Enums
{
    public enum UserRole
    {
        Coordinator,
        Volunteer,
        Parent,
        Student
    }

    public enum TimeSlotStatus
    {
        Draft,
        Open,
        Cancelled,
        Completed
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        NoShow
    }

    public enum VolunteerShiftStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public enum VolunteerApplicationStatus
    {
        Pending = 1,
        Approved = 2,
        Declined = 3
    }

    public enum ConfidenceLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public enum WeekDay
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
