using AspNetCoreHero.ToastNotification.Abstractions;

namespace WebUI.DTOs
{
    public class PopNotification
    {
        private readonly INotyfService notyfService;

        public PopNotification(INotyfService _notyfService)
        {
            notyfService = _notyfService;
        }
        public PopNotification()
        {

        }

        public void Notyf(string message)
        {
            if (message != null)
            {
                if (message.ToLower().Contains("success"))
                {
                    notyfService.Success(message, 10);
                }
                else if (message.ToLower().Contains("error"))
                {
                    notyfService.Error(message, 10);
                }
                else
                {
                    notyfService.Information(message, 10);
                }
            }
        }
    }
}