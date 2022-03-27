#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoords;

uniform mat4 g_mModel;
uniform mat4 g_mProj;
uniform mat4 g_mView;

out struct VS_OUT {
    vec2 vTexCoords;
    vec3 vNormal;
    vec3 vPosition;
} vs_out;

void main() {
    vs_out.vTexCoords = texCoords;
    vs_out.vNormal = normal;

    vec4 pos = g_mModel * vec4( position, 1.0 );
    vs_out.vPosition = pos.xyz;

    gl_Position = g_mProj * g_mView * pos;
}