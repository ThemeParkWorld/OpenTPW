#version 450

//
// Layout
//
layout( location = 0 ) in vec3 position;
layout( location = 1 ) in vec3 normal;
layout( location = 2 ) in vec2 texCoords;

//
// Uniforms
//
layout( set = 0, binding = 2 ) uniform ObjectUniformBuffer {
    mat4 g_mModel;
    mat4 g_mView;
    mat4 g_mProj;

    vec3 g_vLightPos;
    vec3 g_vLightColor;
    vec3 g_vCameraPos;
} g_oUbo;

//
// Out
//
layout( location = 0 ) out struct VS_OUT {
    vec2 vTexCoords;
    vec3 vNormal;
    vec3 vPosition;
} vs_out;

void main() {
    vs_out.vTexCoords = texCoords;
    vs_out.vNormal = normal;
    vs_out.vPosition = vec3( g_oUbo.g_mModel * vec4( position, 1.0 ));

    vec4 pos = g_oUbo.g_mModel * vec4( position, 1.0 );
    gl_Position = g_oUbo.g_mProj * g_oUbo.g_mView * pos;
}