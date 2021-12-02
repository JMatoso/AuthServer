using jwt_identity_api.Data;

namespace jwt_identity_api.Models.VModels
{
    public class UserVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public UserStatus UserStatus { get; set; }
        public string? FacebookId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AppleId { get; set; }
        public string? GoogleId { get; set; }
        public Guid PusherId { get; set; }
        public DateTime Created { get; set; }
    }
}