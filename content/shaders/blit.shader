vertex {

layout(location = 0) out vec2 vs_texCoords;

void main() {
    // Generate fullscreen triangle positions from vertex ID
    vec2 positions[3] = vec2[](
        vec2(-1.0, -1.0),
        vec2(-1.0,  3.0),
        vec2( 3.0, -1.0)
    );

    vec2 texCoords[3] = vec2[](
        vec2(0.0, 1.0),
        vec2(0.0, -1.0),
        vec2(2.0, 1.0)
    );

    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
    vs_texCoords = texCoords[gl_VertexIndex];
}

}

fragment {

layout(set = 0, binding = 0) uniform texture2D g_tInput;
layout(set = 0, binding = 1) uniform sampler g_sSampler;

layout(location = 0) in vec2 fs_texCoords;
layout(location = 0) out vec4 fragColor;

void main() {
    fragColor = texture(sampler2D(g_tInput, g_sSampler), fs_texCoords);
}

}