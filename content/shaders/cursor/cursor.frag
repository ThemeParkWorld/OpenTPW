#version 450

in struct VS_OUT {
    vec2 texCoords;
} vs_out;

out vec4 fragColor;
uniform sampler2D g_tDiffuse;

uniform int g_iFrame;

void main() {
    vec2 texCoords = vs_out.texCoords;
    texCoords.x = ( texCoords.x / 4 );
    texCoords.x += g_iFrame * 0.25;
    
    fragColor = texture( g_tDiffuse, texCoords );

    if ( fragColor == vec4( 1.0, 0.0, 1.0, 1.0 ) )
        discard;
}