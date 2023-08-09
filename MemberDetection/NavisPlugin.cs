using Autodesk.Navisworks.Api.Plugins;

namespace MemberDetection
{
    [Plugin("MemberDetection", "CONN", DisplayName = "Member Detection")]
    public class NavisPlugin : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            Screen screen = new Screen();
            screen.Show();
            return 0;
        }
    }
}
