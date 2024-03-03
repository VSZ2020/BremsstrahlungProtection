namespace BSP.Common
{
    public abstract class BaseNotificationViewModel : BaseValidationViewModel
    {
        private string messageTitle;
        private string messageContent;
        private bool isStatusMessageVisible = false;

        public string MessageTitle { get => messageTitle; set { messageTitle = value; OnChanged(); } }
        public string MessageContent { get => messageContent; set { messageContent = value; OnChanged(); } }

        public bool IsStatusMessageVisible { get => isStatusMessageVisible; set { isStatusMessageVisible = value; OnChanged(); } }

        public void ShowStatusMessage(string message, string title = null)
        {
            if (!string.IsNullOrEmpty(title))
                MessageTitle = title;
            MessageContent = message;
            IsStatusMessageVisible = true;
        }

        public void HideStatusMessage()
        {
            MessageTitle = "";
            MessageContent = "";
            IsStatusMessageVisible = false;
        }
    }
}
