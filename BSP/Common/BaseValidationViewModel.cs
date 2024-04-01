using System.Text;

namespace BSP.Common
{
    public class BaseValidationViewModel : BaseViewModel
    {
        public BaseValidationViewModel()
        {
            _messages = new List<ValidationMessage>();
        }

        private List<ValidationMessage> _messages;

        public void AddError(string message, string title = null)
        {
            _messages.Add(new ValidationMessage() { Message = message, Title = string.IsNullOrEmpty(title) ? "" : title });
        }

        public bool IsValid => _messages.Count == 0;
        public bool HasErrors => _messages.Count > 0;

        public void ClearValidationMessages() => _messages.Clear();

        public string ValidationMessagesToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var msg in _messages)
            {
                builder.AppendLine("- " + (!string.IsNullOrEmpty(msg.Title) ? msg.Title + ": " : "") + msg.Message);
            }
            return builder.ToString();
        }
    }
}
