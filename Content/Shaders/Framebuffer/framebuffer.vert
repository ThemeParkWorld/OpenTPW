#version 450

layout(location = 0) in vec3 vertexPos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 biTangent;
layout(location = 4) in vec2 texCoords;

out vec3 outVertexPos;
out vec2 outTexCoords;
out vec3 outNormal;

void main() {
    outVertexPos = vertexPos;
    outTexCoords = texCoords;
    outNormal = normal;

    gl_Position = vec4(vertexPos, 1.0);
}