using UnityEngine;
using UnityEngine.Rendering;

namespace Ash.Core.Features.ImmersiveUI
{
    [RequireComponent(typeof(Camera))]
    internal class CommandBufferBlur : MonoBehaviour
    {
        private static int Offsets;

        private Shader Shader;

        private Material Material;

        private Camera Camera;
        private CommandBuffer CommandBuffer;

        private Vector2 ScreenResolution = Vector2.zero;
        private const RenderTextureFormat TextureFormat = RenderTextureFormat.ARGB32;

        private bool Initialized => CommandBuffer != null;

        private static readonly int GrabSharpTexture = Shader.PropertyToID("_GrabSharpTexture");
        private static readonly int BlurSpreadId = Shader.PropertyToID("_BlurSpread");

        private void OnEnable() {
            Cleanup();
            Initialize();
        }

        private void OnDisable() {
            Cleanup();
        }

        private void OnDestroy() {
            Cleanup();
        }

        private void OnPreRender() {
            if (ScreenResolution != new Vector2(Screen.width, Screen.height))
                Cleanup();

            Initialize();
        }

        private void Cleanup() {
            if (!Initialized)
                return;

            Camera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, CommandBuffer);
            CommandBuffer = null;
            DestroyImmediate(Material);
        }

        private void Initialize() {
            if (!IuiMain.IsLegalScene)
                return;

            if (Initialized)
                return;

            if (Offsets == 0)
                Offsets = Shader.PropertyToID("offsets");

            if (!Shader)
                Shader = Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/separableblur.shader");

            if (!Material) {
                Material = new Material(Shader) {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            Material.SetFloat(BlurSpreadId, IuiSettings.BlurSpread);

            Camera = GetComponent<Camera>();

            CommandBuffer = new CommandBuffer();
            CommandBuffer.name = "Blur screen";

            // preserve original RT
            CommandBuffer.GetTemporaryRT(GrabSharpTexture, -1, -1, 0, FilterMode.Bilinear, TextureFormat);
            CommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, GrabSharpTexture);
            CommandBuffer.SetGlobalTexture(GrabSharpTexture, GrabSharpTexture);

            Vector2[] sizes = {
                new Vector2(Screen.width, Screen.height),
                new Vector2((float)Screen.width / 2, (float)Screen.height / 2),
                new Vector2((float)Screen.width / 4, (float)Screen.height / 4),
                new Vector2((float)Screen.width / 8, (float)Screen.height / 8),
                new Vector2((float)Screen.width / 16, (float)Screen.height / 16),
            };

            for (var i = 0; i < sizes.Length; ++i) {
                var screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
                CommandBuffer.GetTemporaryRT(screenCopyID, -1, -1, 0, FilterMode.Bilinear, TextureFormat);
                CommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

                var blurredID = Shader.PropertyToID("_Grab" + i + "_Temp1");
                var blurredID2 = Shader.PropertyToID("_Grab" + i + "_Temp2");
                CommandBuffer.GetTemporaryRT(blurredID, (int)sizes[i].x, (int)sizes[i].y, 0, FilterMode.Bilinear,
                    TextureFormat);
                CommandBuffer.GetTemporaryRT(blurredID2, (int)sizes[i].x, (int)sizes[i].y, 0, FilterMode.Bilinear,
                    TextureFormat);

                CommandBuffer.Blit(screenCopyID, blurredID);
                CommandBuffer.ReleaseTemporaryRT(screenCopyID);

                CommandBuffer.SetGlobalVector(Offsets, new Vector4(IuiSettings.BlurSpread / sizes[i].x, 0, 0, 0));
                CommandBuffer.Blit(blurredID, blurredID2, Material);
                CommandBuffer.SetGlobalVector(Offsets, new Vector4(0, IuiSettings.BlurSpread / sizes[i].y, 0, 0));
                CommandBuffer.Blit(blurredID2, blurredID, Material);

                CommandBuffer.SetGlobalTexture("_GrabBlurTexture_" + i, blurredID);
            }

            Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, CommandBuffer);

            ScreenResolution = new Vector2(Screen.width, Screen.height);
        }
    }
}
