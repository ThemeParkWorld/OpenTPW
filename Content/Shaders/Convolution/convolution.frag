#version 450
const float PI = 3.14159265359;

in VS_OUT {
    vec3 position;
} vs_out;

out vec4 fragColor;

uniform samplerCube environmentMap;

void main()
{
    vec3 normal = normalize(vs_out.position);
    vec3 irradiance = vec3(0.0);
    vec3 up = vec3(0, 1.0, 0);
    vec3 right = cross(up, normal);
    up = cross(normal, right);

    float sampleDelta = 0.025;
    int sampleCount = 0;
    for (float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta)
    {
        for (float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta)
        {
            vec3 tangentSample = vec3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta));
            vec3 sampleVec = tangentSample.x * right + tangentSample.y * up + tangentSample.z * normal;

            irradiance += texture(environmentMap, sampleVec).rgb * cos(theta) * sin(theta);
            sampleCount++;
        }
    }

    irradiance = PI * irradiance * (1.0 / float(sampleCount));
    fragColor = vec4(irradiance, 1.0);
}