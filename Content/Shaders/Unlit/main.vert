#version 450

layout(location = 0) in vec3 inVertexPos;
layout(location = 1) in vec2 inUvCoord;
layout(location = 2) in vec3 inNormal;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec3 outVertexPos;
out vec2 outUvCoord;
out vec3 outNormal;
out vec3 outFragPos;

void main() {
    outVertexPos = inVertexPos;
    outUvCoord = inUvCoord;
    outNormal = inNormal;

    gl_Position = projMatrix * viewMatrix * modelMatrix * vec4(inVertexPos, 1.0);
    outFragPos = vec3(modelMatrix * vec4(inVertexPos, 1.0));
}