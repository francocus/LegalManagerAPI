using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly List<User> Users = new List<User>();

        [HttpPost]
        public ActionResult Create([FromBody] User user)
        {
            var objetoUser = new User();

            objetoUser.Id = user.Id;
            objetoUser.Name = user.Name;
            objetoUser.Dni = user.Dni;
            objetoUser.Email = user.Email;
            objetoUser.Password = user.Password;
            objetoUser.Role = user.Role;
            objetoUser.Active = user.Active;

            Users.Add(objetoUser);

            return Created();
        }

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            if (!Users.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(Users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById([FromRoute] int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            if (!Users.Remove(user))
            {
                return Conflict($"Problem to delete the item {id}");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<User> PartialUpdate([FromRoute] int id, [FromBody] User user)
        {
            var userFound = Users.FirstOrDefault(x => x.Id == id);

            if (userFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            userFound.Name = user.Name ?? userFound.Name;
            userFound.Dni = user.Dni ?? userFound.Dni;
            userFound.Email = user.Email ?? userFound.Email;
            userFound.Role = user.Role ?? userFound.Role;

            return Ok(userFound);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update([FromRoute] int id, [FromBody] User user)
        {
            var userFound = Users.FirstOrDefault(x => x.Id == id);

            if (userFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            userFound.Name = user.Name;
            userFound.Dni = user.Dni;
            userFound.Email = user.Email;
            userFound.Password = user.Password;
            userFound.Role = user.Role;
            userFound.Active = user.Active;

            return Ok(userFound);
        }
    }
}