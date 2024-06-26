﻿namespace GarageThree.Web.ViewModels.Vehicle
{
    public class VehicleIndexViewModel
    {
        public string SearchTerm { get; set; } = default!;
        public int PageSize { get; set; } = default!;
        public int CurrentPage { get; set; } = default!;
        public int GarageId { get; set; } = default!;
        public MessageViewModel? Message { get; set; }
        public IEnumerable<VehicleViewModel> Vehicles { get; set; } = [];
    }
}
