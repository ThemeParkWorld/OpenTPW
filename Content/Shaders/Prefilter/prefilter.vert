#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 bitangent;
layout(location = 4) in vec2 texCoords;

out VS_OUT {
    vec3 position;
} vs_out;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
    vs_out.position = position;
    gl_Position = projectionMatrix * viewMatrix * vec4(position, 1.0);
}