using GarageThree.Persistence.Repositories;

namespace GarageThree.Web.Controllers;

public class GaragesController(IMapper mapper, IRepository<Garage> garageRepository) : Controller
{
    private readonly IMapper _mapper = mapper;
    private readonly IRepository<Garage> _garageRepository = garageRepository;

    public async Task<IActionResult> Index()
    {
        var garages = await _garageRepository.GetAll();

        GarageIndexViewModel viewModel = new()
        {
            Garages = _mapper.Map<IEnumerable<GarageViewModel>>(garages)
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var viewModel = new GarageCreateOrEditViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GarageCreateOrEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var garage = _mapper.Map<Garage>(viewModel);

        var createdGarage = await _garageRepository.Create(garage);
        if (createdGarage is not null)
        {

            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var garage = await _garageRepository.GetById(id);
        if (garage == null)
        {
            return NotFound();
        }

        var garageViewModel = new GarageViewModel
        {
            Id = garage.Id,
            Name = garage.Name,
            Capacity = garage.Capacity,
        };

        return View(garageViewModel);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var modelToEdit = await _garageRepository.GetById((int)id);
        var viewModel = _mapper.Map<GarageCreateOrEditViewModel>(modelToEdit);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GarageCreateOrEditViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        var garageToUpdate = _mapper.Map<Garage>(viewModel);

        var updatedGarage = await _garageRepository.Update(garageToUpdate);
        if (updatedGarage is not null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }


}




