using System;
using UnityEngine;
/*
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RadarPingRenderer), PostProcessEvent.AfterStack, "Custom/RadarPing")]
public sealed class RadarPing : PostProcessEffectSettings
{
    [Range(0f, 2f), Tooltip("Speed of the pulse.")]
    public FloatParameter speed = new FloatParameter { value = 1.0f };
    [Range(0f, 1f), Tooltip("Size of the Pulse")]
    public FloatParameter pulsewidth = new FloatParameter { value = 0.1f };
    
}

public sealed class RadarPingRenderer : PostProcessEffectRenderer<RadarPing>
{
    private float distance = 0f;
    private float widthDefault = 0.1f;
    public override void Render(PostProcessRenderContext context)
    {
        

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/RadarPing"));
        sheet.properties.SetFloat("_Position", distance);
        sheet.properties.SetFloat("_PulseWidth", settings.pulsewidth*widthDefault);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

        distance += settings.speed * 0.001f;
        if (distance > 1.0f)
            distance = 0f;
    }
}
*/