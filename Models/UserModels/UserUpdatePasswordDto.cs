namespace DistributedSystemAPI.Models.UserModels
{
    public class UserUpdatePasswordDto
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
