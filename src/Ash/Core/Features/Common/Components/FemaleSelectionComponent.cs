using System;
using System.Linq;
using Ash.Core.SceneManagement;
using Illusion.Extensions;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;

namespace Ash.Core.Features.Common.Components
{
    internal static class FemaleSelectionComponent
    {
        private const string ChooseFemaleSubtitle = "Select female:";

        internal static void Component(Female activeFemale, Action<Female> callback) {
            // Female selection
            Subtitle(ChooseFemaleSubtitle);
            Flow(
                SceneComponentRegistry.GetComponentsOfType<Female>().ToArray(),
                (female, idx) => RadioButton(
                    female.HeroineID.ToString().ToLower().ToTitleCase(),
                    activeFemale.heroineID == female.heroineID,
                    () => callback(female))
            );
        }
    }
}
