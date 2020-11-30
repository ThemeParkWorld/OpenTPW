#version 450

out vec4 fragColor;
in VS_OUT {
    vec3 worldPos;
} vs_out;

uniform samplerCube environmentMap;

void main()
{
    vec3 envColor = textureLod(environmentMap, vs_out.worldPos, 0.0).rgb;
    fragColor = vec4(envColor, 1.0);
}