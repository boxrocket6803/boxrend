#version 450

struct PixelInput {
	vec3 Position;
	vec3 Normal;
	vec2 TexCoord;
};

in PixelInput i;
out vec4 Color;

void main() {
	Color = vec4(1, 0, 1, 1);
}