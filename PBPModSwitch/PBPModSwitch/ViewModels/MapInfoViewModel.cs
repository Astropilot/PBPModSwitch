using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PBPModSwitch.ViewModels
{
    class MapInfoViewModel : INotifyPropertyChanged
    {
        protected bool? _isChecked = false;
        protected MapInfoViewModel _parent;

        public MapInfoViewModel(string name)
        {
            Name = name;
            Children = new List<MapInfoViewModel>();
        }

        public void Initialize()
        {
            foreach (MapInfoViewModel child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public List<MapInfoViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public event EventHandler CheckedStateChanged;


        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true, true); }
        }

        public virtual void SetIsChecked(bool? value, bool updateChildren, bool updateParent, bool notifyChange)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                Children.ForEach(c => c.SetIsChecked(_isChecked, true, false, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            OnPropertyChanged("IsChecked");

            if (notifyChange)
                CheckedStateChanged?.Invoke(this, EventArgs.Empty);
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true, false);
        }

        void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
