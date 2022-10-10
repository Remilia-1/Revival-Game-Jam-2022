using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Project.PostProcessing
{
    [System.Serializable]
    public class PixelatePass : ScriptableRenderPass
    {
        private RenderTargetIdentifier m_Src;
        private RenderTargetIdentifier m_Dst;

        private readonly int temporaryRenderTextureID = Shader.PropertyToID("_TempRT");
        private readonly int m_SizeID = Shader.PropertyToID("Size");



        public PixelatePass()
        {
            // Set the render pass event
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Grab the camera target descriptor. We will use this when creating a temporary render texture.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            var renderer = renderingData.cameraData.renderer;
            m_Src = renderer.cameraColorTarget;

            // Create a temporary render texture using the descriptor from above.
            cmd.GetTemporaryRT(temporaryRenderTextureID, descriptor, FilterMode.Point);
            m_Dst = new RenderTargetIdentifier(temporaryRenderTextureID);
        }

        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Skipping post processing rendering inside the scene View
            if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Pixelization Post Process");
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<PixelateComponent>();

            if (customEffect.IsActive())
            {
                var material = new Material(Shader.Find("Custom/S_PixelatePostProcess"));

                material.SetFloat(m_SizeID, customEffect.size.value);

                Blit(cmd, m_Src, m_Dst, material);
                Blit(cmd, m_Dst, m_Src);

                context.ExecuteCommandBuffer(cmd);
            }

            CommandBufferPool.Release(cmd);
        }

        //Cleans the temporary RTs when we don't need them anymore
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryRenderTextureID);
        }
    }
}