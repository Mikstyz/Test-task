namespace DTOs.auth
{
    public class register
    {
        public string name { get; set; }
        public string login { get;set; }
        public string password { get; set; }
    }

    public class Authorization
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}