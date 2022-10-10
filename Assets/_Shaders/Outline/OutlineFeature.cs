using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    [SerializeField] private LayerMask m_CharactersMask;
    [SerializeField] private LayerMask m_OccluderMask;
    [SerializeField] private Material m_TargetMaterial;
    [SerializeField] private Material m_OutlineWriteMaterial;
    [SerializeField] private Material m_OutlineOccludeMaterial;
    [SerializeField] private RenderPassEvent m_OutlineWritePassEvent = RenderPassEvent.AfterRenderingOpaques;
    [SerializeField] private RenderPassEvent m_OutlinePostProcessPassEvent = RenderPassEvent.AfterRenderingOpaques;
    [SerializeField] private RenderQueueType m_QueueType = RenderQueueType.Opaque;
    [SerializeField] PerObjectData m_PerObjectData;
    [SerializeField] private string[] m_ShaderTags = new string[] { "SRPDefaultUnlit", "UniversalForward", "UniversalForwardOnly", "LightweightForward" };

    private OutlineWritePass m_OutlineWritePass;
    private OutlinePostProcessPass m_PostProcessPass;



    public override void Create()
    {
        m_OutlineWritePass = new OutlineWritePass(m_CharactersMask, m_OccluderMask, m_OutlineWriteMaterial, m_OutlineOccludeMaterial, m_OutlineWritePassEvent);
        m_PostProcessPass = new OutlinePostProcessPass(m_ShaderTags, m_OutlinePostProcessPassEvent, m_QueueType, m_CharactersMask, m_TargetMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled)
        {
            renderer.EnqueuePass(m_OutlineWritePass);
            renderer.EnqueuePass(m_PostProcessPass);
        }
    }
}