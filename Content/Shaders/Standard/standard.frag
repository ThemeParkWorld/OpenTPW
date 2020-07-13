#version 450

#define PI 3.14159265359

struct Material {
    vec4 ambientColor;
    sampler2D ambientTexture;

    vec4 diffuseColor;
    sampler2D diffuseTexture;

    vec4 specularColor;
    sampler2D specularTexture;

    float specularExponentColor;
    sampler2D specularExponentTexture;

    float transparency;
    sampler2D transparencyTexture;

    int illuminationModel;

    sampler2D bumpTexture;
    sampler2D displacementTexture;
    sampler2D stencilTexture;

    float roughness;
    sampler2D roughnessTexture;

    float metallic;
    sampler2D metallicTexture;

    float sheen;
    sampler2D sheenTexture;

    float clearcoatThickness;
    float clearcoatRoughness;
    
    vec4 emissiveColor;
    sampler2D emissiveTexture;

    float anisotropy;
    float anisotropyRot;
};

struct Light {
    vec3 pos;
    float range;
    float constant;
    float linear;
    float quadratic;
};

in vec3 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormal;
in vec3 outFragPos;
in vec4 outFragPosLightSpace;

uniform Material material;
uniform Light light;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform vec3 cameraPos;

uniform vec3 skyColor;
uniform float fogNear;

uniform sampler2D shadowMap;

out vec4 fragColor;

vec3 modelPos;
vec3 cameraDirection;
vec3 lightDirection;
vec3 normalizedNormal;

vec3 CalcAmbience()
{
    return material.ambientColor.xyz * 0.05;
}

vec3 CalcDiffuse()
{
    float diffuseStrength = max(dot(normalizedNormal, lightDirection), 0.0);
    return vec3(diffuseStrength);
}

vec3 CalcSpecular()
{
    float specularStrength = 0.5;
    float specularPower = 16;
    vec3 viewDirection = normalize(cameraPos - outFragPos);
    vec3 halfwayDirection = normalize(lightDirection + cameraDirection);
    float specularDirection = pow(max(dot(normalizedNormal, halfwayDirection), 0.0), specularPower);
    return specularStrength * specularDirection * vec3(1.0);
}

float CalcShadow()
{
    vec3 projCoords = outFragPosLightSpace.xyz / outFragPosLightSpace.w;
    projCoords = (projCoords * 0.5) + 0.5;
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    float currentDepth = projCoords.z;
    return (currentDepth > closestDepth) ? 1.0 : 0.0;
}

vec3 CalcFullMix()
{
    float shadow = CalcShadow();
    vec3 fullMix = (CalcAmbience() + (1.0 - shadow) * (CalcSpecular() + CalcDiffuse())) * texture(material.diffuseTexture, outUvCoord).xyz;
    return fullMix;
}

void main()
{
    normalizedNormal = normalize(outNormal);
    lightDirection = normalize(light.pos - outFragPos);

    fragColor = vec4(CalcFullMix(), 1.0 - material.transparency);
}