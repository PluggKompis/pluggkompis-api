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
        Open,
        Full,
        Cancelled
    }

    public enum BookingStatus
    {
        Confirmed,
        Cancelled,
        Attended
    }

    public enum VolunteerShiftStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Competed
    }

    public enum VolunteerApplicationStatus
    {
        Pending,
        Approved,
        Declined
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
