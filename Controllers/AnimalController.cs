using Microsoft.AspNetCore.Mvc;
using ZooAnimalManagmentSystem.Data;
using ZooAnimalManagmentSystem.Models;

[ApiController]
[Route("api/animals")]
public class AnimalController : ControllerBase
{
    private readonly AnimalTransferService _animalTransferService;
    private readonly ZooContext _context;

    public AnimalController(AnimalTransferService animalTransferService, ZooContext context)
    {
        _animalTransferService = animalTransferService;
        _context = context;
    }

    [HttpPost("transfer")]
    public IActionResult TransferAnimals([FromBody] AnimalTransferRequest request)
    {
        try
        {
            var result = _animalTransferService.TransferAnimals(request.AnimalsJson, request.EnclosuresJson);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("add")]
    public IActionResult AddAnimal([FromBody] Animal animal)
    {
        try
        {
            // Add the new animal to the database
            _context.Animals.Add(animal);
            _context.SaveChanges();

            return Ok($"Animal {animal.Species} added successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetAnimal(int id)
    {
        try
        {
            // Retrieve the animal by ID
            var animal = _context.Animals.Find(id);

            if (animal == null)
                return NotFound("Animal not found");

            return Ok(animal);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPut("update/{id}")]
    public IActionResult UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
    {
        try
        {
            // Retrieve the existing animal by ID
            var existingAnimal = _context.Animals.Find(id);

            if (existingAnimal == null)
                return NotFound("Animal not found");

            // Update properties of the existing animal
            existingAnimal.Species = updatedAnimal.Species;
            existingAnimal.Food = updatedAnimal.Food;
            existingAnimal.Amount = updatedAnimal.Amount;

            _context.SaveChanges();

            return Ok($"Animal {existingAnimal.Species} updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpDelete("remove/{id}")]
    public IActionResult RemoveAnimal(int id)
    {
        try
        {
            // Find the animal by ID
            var animal = _context.Animals.Find(id);

            if (animal == null)
                return NotFound("Animal not found");

            // Remove the animal from the database
            _context.Animals.Remove(animal);
            _context.SaveChanges();

            return Ok($"Animal {animal.Species} removed successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    public class AnimalTransferRequest
    {
        public string AnimalsJson { get; set; }
        public string EnclosuresJson { get; set; }
    }
}
