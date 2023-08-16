using MyHomeLib;

namespace MyHomeWebApi
{
    public interface IAudioControllerSelectorService
    {
        public IAudioController? AudioController { get; set; }
    }

    public class AudioControllerSelectorService : IAudioControllerSelectorService
    {
        public IAudioController? AudioController { get; set; }
    }
}
