using MinimalApiAuth.Models;

namespace MinimalApiAuth.Repositories
{
    public static class UserRepository
    {
        public static User Salvar(User user)
        {
            Console.WriteLine("Salvando " + user.Username);
            var novoUser = new User(user.Id, user.Username, user.Password);
            return novoUser;
        }
    }
}
