
namespace PBPModSwitch.ViewModels
{
    public class WaitingStatus : ViewModel
    {
        private bool _isWaiting;
        public bool isWaiting
        {
            get => _isWaiting;
            set => Set(ref _isWaiting, value);
        }

        private bool _showProgress;
        public bool showProgress
        {
            get => _showProgress;
            set => Set(ref _showProgress, value);
        }

        private int _progressValue;
        public int progressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private int _progressMax;
        public int progressMax
        {
            get => _progressMax;
            set => Set(ref _progressMax, value);
        }

        private string _waitingMessage;
        public string waitingMessage
        {
            get => _waitingMessage;
            set => Set(ref _waitingMessage, value);
        }

        public WaitingStatus(bool waiting, string message)
        {
            isWaiting = waiting;
            waitingMessage = message;
            _showProgress = false;
        }
    }
}
