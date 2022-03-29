#version 450

//
// Layout
//
layout( location = 0 ) in vec3 position;
layout( location = 1 ) in vec3 normal;
layout( location = 2 ) in vec2 texCoords;

//
// Uniforms
//
layout( set = 0, binding = 2 ) uniform ObjectUniformBuffer {
    mat4 g_mModel;
} g_oUbo;

//
// Out
//
layout( location = 0 ) out struct VS_OUT {
    vec2 texCoords;
} vs_out;

void main() {
    vs_out.texCoords = texCoords;
    gl_Position = g_oUbo.g_mModel * vec4( position, 1.0 );
}