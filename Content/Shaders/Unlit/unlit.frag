#version 450
const float PI = 3.14159265359;

in VS_OUT {
    vec2 texCoords;
    vec3 normal;
    vec4 fragPosLightSpace;

    vec3 lightPos;
    vec3 camPos;
    vec3 worldPos;

    vec3 tangentLightPos;
    vec3 tangentCamPos;
    vec3 tangentWorldPos;
} vs_out;

out vec4 fragColor;

struct Texture {
    bool exists;
    sampler2D texture;
};

struct Material {
    Texture texture_diffuse1;
    Texture texture_diffuse2;

    Texture texture_emissive1;

    Texture texture_unknown1;

    Texture texture_normal1;

    Texture texture_specular1;
};

uniform Material material;
uniform sampler2D shadowMap;
uniform samplerCube irradianceMap;
uniform samplerCube prefilterMap;
uniform sampler2D brdfLut;
uniform sampler2D holoMap;

void main() {
    vec4 albedoSrc = texture(material.texture_diffuse1.texture, vs_out.texCoords);
    vec3 albedo = albedoSrc.xyz;
    
    fragColor = vec4(albedo, albedoSrc.w);
}