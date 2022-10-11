using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePostProcessPass : ScriptableRenderPass
{
    private ProfilingSampler m_Sampler;
    private RenderTargetIdentifier m_PostProcessTexture;
    private readonly int temporaryRTIdA = Shader.PropertyToID("_TempRT_Outline");
    private readonly int m_SizeID = Shader.PropertyToID("_Size");
    private readonly int m_OffsetID = Shader.PropertyToID("_Offset");
    private readonly int m_OutlineColorID = Shader.PropertyToID("_OutlineColor");

    private List<ShaderTagId> ShaderTagIdList = new List<ShaderTagId>();
    private RenderTargetIdentifier m_CameraTargetId;



    public OutlinePostProcessPass (string[] shaderTags, RenderPassEvent renderPassEvent)
    {
        this.renderPassEvent = renderPassEvent;
        m_Sampler = new ProfilingSampler("OutlinePass");

        if (shaderTags != null && shaderTags.Length > 0)
        {
            foreach (var passName in shaderTags)
                ShaderTagIdList.Add(new ShaderTagId(passName));
        }
        else
        {
            ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
            ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
        }
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        var renderer = renderingData.cameraData.renderer;

        cmd.GetTemporaryRT(temporaryRTIdA, cameraTargetDescriptor, FilterMode.Point);
        m_PostProcessTexture = new RenderTargetIdentifier(temporaryRTIdA);

        m_CameraTargetId = renderer.cameraColorTarget;
        ConfigureTarget(m_PostProcessTexture);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("OutlinePass");
        var stack = VolumeManager.instance.stack;
        var customEffect = stack.GetComponent<OutlineComponent>();

        using (new ProfilingScope(cmd, m_Sampler))
        {
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var targetMaterial = new Material(Shader.Find("Custom/PostProcessing/S_Outline"));

            targetMaterial.SetFloat(m_SizeID, customEffect.Size.value);
            targetMaterial.SetFloat(m_OffsetID, customEffect.Offset.value);
            targetMaterial.SetColor(m_OutlineColorID, customEffect.Color.value);

            Blit(cmd, m_CameraTargetId, m_PostProcessTexture, targetMaterial);
            Blit(cmd, m_PostProcessTexture, m_CameraTargetId);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTIdA);
    }
}
