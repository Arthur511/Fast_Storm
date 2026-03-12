#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
struct SurfaceVariables {

    float3 normal;
    float3 view;
    float smoothness;
    float shininess;
    float rimTreshold;
};

float3 CalculateCelShading(Light l, SurfaceVariables s) {
    float attenuation = l.shadowAttenuation * l.distanceAttenuation;
    float diffuse = saturate(dot(s.normal, l.direction));
    float3 h = SafeNormalize(l.direction + s.view);
    diffuse *= attenuation;
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *= diffuse * s.smoothness;
    float rim = 1 - dot(s.view, s.normal);
    rim *= pow(diffuse, s.rimTreshold);

    return l.color * (diffuse + max(specular, rim));
}

#endif

void GetLighting_V2_float(float Smoothness, float RimTreshold, float3 Position,float3 Normal, float3 View,out float3 Color) {
#if defined(SHADERGRAPH_PREVIEW)
    ...
#else
    SurfaceVariables s;
    s.normal = normalize(Normal);
    s.view = SafeNormalize(View);
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * Smoothness + 1);
    s.rimTreshold = RimTreshold;
    
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    
    Light light = GetMainLight(shadowCoord);
    Color = CalculateCelShading(light, s);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++) {
        light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, s);
    }
#endif
}

#endif