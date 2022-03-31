#version 450

//
// In
//
layout( location = 0 ) in struct VS_OUT {
    vec2 texCoords;
} vs_out;

//
// Out
//
layout( location = 0 ) out vec4 fragColor;

//
// Uniforms
//
layout( set = 0, binding = 0 ) uniform texture2D g_tDiffuse;
layout( set = 0, binding = 1 ) uniform sampler g_sDiffuse;
layout( set = 0, binding = 2 ) uniform ObjectUniformBuffer {
    mat4 g_mModel;
    int g_iFrame;
} g_oUbo;

void main() {
    vec2 texCoords = vs_out.texCoords;
    texCoords.x = ( texCoords.x / 4 );
    texCoords.x += g_oUbo.g_iFrame * 0.25;
    
    fragColor = texture( sampler2D( g_tDiffuse, g_sDiffuse ), texCoords );

    if ( fragColor == vec4( 1.0, 0.0, 1.0, 1.0 ) )
        discard;
}