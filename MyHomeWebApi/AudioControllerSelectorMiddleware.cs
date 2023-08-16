using MyHomeLib;

namespace MyHomeWebApi
{
    public class AudioControllerSelectorMiddleware
    {
        private RequestDelegate next;
        private IAudioManager audioManager;

        public AudioControllerSelectorMiddleware(RequestDelegate next, IMyHomeApiService myHomeApiService)
        {
            audioManager = myHomeApiService.GetAudioManager();
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, IAudioControllerSelectorService audioControllerSelector)
        {
            string? type = context.GetRouteData().Values["type"] as string;
            if (string.IsNullOrEmpty(type))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Type should be either \"input\" or \"output\"");
                return;
            }
            type = type.Trim().ToLower();
            if (type == "input")
                audioControllerSelector.AudioController = audioManager.GetAudioInputController();
            else if (type == "output")
                audioControllerSelector.AudioController = audioManager.GetAudioOutputController();

            if (audioControllerSelector.AudioController == null)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Server couldn't acquire an auido controller");
                return;
            }

            await next(context);
        }
    }
}
