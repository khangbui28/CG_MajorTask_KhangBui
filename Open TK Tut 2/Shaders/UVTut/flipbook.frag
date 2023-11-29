#version 440

out vec4 FragColor;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

uniform sampler2D tex3;
uniform float time;

const float width = 5;
const float height = 5;

const float speed = 24; 

void main(){


	const float totalFrames = width * height;
	float currentFrameIndex = floor(mod(time * speed + width,totalFrames));


	vec2 newUVs = UV0;

	newUVs.x = (UV0.x + mod(currentFrameIndex,width)) / width; 
	newUVs.y = (UV0.y - floor(currentFrameIndex/ width)) / height; 

	vec4 textureColor = texture(tex3,newUVs);

	FragColor = textureColor;


}