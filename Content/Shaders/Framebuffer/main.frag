#version 450

in vec3 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormal;

uniform sampler2D texture;

out vec4 fragColor;

void main() 
{
    fragColor = texture2D(texture, outUvCoord);
}