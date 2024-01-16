using Newtonsoft.Json;
using ZooAnimalManagmentSystem.Data;
using ZooAnimalManagmentSystem.Models;

public class AnimalTransferService
{
    private readonly ZooContext _context;

    public AnimalTransferService(ZooContext context)
    {
        _context = context;
    }

    public List<Animal> TransferAnimals(string animalsJsonPath, string enclosuresJsonPath)
    {
        try
        {
            var animals = LoadAnimalsFromJson(animalsJsonPath);
            var enclosures = LoadEnclosuresFromJson(enclosuresJsonPath);

            // Assign vegetarian animals to the same enclosure
            var vegetarianAnimals = animals.Where(a => a.Food == "Herbivore").ToList();
            AssignAnimalsToEnclosures(vegetarianAnimals, enclosures);

            // Assign meat-eating animals to the same enclosure based on rules
            var meatEatingAnimals = animals.Where(a => a.Food == "Carnivore").ToList();
            AssignMeatEatingAnimalsToEnclosures(meatEatingAnimals, enclosures);

            // Save changes to the database
            _context.SaveChanges();

            return animals;
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed
            throw new ApplicationException($"Error transferring animals: {ex.Message}");
        }
    }

    private List<Animal> LoadAnimalsFromJson(string animalsJsonPath)
    {
        try
        {
            var json = File.ReadAllText(animalsJsonPath);
            return JsonConvert.DeserializeObject<List<Animal>>(json);
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed
            throw new ApplicationException($"Error loading animals from JSON: {ex.Message}");
        }
    }

    private List<Enclosure> LoadEnclosuresFromJson(string enclosuresJsonPath)
    {
        try
        {
            var json = File.ReadAllText(enclosuresJsonPath);
            return JsonConvert.DeserializeObject<List<Enclosure>>(json);
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed
            throw new ApplicationException($"Error loading enclosures from JSON: {ex.Message}");
        }
    }

    private void AssignAnimalsToEnclosures(List<Animal> animals, List<Enclosure> enclosures)
    {
        foreach (var animal in animals)
        {
            var suitableEnclosure = enclosures.FirstOrDefault(e => e.Size == "Large" && e.Location == "Outside" && e.Animals.All(a => a.Food == "Herbivore"));

            if (suitableEnclosure == null)
            {
                throw new ApplicationException($"No suitable enclosure found for {animal.Species}");
            }

            animal.Enclosure = suitableEnclosure;
            suitableEnclosure.Animals.Add(animal);
        }
    }

    private void AssignMeatEatingAnimalsToEnclosures(List<Animal> meatEatingAnimals, List<Enclosure> enclosures)
    {
        foreach (var animal in meatEatingAnimals)
        {
            var suitableEnclosure = enclosures.FirstOrDefault(e => e.Size == "Large" && e.Location == "Outside" && e.Animals.All(a => a.Food == "Carnivore") && e.Animals.Count < 2);

            if (suitableEnclosure == null)
            {
                throw new ApplicationException($"No suitable enclosure found for {animal.Species}");
            }

            animal.Enclosure = suitableEnclosure;
            suitableEnclosure.Animals.Add(animal);
        }
    }
}
