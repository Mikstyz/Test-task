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
                    Log.Error("������ ��� ����������� ������������: ������������ UserId ��� {UserLogin}", reg.user_login);
                    return null;
                }

                Log.Information("��������� JWT ����� ��� ������������ {UserLogin}", reg.user_login);
                var (acc, refr) = await Jwt.GenerateKey(reg.user_login);

                if (acc == null || refr == null)
                {
                    Log.Error("������ ��� ��������� ������� ��� ������������ {UserLogin}", reg.user_login);
                    return null;
                }

                Log.Information("����� ������������ � id: {UserId} ���������������", UserId);
                return new Token()
                {
                    Access = acc,
                    Refresh = refr
                };
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "������ ��� ����������� ������������");
                return null;
            }
        }

        public async Task<Token> Authorization(Authorization auth)
        {
            try
            {
                Log.Information("������� ����� � ������� ��� {UserLogin}", auth.user_login);

                user User = await Userdb.Authorization(auth.user_login, auth.user_password);

                if (User == null)
                {
                    Log.Error("������ ����� � �������. ������������ � ������� {UserLogin} �� ������", auth.user_login);
                    return null;
                }

                var (acc, refr) = await Jwt.GenerateKey(auth.user_login);

                if (acc == null || refr == null)
                {
                    Log.Error("������ ��� ��������� ������� ��� ������������ {UserLogin}", auth.user_login);
                    return null;
                }

                Log.Information("�������� ���� � ������� ������������ {UserLogin}", auth.user_login);
                return new Token()
                {
                    Access = acc,
                    Refresh = refr
                };
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "������ ��� ����������� ������������");
                return null;
            }
        }

        public async Task<bool> Delete(Token tok, int id, string email)
        {
            try
            {
                if (!await Jwt.CheckKey(tok.Access, email))
                {
                    Log.Warning("�� ������� ��������� ����� ��� ������������ � email {Email}", email);
                    return false;
                }

                bool isDeleted = await Userdb.Delete(id);
                if (!isDeleted)
                {
                    Log.Error("������ ��� �������� ������������ � id {UserId} � email {Email}", id, email);
                    return false;
                }

                Log.Information("������������ � id {UserId} � email {Email} ������� ������", id, email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "������ ��� �������� ��������");
                return false;
            }
        }
    }
}