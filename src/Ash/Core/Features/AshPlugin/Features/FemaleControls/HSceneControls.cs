using System.Linq;
using Ash.Core.SceneManagement;
using Character;

namespace Ash.Core.Features.HSceneControls
{
    public static class HSceneControls
    {
        public static float GetFemaleVolumeModifier(HEROINE heroineID) {
            if (!Ash.Settings.ShouldMuteBackgroundFemale.Value)
                return 1f;

            var bgFemale = SceneComponentRegistry.GetComponentsOfType<Female>()
                .ToArray()
                // active female will have H_Pos parent, while inactive will have null
                .First(female => !female.gameObject.transform.parent);

            return bgFemale.heroineID == heroineID
                ? 0f
                : 1f;
        }
    }
}
