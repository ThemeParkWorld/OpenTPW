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

void main() 
{
    vec4 vColor = texture( sampler2D( g_tDiffuse, g_sDiffuse ), vs_out.vTexCoords );
    fragColor = vColor;
}