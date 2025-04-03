using Core.Auth;
using Entities.Auth;
using Entities.User;
using Repositories.User;
using Serilog;


namespace Service.User
{
    public class ServiceManager
    {
        public async Task<Token> Register(Register reg)
        {
            try
            {
                int UserId = await Userdb.Register(reg.Name, reg.user_login, reg.user_password);

                if (UserId <= 0)
                {
                    Log.Error("Ошибка при регистрации пользователя: некорректный UserId для {UserLogin}", reg.user_login);
                    return null;
                }

                Log.Information("Генерация JWT ключа для пользователя {UserLogin}", reg.user_login);
                var (acc, refr) = await Jwt.GenerateKey(reg.user_login);

                if (acc == null || refr == null)
                {
                    Log.Error("Ошибка при генерации токенов для пользователя {UserLogin}", reg.user_login);
                    return null;
                }

                Log.Information("Новый пользователь с id: {UserId} зарегистрирован", UserId);
                return new Token()
                {
                    Access = acc,
                    Refresh = refr
                };
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Ошибка при регистрации пользователя");
                return null;
            }
        }

        public async Task<Token> Authorization(Authorization auth)
        {
            try
            {
                Log.Information("Попытка входа в аккаунт для {UserLogin}", auth.user_login);

                user User = await Userdb.Authorization(auth.user_login, auth.user_password);

                if (User == null)
                {
                    Log.Error("Ошибка входа в аккаунт. Пользователь с логином {UserLogin} не найден", auth.user_login);
                    return null;
                }

                var (acc, refr) = await Jwt.GenerateKey(auth.user_login);

                if (acc == null || refr == null)
                {
                    Log.Error("Ошибка при генерации токенов для пользователя {UserLogin}", auth.user_login);
                    return null;
                }

                Log.Information("Успешный вход в аккаунт пользователя {UserLogin}", auth.user_login);
                return new Token()
                {
                    Access = acc,
                    Refresh = refr
                };
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Ошибка при авторизации пользователя");
                return null;
            }
        }

        public async Task<bool> Delete(Token tok, int id, string email)
        {
            try
            {
                if (!await Jwt.CheckKey(tok.Access, email))
                {
                    Log.Warning("Не удалось проверить токен для пользователя с email {Email}", email);
                    return false;
                }

                bool isDeleted = await Userdb.Delete(id);
                if (!isDeleted)
                {
                    Log.Error("Ошибка при удалении пользователя с id {UserId} и email {Email}", id, email);
                    return false;
                }

                Log.Information("Пользователь с id {UserId} и email {Email} успешно удален", id, email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Ошибка при удалении аккаунта");
                return false;
            }
        }
    }
}