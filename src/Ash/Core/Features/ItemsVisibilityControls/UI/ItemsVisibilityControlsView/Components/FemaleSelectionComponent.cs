using System;
using System.Linq;
using Ash.Core.SceneManagement;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class FemaleSelectionComponent
    {
        private const string ChooseFemaleSubtitle = "Select female:";

        public static void Component(Female activeFemale, Action<Female> setActiveFemale) {
            // Female selection
            Subtitle(ChooseFemaleSubtitle);
            Flow(
                SceneComponentRegistry.GetComponentsOfType<Female>().ToArray(),
                (female, idx) => RadioButton(
                    female.HeroineID.ToString(),
                    activeFemale == female,
                    () => {
                        activeFemale = female;
                        setActiveFemale(female);
                    })
            );
        }
    }
}
