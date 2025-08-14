#version 100
precision highp float;

uniform sampler2D tex_object;

varying vec2 tc;
varying vec4 colour;

void main(void)
{
	gl_FragColor = texture2D(tex_object, tc) * vec4(colour);
}
