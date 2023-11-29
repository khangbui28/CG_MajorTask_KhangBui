#version 440 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormals;
layout(location = 2) in vec2 UVs;

out vec2 UV0;
out vec3 Normals;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 pos = vec4(aPosition, 1.0);
    mat4 billboardModel = view; // Use the view matrix directly for the billboard effect
    billboardModel[0][0] = 1.0; // Ensure the scale on the billboard's model matrix remains uniform
    billboardModel[0][1] = 0.0;
    billboardModel[0][2] = 0.0;
    billboardModel[1][0] = 0.0;
    billboardModel[1][1] = 1.0;
    billboardModel[1][2] = 0.0;
    billboardModel[2][0] = 0.0;
    billboardModel[2][1] = 0.0;
    billboardModel[2][2] = 1.0;

    gl_Position = pos * billboardModel * projection; // Use the modified model matrix
    UV0 = UVs;
    Normals = aNormals * mat3(transpose(inverse(model)));
    FragPos = vec3(pos * model);
}
