#version 450

layout(location = 0) in vec3 vertexPos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 biTangent;
layout(location = 4) in vec2 texCoords;

out VS_OUT {
    vec3 position;
    vec2 texCoords;
} vs_out;

void main() {
    vs_out.position = vertexPos;
    vs_out.texCoords = texCoords;

    gl_Position = vec4(vertexPos, 1.0);
}