using System.Linq;
using Character;

namespace Ash.Core.SceneManagement
{
    internal static class SceneDataModifiers
    {
        internal static float GetFemaleVolumeModifier(HEROINE heroineID) {
            if (!Ash.PersistentSettings.ShouldMuteBackgroundFemale.Value)
                return 1f;

            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null)
                return 1f;

            var bgFemale = SceneComponentRegistry.GetComponentsOfType<Female>()
                // active female will have H_Pos parent, while inactive will have null
                .FirstOrDefault(female => female.gameObject.transform.parent == null);

            return bgFemale != null && bgFemale.heroineID == heroineID
                ? 0f
                : 1f;
        }
    }
}
