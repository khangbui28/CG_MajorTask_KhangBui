#version 440 core

out vec4 FragColor;

uniform sampler2D tex1;

in vec2 UV0;

void main()
{

   FragColor =  texture(tex1, UV0);
   

}