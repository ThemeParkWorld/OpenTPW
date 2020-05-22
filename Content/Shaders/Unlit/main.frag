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

uniform Material material;
uniform Light light;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform vec3 cameraPos;

uniform vec3 skyColor;
uniform float fogNear;

out vec4 fragColor;

vec3 modelPos;
vec3 cameraDirection;

vec3 CalcAmbience()
{
    return material.ambientColor.xyz;
}

vec3 CalcFullMix()
{
    return CalcAmbience() * texture(material.diffuseTexture, outUvCoord).xyz;
}

void main()
{
    cameraDirection = normalize(cameraPos - modelPos);

    fragColor = vec4(CalcFullMix(), 1.0 - material.transparency);
}