using Rise.Data.ViewModels;

namespace Rise.App.ViewModels
{
    public sealed class DeviceViewModel : ViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Online { get; set; }

    }
}
