#version 100
precision highp float;

uniform mat4 proj_matrix;
uniform float text_geometry_scale;
uniform vec2 text_geometry_scale_pivot;

attribute vec3 in_position;
attribute vec2 in_tc;
attribute vec4 in_colour;

varying vec2 tc;
varying vec4 colour;

void main(void)
{
	tc = in_tc;
	colour = in_colour;

    vec2 scaled_position =
        text_geometry_scale_pivot +
        (in_position.xy - text_geometry_scale_pivot) * text_geometry_scale;

	gl_Position = proj_matrix * vec4(scaled_position, in_position.z, 1.);
}
