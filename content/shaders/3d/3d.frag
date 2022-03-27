#version 450

in struct VS_OUT {
    vec2 vTexCoords;
    vec3 vNormal;
    vec3 vPosition;
} vs_out;

out vec4 fragColor;

uniform sampler2D g_tDiffuse;

uniform vec3 g_vLightPos;
uniform vec3 g_vLightColor;

uniform vec3 g_vCameraPos;

uniform bool g_bHighlighted;

vec3 lambert( vec3 vLightDir, vec3 vNormal ) 
{
    return dot( normalize( vLightDir ), normalize( vNormal ) ) * g_vLightColor;
}

vec3 ambient( vec3 vLightColor ) 
{
    return vLightColor * 0.2;
}

vec3 specular( vec3 vLightDir, vec3 vNormal, vec3 vCameraDir, float fShininess ) 
{
    vec3 vReflection = reflect( normalize( -vLightDir ), normalize( vNormal ) );
    return pow( max( dot( normalize( vCameraDir ), vReflection ), 0.0 ), fShininess ) * g_vLightColor;
}

void main() 
{
    if ( g_bHighlighted )
    {
        fragColor = texture( g_tDiffuse, vs_out.vTexCoords ) * 1.5;
        return;
    }

    vec3 vLightDir = normalize( g_vLightPos - vs_out.vPosition );

    vec3 vLambert = lambert( vLightDir, vs_out.vNormal );
    vec3 vAmbient = ambient( g_vLightColor );
    
    vec3 vCameraDir = normalize( g_vCameraPos - vs_out.vPosition );
    vec3 vSpecular = specular( vLightDir, vs_out.vNormal, vCameraDir, 32.0 );

    vec4 vColor = texture( g_tDiffuse, vs_out.vTexCoords );

    fragColor = vec4( vLambert + vAmbient + vSpecular, 1.0 ) * vColor;
}