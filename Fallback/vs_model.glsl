#version 450

#include "shaders/vs_input.glsl"
uniform mat4 proj;
uniform mat4 view;
out vec2 uv;

void main() {
	gl_Position = proj * view * vec4(in_pos, 1.0);
	uv = in_uv;
}