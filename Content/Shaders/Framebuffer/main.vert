#version 450

layout(location = 0) in vec3 inVertexPos;
layout(location = 1) in vec2 inUvCoord;
layout(location = 2) in vec3 inNormal;

out vec3 outVertexPos;
out vec2 outUvCoord;
out vec3 outNormal;

void main() {
    outVertexPos = inVertexPos;
    outUvCoord = inUvCoord;
    outNormal = inNormal;

    gl_Position = vec4(inVertexPos, 1.0);
}