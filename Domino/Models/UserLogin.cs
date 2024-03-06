namespace Domino.Models
{
    public class UserLogin
    {
        public string IdUser { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public static UserLogin UserBD() { 
            UserLogin user = new UserLogin();
            user.IdUser = "1";
            user.UserName = "Inalambria";
            user.Password = "password";
            return user;
        }
    }
}
