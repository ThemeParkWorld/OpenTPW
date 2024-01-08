vertex {
    //
    // Layout
    //
    layout( location = 0 ) in vec3 position;
    layout( location = 1 ) in vec3 normal;
    layout( location = 2 ) in vec2 texCoords;

    //
    // Uniforms
    //
    layout( set = 0, binding = 0 ) uniform ObjectUniformBuffer {
        mat4 g_mModel;
        mat4 g_mView;
        mat4 g_mProj;

        vec3 g_vLightPos;
        vec3 g_vLightColor;
        vec3 g_vCameraPos;

        float g_fTime;
    } g_oUbo;

    //
    // Out
    //
    layout( location = 0 ) out struct VS_OUT {
        vec2 vTexCoords;
        vec3 vNormal;
        vec3 vPosition;
        vec3 vPositionWs;
    } vs_out;

    void main() {
        vs_out.vTexCoords = texCoords;
        
        vs_out.vNormal = normal;
        vs_out.vPosition = vec3( g_oUbo.g_mModel * vec4( position, 1.0 ));

        vec4 pos = g_oUbo.g_mModel * vec4( position, 1.0 );
        vs_out.vPositionWs = vec3( g_oUbo.g_mProj * g_oUbo.g_mView * pos );
        gl_Position = g_oUbo.g_mProj * g_oUbo.g_mView * pos;
    }
}

fragment {
    //
    // In
    //
    layout( location = 0 ) in struct VS_OUT {
        vec2 vTexCoords;
        vec3 vNormal;
        vec3 vPosition;
        vec3 vPositionWs;
    } vs_out;

    //
    // Out
    //
    layout( location = 0 ) out vec4 fragColor;

    //
    // Uniforms
    //
    layout( set = 0, binding = 0 ) uniform ObjectUniformBuffer {
        mat4 g_mModel;
        mat4 g_mView;
        mat4 g_mProj;

        vec3 g_vLightPos;
        vec3 g_vLightColor;
        vec3 g_vCameraPos;

        float g_fTime;
    } g_oUbo;

    layout( set = 1, binding = 0 ) uniform texture2D Color;
    layout( set = 1, binding = 1 ) uniform sampler s_Color;

    void main() 
    {
        float waveScale = 2.0;
        float waveFrequency = 0.25;
        float waveAmplitude = 0.025;
        float pi = 3.14159265359;

        vec2 sinTime = (vs_out.vPosition.yx * waveScale + g_oUbo.g_fTime * waveFrequency) * pi;
        vec2 uv = vs_out.vPosition.xy + vec2(sin(sinTime.x), sin(sinTime.y)) * waveAmplitude;

        vec4 vColor = texture( sampler2D( Color, s_Color ), uv ) * 0.75;
        vColor.z *= 1.25;

        vec3 fogColor = vec3( 0.28, 0.88, 1.0 );
        float fogStart = 2.0;
        float fogEnd = 30.0;

        // smoother fog, better looking
        float fogAmount = ( 1.0 - exp( -length( vs_out.vPositionWs ) * 0.01 ) ) / ( 1.0 - exp( -fogEnd * 0.01 ) );
        fogAmount = clamp( fogAmount, 0.0, 1.0 );

        fragColor = mix( vColor, vec4( fogColor, 1.0 ), fogAmount );
    }
}