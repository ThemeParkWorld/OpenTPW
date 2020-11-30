#version 450

in vec3 outVertexPos;
in vec2 outTexCoords;
in vec3 outNormal;

uniform sampler2D diffuseTexture;
uniform float exposure;

uniform int tonemapOperator;

out vec4 fragColor;
const float gamma = 2.2;

const int Tonemap_None                          = 0;
const int Tonemap_Reinhard                      = 1;
const int Tonemap_ReinhardExtendedLuminance     = 2;
const int Tonemap_ReinhardJodie                 = 3;
const int Tonemap_AcesApproximation             = 4;

// Useful resources:
// - https://64.github.io/tonemapping/
// - https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/

float Luminance(vec3 v)
{
    return dot(v, vec3(0.2126, 0.7152, 0.0722));
}

vec3 ChangeLuminance(vec3 cIn, float lOut)
{
    float lIn = Luminance(cIn);
    return cIn * (lOut / lIn);
}

vec3 ReinhardJodie(vec3 v)
{
    float l = Luminance(v);
    vec3 tv = v / (1.0 + v);
    return mix(v / (1.0 + l), tv, tv);
}

vec3 Reinhard(vec3 v)
{
    return v / (1.0 + v);
}

vec3 ReinhardExtendedLuminance(vec3 v, float maxWhiteL)
{
    float lOld = Luminance(v);
    float numerator = lOld * (1.0 + (lOld / (maxWhiteL * maxWhiteL)));
    float lNew = numerator / (1.0 + lOld);
    return ChangeLuminance(v, lNew);
}

vec3 AcesApproximation(vec3 v)
{
    v *= 0.6;
    float a = 2.51;
    float b = 0.03;
    float c = 2.43;
    float d = 0.59;
    float e = 0.14;
    return clamp((v * (a * v + b)) / (v * (c * v + d) + e), 0.0, 1.0);
}

void main() 
{
    vec3 hdrColor = texture2D(diffuseTexture, outTexCoords).xyz;
    vec3 tonemappedColor;
    switch (tonemapOperator)
    {
        case Tonemap_None:
            tonemappedColor = hdrColor;
            break;
        case Tonemap_Reinhard:
            tonemappedColor = Reinhard(hdrColor);
            break;
        case Tonemap_ReinhardExtendedLuminance:
            tonemappedColor = ReinhardExtendedLuminance(hdrColor, 1.0);
            break;
        case Tonemap_ReinhardJodie:
            tonemappedColor = ReinhardJodie(hdrColor);
            break;
        case Tonemap_AcesApproximation:
            tonemappedColor = AcesApproximation(hdrColor);
            break;
    }

    tonemappedColor *= pow(2, exposure);
    fragColor = vec4(tonemappedColor, 1.0);
}