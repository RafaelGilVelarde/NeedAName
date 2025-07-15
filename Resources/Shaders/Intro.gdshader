shader_type canvas_item;
uniform sampler2D noise : repeat_enable;
uniform sampler2D noise1 : repeat_enable;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture,filter_linear_mipmap;
uniform vec2 scroll = vec2(0.05,0.05);
uniform vec2 scroll1 = vec2(-0.05,-0.05);
uniform float distortion_strength : hint_range(-1,1) = 0.2;
uniform vec4 tone_color : source_color;

void vertex() {
	// Called for every vertex the material is visible on.
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 noise_col = texture(noise, UV+scroll*TIME);
	vec4 noise_col_1 = texture(noise1, UV+scroll1*TIME);
	float depth = noise_col.r*noise_col_1.r;
	vec4 screen_col = texture(SCREEN_TEXTURE, SCREEN_UV + distortion_strength * vec2(depth));
	COLOR = screen_col*tone_color;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
//}
