#version 450

in vec2 outVertexPos;
in vec2 outUvCoord;
in vec4 outColor;

uniform sampler2D albedoTexture;

out vec4 frag_color;

void main() {
    frag_color = texture(albedoTexture, outUvCoord) * outColor;
    // frag_color = vec4(outUvCoord, 0, 1);
}