using System;
using System.Linq;
using Ash.Core.SceneManagement;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.Common.Components
{
    public static class FemaleSelectionComponent
    {
        private const string ChooseFemaleSubtitle = "Select female:";

        public static void Component(Female activeFemale, Action<Female> callback) {
            // Female selection
            Subtitle(ChooseFemaleSubtitle);
            Flow(
                SceneComponentRegistry.GetComponentsOfType<Female>().ToArray(),
                (female, idx) => RadioButton(
                    female.HeroineID.ToString(),
                    activeFemale.heroineID == female.heroineID,
                    () => callback(female))
            );
        }
    }
}
