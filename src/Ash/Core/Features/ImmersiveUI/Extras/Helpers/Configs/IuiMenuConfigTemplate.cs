using System.Reflection;
using static Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs.IuiScreenResolutionMathHelpers;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs
{
    internal class IuiMenuConfigTemplate
    {
        protected IuiMenuConfigTemplate() {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
                field.SetValue(this, GetRelativeValue(field.GetValue(this)));
        }
    }
}
