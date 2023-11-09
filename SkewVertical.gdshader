shader_type spatial;
render_mode depth_draw_always;

uniform float CURVATURE;
uniform float CURVATURE_ACTIVE;
uniform float CURVATURE_DISTANCE;

instance uniform vec4 color : source_color = vec4(1.0);

void vertex() {
	NORMAL = (INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz;
	float dist = length(INV_VIEW_MATRIX[3].xyz - VERTEX) / CURVATURE_DISTANCE;
	VERTEX.y -= pow(dist, CURVATURE);

	//vec3 v = (VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	//float vzr = mix(0.0, 5.0, v.z) / 5.0;
	//float y = mix(0.0, 2.0, vzr);
	//POSITION = vec4(v.x, v.y + y, v.z, 1.0);
//	vec3 newpos = VERTEX + vec3(sin(TIME*0.1), 0, 0);
//	POSITION = vec4(sin(TIME*0.1), 0, 0, 0);
}

//void fragment() {
//	float z = mix(0,4,FRAGCOORD.z) / 4.0;
//	ALBEDO = vec3(z, 0, 0);
//}