using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceManagment.DTO;
using SpaceManagment.Model;
using SpaceManagment.Service;

namespace SpaceManagment.Controllers
{
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            this.logger = logger;
            _userService = userService;
        }

        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<ActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.Users();
            return Ok(users);
        }

        [HttpGet("Get")]
        public async Task<ActionResult<User>> Get(string id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            return user;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<IdentityResult>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            var existUser = await _userService.Get(userDto);
            if (existUser != null)
                return BadRequest("نام کاربری تکراری است");
            var result = await _userService.AddUser(userDto);
            logger.LogInformation($"کاربر جدید  با نام کاربری {userDto.UserName} ایجاد شد");
            return result;
        }

        [HttpPut("Update")]
        public async Task<ActionResult> Update(string id, UserDto userDto, CancellationToken cancellationToken)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return BadRequest("کاربری با این مشخصات موجود نمیباشد");

            var result = await _userService.UpdateUser(user, userDto);
            logger.LogInformation($"کاربر  با نام کاربری {userDto.UserName} ویرایش شد");
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteUser(id);
            if (result.Succeeded)
                logger.LogInformation($"کاربر  با نام آیدی {id} حذف شد");
            return Ok(result);
        }
    }
}
