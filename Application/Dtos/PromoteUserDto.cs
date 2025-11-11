namespace Application.Dtos
{
    public class PromoteUserDto
    {
        public string NewRole { get; set; } = "Staff";
        public bool ReplaceExistingRoles { get; set; } = true;
    }
}
