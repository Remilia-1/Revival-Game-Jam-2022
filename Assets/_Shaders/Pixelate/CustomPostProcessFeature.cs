using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Project.PostProcessing
{
    public class CustomPostProcessFeature : ScriptableRendererFeature
    {
        //[SerializeField] private Material m_GlitchMaterial;

        private PixelatePass pass;
        //private GlitchPass glitchPass;



        public override void Create()
        {
            pass = new PixelatePass();
            //glitchPass = new GlitchPass(m_GlitchMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.postProcessEnabled)
            {
                renderer.EnqueuePass(pass);
                //renderer.EnqueuePass(glitchPass);
            }
        }
    }
}