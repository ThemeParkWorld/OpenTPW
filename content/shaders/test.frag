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
// layout( set = 0, binding = 0 ) uniform texture2D g_tDiffuse;
// layout( set = 0, binding = 1 ) uniform sampler g_sDiffuse;

void main() {
    fragColor = vec4( vs_out.texCoords.x, vs_out.texCoords.y, 0.0, 1.0 );
}