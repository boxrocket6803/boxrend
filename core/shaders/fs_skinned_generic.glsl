#version 450

in vec2 uv;
uniform sampler1D palette;
uniform sampler3D color;
out vec4 out_color;

void main() {
	float i = texture(color, vec3(uv,0)).r;
	if (i == 0)
		discard;
	out_color = texture(palette, i);
}