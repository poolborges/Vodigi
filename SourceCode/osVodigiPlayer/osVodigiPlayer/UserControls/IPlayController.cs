using System.Windows.Media;

namespace osVodigiPlayer.UserControls
{
    public interface IPlayController
    {
        void Pause();
        void ResetControl();
        void Resume();
        void Stop();
    }
}