#version 330 core

in vec3 in_pos;
in vec2 in_uv;
in vec4 in_color;

out vec3 out_uv;
out vec4 out_color;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;

void main() {
	gl_Position = projMatrix * viewMatrix * vec4(in_pos, 1.0);
	out_uv = in_uv;
	out_color = in_color;
}