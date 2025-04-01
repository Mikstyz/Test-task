
namespace Service.Auth
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
}