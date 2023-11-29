#version 440

out vec4 FragColor;

uniform float aspectRatio;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;


void main()
{
	const vec2 midPoint = vec2(0.5f * aspectRatio, 0.5f);

	vec2 tempUVs = UV0;
	tempUVs.x *= aspectRatio;

	float dist = distance(midPoint, UV0);

	const float reticleWidth = 0.02f;
	const float innerThickness = 0.0025f;

	dist = step(reticleWidth, dist) - step(reticleWidth + innerThickness, dist);



	FragColor = dist * vec4(1.0f, 0.0f, 0.0f, 1.0f);


}