#version 440 core

out vec4 FragColor;

uniform vec4 bulletColor; // Color of the bullet

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;


void main()
{
    // Set the fragment color to the bullet color
    FragColor = bulletColor;
}
