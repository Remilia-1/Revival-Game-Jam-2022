using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using System;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class OutlineComponent : VolumeComponent, IPostProcessComponent
{
    public FloatParameter Size = new FloatParameter(2, true);
    public FloatParameter Offset = new FloatParameter(2, true);
    public ColorParameter Color = new ColorParameter(new Color(0, 0, 0), true);

    public bool IsActive() => !Mathf.Approximately(Size.value, 0);

    public bool IsTileCompatible() => true;
}
