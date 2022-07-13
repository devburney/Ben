
namespace TBS.PrintTest.Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool IsCustomError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
