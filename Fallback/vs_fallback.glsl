#version 450

struct PerspCam {
	mat4 Proj;
	mat4 View;
	vec3 Position;
	vec3 Forward;
};
uniform PerspCam Camera;

vec4 CameraProject(vec3 pos) {
    return Camera.Proj * Camera.View * vec4(pos, 1.0);
}

struct PixelInput {
	vec3 Position;
	vec3 Normal;
	vec2 TexCoord;
};

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normal;
layout (location = 2) in vec2 TexCoord;
out PixelInput i;

void main() { 
	i.Position = Position;
	i.Normal = Normal;
	i.TexCoord = TexCoord;
	
	gl_Position = CameraProject(i.Position);
}