namespace Application.Children.Dtos
{
    public class ChildDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public int BirthYear { get; set; }
        public string SchoolGrade { get; set; } = default!;
    }

    public class CreateChildRequest
    {
        public string FirstName { get; set; } = default!;
        public int BirthYear { get; set; }
        public string SchoolGrade { get; set; } = default!;
    }

    public class UpdateChildRequest
    {
        public string FirstName { get; set; } = default!;
        public int BirthYear { get; set; }
        public string SchoolGrade { get; set; } = default!;
    }
}
