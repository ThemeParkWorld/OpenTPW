#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoords;

out struct VS_OUT {
    vec2 texCoords;
} vs_out;

void main() {
    vs_out.texCoords = texCoords;
    gl_Position = vec4( position, 1.0 );
}