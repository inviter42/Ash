using System;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Wears;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Character;
using Illusion.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components
{
    internal class WearAndAccessoryButtonsContainer: IDisposable
    {
        internal readonly GameObject Root;
        internal readonly HEROINE HeroineId;

        internal WearButtonsContainer WearButtonsContainer { get; set; }
        internal InteractableRing InteractableRing { get; set; }

        internal WearAndAccessoryButtonsContainer(
            WearsMenuMain menu,
            WearsMenuConfig config,
            WearsMenuTextures textures,
            Material iconMaskMaterial,
            GameObject root,
            GameObject frostedGlassGameObject,
            Female female
        ) {
            HeroineId = female.HeroineID;

            Root = new GameObject(HeroineId.ToString().ToLower().ToTitleCase(), typeof(RectTransform));
            Root.transform.SetParent(root.transform, false);

            WearButtonsContainer = new WearButtonsContainer(
                config,
                textures,
                iconMaskMaterial,
                Root,
                female
            );

            InteractableRing = new InteractableRing(
                config,
                textures,
                iconMaskMaterial,
                Root,
                female
            );

            if (InteractableRing.AccessoryContainer == null)
                return;

            ExpandAccessoriesButton.CreateExpandAccessoriesButton(
                menu,
                config,
                textures,
                Root,
                InteractableRing.AccessoryContainer,
                frostedGlassGameObject
            );
        }

        public void Dispose() {
            Object.DestroyImmediate(Root);
            GC.SuppressFinalize(this);
        }
    }
}
