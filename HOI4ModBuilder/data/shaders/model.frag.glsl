#version 330 core

in vec2 in_uv;
in vec4 in_color;

out vec4 out_color;

uniform sampler2D in_texture;

void main() {
	out_color = texture(in_texture, in_uv.xy) * in_color;
}