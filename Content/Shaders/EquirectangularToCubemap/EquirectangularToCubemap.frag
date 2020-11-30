#version 450
const float PI = 3.14159265359;

in VS_OUT {
    vec3 position;
} vs_out;

out vec4 fragColor;

uniform sampler2D equirectangularMap;
const vec2 invAtan = vec2(0.1591, 0.3183);

vec2 SampleSphericalMap(vec3 v)
{
    // Spherical to cartesian
    vec2 uv = vec2(atan(v.z, v.x), asin(v.y));
    uv *= invAtan;
    uv += 0.5;

    uv.y = 1.0 - uv.y;
    return uv;
}
void main()
{
    vec2 uv = SampleSphericalMap(normalize(vs_out.position));
    vec3 color = texture(equirectangularMap, uv).xyz;
    fragColor = vec4(color, 1.0);
}