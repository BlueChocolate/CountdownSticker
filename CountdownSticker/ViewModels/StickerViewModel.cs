using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.ViewModels
{
    public partial class StickerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string? _note;

        [ObservableProperty]
        private TimeSpan _remaining;

        [ObservableProperty]
        private bool _isActive;

        public StickerViewModel(string title, string? note, TimeSpan remaining, bool isActive)
        {
            Title = title;
            Note = note;
            Remaining = remaining;
            IsActive = isActive;
        }
    }
}
