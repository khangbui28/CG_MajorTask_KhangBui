#version 440 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormals;
layout (location = 2) in vec2 UVs;


out vec2 UV0;
out vec3 Normals;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

float rand(vec2 uv)
{
    return fract(sin(dot(uv.xy, vec2(12.98989, 78.525))) * 43785.42344);
}

float perlinNoise(vec2 uv)
{

        vec2 i = floor(uv);
        vec2 r = fract(uv);

        float a = rand(i);
        float b = rand(i + vec2(1, 0));
        float c = rand(i + vec2(0, 1));
        float d = rand(i + vec2(1, 1));

        vec2 u = r * r * (3.0 - 2.0 * r);

        return (mix(a, b, u.r) + (c - a) * u.g * (1 - u.r) + (d - b) * u.r * u.g);
   
}

void main()
{

    vec4 pos = vec4(aPosition, 1.0);

    const float scale = 10;
    const float Offset = 0.2f;

    const float circleWidth = 0.5f;
    float dist = distance(vec2(0.5), UVs);

    dist = step(circleWidth, dist);

    float rng = perlinNoise(UVs * scale);
    pos.z += rng * dist * Offset;

    gl_Position = pos * model * view * projection;
    UV0 = UVs;
    Normals = aNormals * mat3(transpose(inverse(model)));
    FragPos = vec3(pos * model);
} 