using System;
using Memory;

namespace PBPModSwitch.ViewModels
{
    class MapFileViewModel : MapInfoViewModel
    {
        private readonly Mem _memLib;
        private readonly long _memoryPtr;

        public MapFileViewModel(string name, long memoryPtr, Mem memLib) : base(name)
        {
            _memoryPtr = memoryPtr;
            _memLib = memLib;
        }

        public override void SetIsChecked(bool? value, bool updateChildren, bool updateParent, bool notifyChange)
        {
            base.SetIsChecked(value, updateChildren, updateParent, notifyChange);

            if (base._isChecked.HasValue)
            {
                if (base._isChecked.Value)
                {
                    _memLib.WriteMemory(Convert.ToString(_memoryPtr, 16), "bytes", MainWindowViewModel.s_modsInAoB);
                } else
                {
                    _memLib.WriteMemory(Convert.ToString(_memoryPtr, 16), "bytes", MainWindowViewModel.s_mapsInAoB);
                }
            }
        }
    }
}
