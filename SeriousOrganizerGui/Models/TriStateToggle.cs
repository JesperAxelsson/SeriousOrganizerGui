using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SeriousOrganizerGui.Models
{
    public enum TriState : byte
    {
        Neutral = 0,
        Selected = 1,
        UnSelected = 2,
    }

    public class TriStateToggle : INotifyPropertyChanged
    {


        private Dto.Label _inner;
        private TriState _state;

        public event PropertyChangedEventHandler PropertyChanged;

        public TriStateToggle(Dto.Label inner)
        {
            _state = TriState.Neutral;
            _inner = inner;
        }

        public static TriStateToggle Create(Dto.Label inner) => new TriStateToggle(inner);

        public TriState State
        {
            get => _state;
            set
            {
                _state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("State"));
            }
        }

        public int Id { get => _inner.Id; }
        public string Name { get => _inner.Name; }
    }
}
