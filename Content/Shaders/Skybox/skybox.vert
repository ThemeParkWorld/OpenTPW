#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 bitangent;
layout(location = 4) in vec2 texCoords;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out VS_OUT {
    vec3 worldPos;
} vs_out;

void main()
{
    vs_out.worldPos = position;
    gl_Position = projMatrix * mat4(mat3(viewMatrix)) * modelMatrix * vec4(position, 1.0);
}
