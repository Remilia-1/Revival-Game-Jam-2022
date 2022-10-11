using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlineWritePass : ScriptableRenderPass
{
    private RenderPassEvent m_TargetPass = RenderPassEvent.AfterRenderingOpaques;

    private FilteringSettings m_FilteringSettings;
    private FilteringSettings m_OccluderSettings;
    private RenderStateBlock m_RenderStateBlock;
    private RenderTargetIdentifier m_TemporaryTextureIdentifier;
    private ProfilingSampler m_Sampler;

    private readonly List<ShaderTagId> m_ShaderTagIdList;
    private readonly int m_TextureID = Shader.PropertyToID("_OutlineMaskTexture");



    public OutlineWritePass(
        LayerMask charactersMask, 
        LayerMask occluderMask,
        RenderPassEvent targetPass)
    {
        this.m_TargetPass = targetPass;

        m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, charactersMask);
        m_OccluderSettings = new FilteringSettings(RenderQueueRange.opaque, occluderMask);
        m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        renderPassEvent = m_TargetPass;
        m_Sampler = new ProfilingSampler("OutlinePassWrite");

        m_ShaderTagIdList = new List<ShaderTagId> 
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward"),
            new ShaderTagId("SRPDefaultUnlit")
        };
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        // get temporary texture to write to
        cmd.GetTemporaryRT(m_TextureID, descriptor, FilterMode.Point);
        m_TemporaryTextureIdentifier = new RenderTargetIdentifier(m_TextureID);

        // set render texture as target
        ConfigureTarget(m_TemporaryTextureIdentifier);
        ConfigureClear(ClearFlag.All, Color.clear);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("OutlinePassWrite");

        using (new ProfilingScope(cmd, m_Sampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            DrawingSettings drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.overrideMaterial = new Material(Shader.Find("Custom/VFX/WriteDepth/S_WriteDepth"));

            DrawingSettings occluderSettings = drawSettings;
            occluderSettings.overrideMaterial = new Material(Shader.Find("Custom/VFX/S_WriteOcclude/S_WriteOcclude"));

            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
            context.DrawRenderers(renderingData.cullResults, ref occluderSettings, ref m_OccluderSettings, ref m_RenderStateBlock);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(m_TextureID);
    }
}
