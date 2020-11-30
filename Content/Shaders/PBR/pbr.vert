#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 bitangent;
layout(location = 4) in vec2 texCoords;

out VS_OUT {
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

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform mat4 lightViewMatrix;
uniform mat4 lightProjectionMatrix;

uniform vec3 camPos;
uniform vec3 lightPos;

void main() {
    mat3 modelNormalMatrix = transpose(inverse(mat3(modelMatrix)));
    vec3 T = normalize(vec3(modelNormalMatrix * tangent));
    vec3 N = normalize(vec3(modelNormalMatrix * normal));
    T = normalize(T - dot(T, N) * N);

    vec3 B = cross(N, T);
    mat3 TBN = transpose(mat3(T, B, N));

    vs_out.worldPos = vec3(modelMatrix * vec4(position, 1.0));
    vs_out.camPos = camPos;
    vs_out.lightPos = lightPos;

    vs_out.texCoords = texCoords;
    vs_out.normal = normal;
    vs_out.fragPosLightSpace = lightProjectionMatrix * lightViewMatrix * vec4(vs_out.worldPos, 1.0);

    vs_out.tangentLightPos = TBN * vs_out.lightPos;
    vs_out.tangentCamPos = TBN * vs_out.camPos;
    vs_out.tangentWorldPos = TBN * vs_out.worldPos;
    
    gl_Position = projectionMatrix * viewMatrix * vec4(vs_out.worldPos, 1.0);
}