using System.Linq;
using System.Reflection;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    public static class InGameUIStrings
    {
        // In-Game UI Button Names

        // Left-Bottom
        public const string ConfigGameObjName = "Button_Config";
        public const string EndSceneGameObjName = "Button_End";

        // Left-Middle

        // Buttons Group
        public const string LeftMiddleTogglesGameObjName = "LeftMiddleToggles";

        // Buttons
        public const string PositionGameObjName = "Styles";
        public const string ClothingGameObjName = "Wears";
        public const string GagGameObjName = "Gag";
        public const string MaleGameObjName = "MaleShow";
        public const string MapGameObjName = "Map";
        public const string MoveGameObjName = "Move";
        public const string LightingGameObjName = "Light";
        public const string EditCharGameObjName = "Customs";

        // Left-Top
        public const string SwapCharGameObjName = "Swap";

        // Right-Top
        public const string BadgesGameObjName = "Badges";
        public const string MaleGageGameObjName = "MaleGage";
        public const string FemaleGageGameObjName = "FemaleGage";
        // ReSharper disable once InconsistentNaming
        public const string MaleXTCLockGameObjName = "MaleXTCLock";
        // ReSharper disable once InconsistentNaming
        public const string FemaleXTCLockGameObjName = "FemaleXTCLock";

        // Right-Middle
        public const string TalkGameObjName = "Talk";

        // Right-Bottom
        public const string PadGameObjName = "Pad";
        public const string ActButtonsGameObjName = "ActButtons";

        public static string[] GetAllStaticStringFieldValues() {
            return typeof(InGameUIStrings)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null))
                .ToArray();
        }
    }
}
