using Entities.Auth;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Service.User;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ServiceManager _serviceManager;

        public UserController(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register reg)
        {
            //Проверку на правильность почты итд я не делал тк считаю что это будет лишним в данный момент

            if (reg == null)
            {
                Log.Error("Не переданы данные для регистрации.");
                return BadRequest("Не переданы данные.");
            }

            var token = await _serviceManager.Register(reg);
            if (token.Access == null || token.Refresh == null)
            {
                Log.Error("Ошибка при регистрации пользователя.");
                return StatusCode(500, "Ошибка при регистрации.");
            }

            Log.Information("Пользователь зарегистрирован успешно.");
            return Ok(token);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Authorization auth)
        {
            if (auth == null)
            {
                Log.Error("Не переданы данные для авторизации.");
                return BadRequest("Не переданы данные.");
            }

            var token = await _serviceManager.Authorization(auth);
            if (token == null)
            {
                Log.Error("Ошибка при авторизации пользователя.");
                return Unauthorized("Ошибка при авторизации.");
            }

            Log.Information("Пользователь авторизован успешно.");
            return Ok(token);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromHeader] string email, [FromHeader] string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(email))
            {
                Log.Error("Токен или email не переданы.");
                return BadRequest("Не переданы данные.");
            }

            var token = new Token { Access = accessToken };

            var result = await _serviceManager.Delete(token, id, email);
            if (!result)
            {
                Log.Error("Ошибка при удалении пользователя с id {UserId}.", id);
                return StatusCode(500, "Ошибка при удалении пользователя.");
            }

            Log.Information("Пользователь с id {UserId} удален успешно.", id);
            return Ok("Пользователь удален.");
        }
    }
}