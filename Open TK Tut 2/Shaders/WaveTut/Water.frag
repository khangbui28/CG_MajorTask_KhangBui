#version 440

out vec4 FragColor;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;


uniform float time;

void main(){
    const float m = 10;
    const float fuzziness = 0.5f;
    const float hardness = 0.2f;
    float r = hardness + fuzziness * sin(UV0.x * m + sin(UV0.y * m ) + time);

    vec2 midPoint = vec2(0.5f, 0.5f);
    float dist = 1.0 - distance(midPoint, UV0);

    const float circleWidth = 0.5f;
    dist = step(circleWidth, dist);

    // Use 'dist' as a mask for 'r' to create a circle
    r *= dist;

    if (dist < circleWidth) {
        discard; // Discard the fragment inside the circle
    }

    FragColor = vec4(r, 1, 1, 1);
}