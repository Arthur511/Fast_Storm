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
    float diffuse = saturate(dot(s.normal, l.direction));
    
    float3 h = SafeNormalize(l.direction + s.view);
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *= diffuse * s.smoothness;
    
    float rim = 1 - dot(s.view, s.normal);
    rim *= pow(diffuse, s.rimTreshold);
    
    return l.color * (diffuse + max(specular, rim));
}

#endif

void GetLighting_V2_float(float Smoothness, float RimTreshold, float3 Normal, float3 View,out float3 Color) {
#if defined(SHADERGRAPH_PREVIEW)
    ...
#else
    SurfaceVariables s;
    s.normal = normalize(Normal);
    s.view = SafeNormalize(View);
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * Smoothness + 1);
    s.rimTreshold = RimTreshold;
    
    Light light = GetMainLight();
    Color = CalculateCelShading(light, s);
#endif
}

#endif