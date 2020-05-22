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

out vec4 fragColor;

vec3 modelPos;

vec3 normal;
vec3 lightDirection;
vec3 cameraDirection;

void main() 
{
    lightDirection = normalize(light.pos - modelPos);

    lightDirection = normalize(modelPos - light.pos);
    cameraDirection = normalize(cameraPos - modelPos);
    normal = normalize(outNormal);

    fragColor = texture(material.diffuseTexture, outUvCoord * vec2(1, -1));
}