using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHomeLib;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyHomeWebApi.Controllers
{
    public class AudioDeviceModel
    {
        public AudioDeviceModel() : this("", "", 0.0f, false) { }
        public AudioDeviceModel(IAudioDevice device) : this(device.GetName(), device.GetId(), device.VolumeLevel, device.IsMuted())
        {
        }

        public AudioDeviceModel(string name, string id, float volumeLevel, bool isMuted)
        {
            Name = name;
            Id = id;
            VolumeLevel = volumeLevel;
            IsMuted = isMuted;
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public float VolumeLevel { get; set; }

        public bool IsMuted { get; set; }
    }

    [Route("api/[controller]/[action]/{type}")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private IAudioController audioController;
        public AudioController(IAudioControllerSelectorService audioControllerSelector)
        {
            if (audioControllerSelector.AudioController == null)
                throw new ArgumentNullException("Audio controller is null");

            audioController = audioControllerSelector.AudioController;
        }

        [HttpPut]
        public IActionResult UpdateDevice([FromBody] AudioDeviceModel deviceModel)
        {
            if (deviceModel.VolumeLevel < 0 || deviceModel.VolumeLevel > 1.0f)
                return BadRequest("Volume level should be in between [0, 1]");

            try
            {
                IAudioDevice audioDevice;
                audioDevice = audioController.GetAudioDevice(deviceModel.Id);
                if (audioDevice.IsMuted() != deviceModel.IsMuted)
                    audioDevice.ToggleMute();
                else if(deviceModel.IsMuted && audioDevice.VolumeLevel != deviceModel.VolumeLevel)
                    audioDevice.ToggleMute();

                audioDevice.VolumeLevel = deviceModel.VolumeLevel;
                return Ok(JsonSerializer.Serialize(new AudioDeviceModel(audioDevice)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet]
        public IActionResult GetAllAudioDevices()
        {
            return Ok(audioController.EnumerateAudioDevices().Select(raw => new AudioDeviceModel(raw)));
        }

        [HttpPost]
        public IActionResult ActivateAudioDevice([FromBody]string deviceId)
        {
            var activeDevice = audioController.ActivateDevice(deviceId);
            return Ok(JsonSerializer.Serialize(new AudioDeviceModel(activeDevice)));
        }

        [HttpGet]
        public IActionResult GetActiveAudioDevice()
        {
            var activeDevice = audioController.GetActiveAudioDevice();
            return Ok(JsonSerializer.Serialize(new AudioDeviceModel(activeDevice)));
        }
    }
}
