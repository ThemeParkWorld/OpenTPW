#version 450

in struct VS_OUT {
    vec2 texCoords;
} vs_out;

out vec4 fragColor;
uniform sampler2D g_tDiffuse;

void main() {
    fragColor = texture( g_tDiffuse, vs_out.texCoords );
}