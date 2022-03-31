#version 450

//
// In
//
layout( location = 0 ) in struct VS_OUT {
    vec2 vTexCoords;
    vec3 vNormal;
    vec3 vPosition;
} vs_out;

//
// Out
//
layout( location = 0 ) out vec4 fragColor;

//
// Uniforms
//
layout( set = 0, binding = 0 ) uniform texture2D g_tDiffuse;
layout( set = 0, binding = 1 ) uniform sampler g_sDiffuse;
layout( set = 0, binding = 2 ) uniform ObjectUniformBuffer {
    mat4 g_mModel;
    mat4 g_mView;
    mat4 g_mProj;

    vec3 g_vLightPos;
    vec3 g_vLightColor;
    vec3 g_vCameraPos;
} g_oUbo;

vec3 lambert( vec3 vLightDir, vec3 vNormal ) 
{
    return dot( normalize( vLightDir ), normalize( vNormal ) ) * g_oUbo.g_vLightColor;
}

vec3 ambient( vec3 vLightColor ) 
{
    return vLightColor * 0.2;
}

vec3 specular( vec3 vLightDir, vec3 vNormal, vec3 vCameraDir, float fShininess ) 
{
    vec3 vReflection = reflect( normalize( -vLightDir ), normalize( vNormal ) );
    return pow( max( dot( normalize( vCameraDir ), vReflection ), 0.0 ), fShininess ) * g_oUbo.g_vLightColor;
}

void main() 
{
    vec3 vLightDir = normalize( g_oUbo.g_vLightPos - vs_out.vPosition );

    vec3 vLambert = lambert( vLightDir, vs_out.vNormal );
    vec3 vAmbient = ambient( g_oUbo.g_vLightColor );
    
    vec3 vCameraDir = normalize( g_oUbo.g_vCameraPos - vs_out.vPosition );
    vec3 vSpecular = specular( vLightDir, vs_out.vNormal, vCameraDir, 32.0 );

    vec4 vColor = texture( sampler2D( g_tDiffuse, g_sDiffuse ), vs_out.vTexCoords );

    fragColor = vec4( vLambert + vAmbient + vSpecular, 1.0 ) * vColor;
}