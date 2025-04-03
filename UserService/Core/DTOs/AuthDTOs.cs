using Entities.Auth;

namespace DTOs.auth
{
    public class registerDtos
    {
        public string name { get; set; }
        public string login { get;set; }
        public string password { get; set; }
    }

    public class AuthorizationDtos
    {
        public string login { get; set; }
        public string password { get; set; }
    }

    public class DeleteDtos : Token
    {
        public string email { get; set; }
        public int id { get; set; }
    }
}