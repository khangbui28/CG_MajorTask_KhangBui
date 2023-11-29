#version 440 core

out vec4 FragColor;

uniform vec4 uniformColor;
uniform float time; // Time variable

uniform sampler2D groundTexture;

struct PointLight {
    vec3 lightColor;
    vec3 lightPosition;
    float lightIntensity;
};

uniform PointLight pointLights[100];
uniform int numPointLights;

uniform vec3 viewPos;
const vec3 ambientLightColor = vec3(0.1, 0.1, 0.1); // Assuming constant ambient light color

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

vec3 HandleLighting()
{
    vec3 outCol = vec3(0.0);

    for (int i = 0; i < numPointLights; ++i)
    {
        const PointLight currentLight = pointLights[i];

        float ambientStrength = 0.2f;
        vec3 ambient = ambientStrength * ambientLightColor * currentLight.lightColor;

        vec3 normals = normalize(Normals);
        vec3 lightDir = normalize(currentLight.lightPosition - FragPos);

        float diff = max(dot(normals, lightDir), 0);
        vec3 diffuse = diff * currentLight.lightColor;

        diffuse = vec3(min(diffuse.x, 1), min(diffuse.y, 1), min(diffuse.z, 1));

        const float shininess = 200.0;
        float specularStrength = 10.0f; // Corrected the float value syntax
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectionDir = reflect(-lightDir, normals);
        float spec = pow(max(dot(viewDir, reflectionDir), 0), shininess);
        vec3 specularColor = specularStrength * spec * currentLight.lightColor;

        outCol += (ambient + diffuse + specularColor) * currentLight.lightIntensity;
    }

    return outCol;
}

void main()
{
    vec4 colorTex0 = texture(groundTexture, UV0);

    // Combine the texture color with lighting
    vec3 lighting = HandleLighting();
    FragColor = vec4(colorTex0.rgb * lighting, colorTex0.a);
}
