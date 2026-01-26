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
        Completed
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
        Sunday = 0,    // Was Monday = 0
        Monday = 1,    // Was Tuesday = 1
        Tuesday = 2,   // Was Wednesday = 2
        Wednesday = 3, // Was Thursday = 3
        Thursday = 4,  // Was Friday = 4
        Friday = 5,    // Was Saturday = 5
        Saturday = 6   // Was Sunday = 6
    }
}
