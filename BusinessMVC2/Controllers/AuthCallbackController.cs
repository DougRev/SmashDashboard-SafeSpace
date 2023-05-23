using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v3;
using Google.Apis.Sample.MVC4;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Mvc;
using Google.Apis.Sheets.v4;

namespace Google.Apis.Sample.MVC4.Controllers
{
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
        {
            get { return new AppFlowMetadata(); }
        }
    }
}
