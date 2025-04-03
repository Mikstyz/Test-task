namespace Entities.Auth
{
    public class Token //Jwt
    {
        public string Access { get; set; }
        public string Refresh { get; set; }
    }

    public class auth
    {
        public Token token { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
    }



    public class Register
    {
        public string Name { get; set; }
        public string user_login { get; set; }
        public string user_password { get; set; }
    }

    public class Authorization
    {
        public string user_login { get; set; }
        public string user_password { get; set; }
    }
}