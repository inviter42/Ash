using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class CommandBufferBlur : MonoBehaviour
{
    private static int Offsets;
    private Shader Shader;

    private Material Material;

    private Camera Camera;
    private CommandBuffer CommandBuffer;

    private Vector2 ScreenResolution = Vector2.zero;
    private RenderTextureFormat TextureFormat = RenderTextureFormat.ARGB32;

    private bool initialized
    {
        get { return CommandBuffer != null; }
    }

    public void OnEnable()
    {
        Cleanup();
        Initialize();
    }

    public void OnDisable()
    {
        Cleanup();
    }


    private void Cleanup()
    {
        if (!initialized)
            return;

        Camera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, CommandBuffer);
        CommandBuffer = null;
        DestroyImmediate(Material);
    }

    private void Initialize()
    {
        if (initialized)
            return;

        if (Offsets == 0)
            Offsets = Shader.PropertyToID("offsets");

        if (!Shader)
        {
            Shader = Shader.Find("Hidden/SeparableGlassBlur");

            if (!Shader)
                throw new MissingReferenceException("Unable to find required shader 'Hidden/SeparableGlassBlur'");
        }

        if (!Material)
        {
            Material = new Material(Shader) {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        Camera = GetComponent<Camera>();

        CommandBuffer = new CommandBuffer();
        CommandBuffer.name = "Blur screen";

        const int numIterations = 4;

        Vector2[] sizes = {
            new Vector2(Screen.width, Screen.height),
            new Vector2((float)Screen.width / 2, (float)Screen.height / 2),
            new Vector2((float)Screen.width / 4, (float)Screen.height / 4),
            new Vector2((float)Screen.width / 8, (float)Screen.height / 8),
        };

        for (var i = 0; i < numIterations; ++i)
        {
            var screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            CommandBuffer.GetTemporaryRT(screenCopyID, -1, -1, 0, FilterMode.Bilinear, TextureFormat);
            CommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

            var blurredID = Shader.PropertyToID("_Grab" + i + "_Temp1");
            var blurredID2 = Shader.PropertyToID("_Grab" + i + "_Temp2");
            CommandBuffer.GetTemporaryRT(blurredID, (int)sizes[i].x, (int)sizes[i].y, 0, FilterMode.Bilinear, TextureFormat);
            CommandBuffer.GetTemporaryRT(blurredID2, (int)sizes[i].x, (int)sizes[i].y, 0, FilterMode.Bilinear, TextureFormat);

            CommandBuffer.Blit(screenCopyID, blurredID);
            CommandBuffer.ReleaseTemporaryRT(screenCopyID);

            CommandBuffer.SetGlobalVector(Offsets, new Vector4(2.0f / sizes[i].x, 0, 0, 0));
            CommandBuffer.Blit(blurredID, blurredID2, Material);
            CommandBuffer.SetGlobalVector(Offsets, new Vector4(0, 2.0f / sizes[i].y, 0, 0));
            CommandBuffer.Blit(blurredID2, blurredID, Material);

            CommandBuffer.SetGlobalTexture("_GrabBlurTexture_" + i, blurredID);
        }

        Camera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, CommandBuffer);

        ScreenResolution = new Vector2(Screen.width, Screen.height);
    }

    private void OnPreRender()
    {
        if (ScreenResolution != new Vector2(Screen.width, Screen.height))
            Cleanup();

        Initialize();
    }
}
