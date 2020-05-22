#version 450

layout(location = 0) in vec2 inVertexPos;
layout(location = 1) in vec2 inUvCoord;
layout(location = 2) in vec4 inColor;

uniform mat4 projection;

out vec2 outVertexPos;
out vec2 outUvCoord;
out vec4 outColor;

void main() {
    outVertexPos = inVertexPos;
    outUvCoord = inUvCoord;
    outColor = inColor;

    gl_Position = projection * vec4(inVertexPos, 1.0, 1.0);
}