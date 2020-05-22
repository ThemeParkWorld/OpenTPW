#version 450

in vec3 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormal;

uniform sampler2D colorTexture;
uniform float exposure;

out vec4 fragColor;
const float gamma = 1.2;

void main() 
{
    vec3 hdrColor = texture2D(colorTexture, outUvCoord).xyz;
    vec3 tonemappedColor = vec3(1.0) - exp(-hdrColor * exposure);
    
    tonemappedColor = pow(tonemappedColor, vec3(1.0 / gamma));
    fragColor = vec4(tonemappedColor, 1.0);
}