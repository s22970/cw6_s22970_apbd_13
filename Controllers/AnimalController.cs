using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AnimalsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals(string orderBy = "name")
        {
            string connectionString = _configuration.GetConnectionString("AnimalsDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM Animals ORDER BY {orderBy}", connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<Animal> animals = new List<Animal>();
                    while (await reader.ReadAsync())
                    {
                        animals.Add(new Animal
                        {
                            IdAnimal = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Category = reader.GetString(3),
                            Area = reader.GetString(4)
                        });
                    }
                    return Ok(animals);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddAnimal([FromBody] Animal animal)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string connectionString = _configuration.GetConnectionString("AnimalsDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO Animals (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)", connection))
                {
                    command.Parameters.AddWithValue("@Name", animal.Name);
                    command.Parameters.AddWithValue("@Description", animal.Description);
                    command.Parameters.AddWithValue("@Category", animal.Category);
                    command.Parameters.AddWithValue("@Area", animal.Area);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }

        [HttpPut("{idAnimal}")]
        public async Task<ActionResult> UpdateAnimal(int idAnimal, [FromBody] Animal animal)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string connectionString = _configuration.GetConnectionString("AnimalsDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal", connection))
                {
                    command.Parameters.AddWithValue("@Name", animal.Name);
                    command.Parameters.AddWithValue("@Description", animal.Description);
                    command.Parameters.AddWithValue("@Category", animal.Category);
                    command.Parameters.AddWithValue("@Area", animal.Area);
                    command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{idAnimal}")]
        public async Task<ActionResult> DeleteAnimal(int idAnimal)
        {
            string connectionString = _configuration.GetConnectionString("AnimalsDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("DELETE FROM Animals WHERE IdAnimal = @IdAnimal", connection))
                {
                    command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        return NotFound();
                }
            }
            return Ok();
        }
    }

    public class Animal
    {
        public int IdAnimal { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Area { get; set; }
    }
}
