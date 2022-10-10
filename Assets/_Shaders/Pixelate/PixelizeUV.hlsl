inline void PixelizeUV_float(float2 UV, float screenWidth, float screenHeight, float size, out float2 pixelatedUV)
{ 
	float heightToWidth = screenHeight / screenWidth;
	float widthToHeight = screenWidth / screenHeight;

	UV.x *= (size * widthToHeight);
	UV.y *= size;

	UV.x = round(UV.x);
	UV.y = round(UV.y);

	UV.x *= heightToWidth;

	UV.x /= size;
	UV.y /= size;

	pixelatedUV = UV;
}

inline void PixelizeUV_float(float2 UV, float screenWidth, float screenHeight, float size, float offset, out float2 shiftedUV_Left, out float2 shiftedUV_Right, out float2 shiftedUV_Top, out float2 shiftedUV_Bottom)
{
	float heightToWidth = screenHeight / screenWidth;
	float widthToHeight = screenWidth / screenHeight;

	// left side
	shiftedUV_Left = UV;
	shiftedUV_Left.x = (UV.x - (offset * heightToWidth)) * (size * widthToHeight);
	shiftedUV_Left.y = UV.y * size;

	shiftedUV_Left.x = round(shiftedUV_Left.x);
	shiftedUV_Left.y = round(shiftedUV_Left.y);
	shiftedUV_Left.x *= heightToWidth;

	shiftedUV_Left.x /= size;
	shiftedUV_Left.y /= size;

	// right side

	shiftedUV_Right = UV;
	shiftedUV_Right.x = (UV.x + offset * heightToWidth) * (size * widthToHeight);
	shiftedUV_Right.y = UV.y * size;

	shiftedUV_Right.x = round(shiftedUV_Right.x);
	shiftedUV_Right.y = round(shiftedUV_Right.y);
	shiftedUV_Right.x *= heightToWidth;

	shiftedUV_Right.x /= size;
	shiftedUV_Right.y /= size;

	// top side

	shiftedUV_Top = UV;
	shiftedUV_Top.x = UV.x * (size * widthToHeight);
	shiftedUV_Top.y = (UV.y + offset) * size;

	shiftedUV_Top.x = round(shiftedUV_Top.x);
	shiftedUV_Top.y = round(shiftedUV_Top.y);
	shiftedUV_Top.x *= heightToWidth;

	shiftedUV_Top.x /= size;
	shiftedUV_Top.y /= size;

	// bottom side

	shiftedUV_Bottom = UV;
	shiftedUV_Bottom.x = UV.x * (size * widthToHeight);
	shiftedUV_Bottom.y = (UV.y - offset) * size;

	shiftedUV_Bottom.x = round(shiftedUV_Bottom.x);
	shiftedUV_Bottom.y = round(shiftedUV_Bottom.y);
	shiftedUV_Bottom.x *= heightToWidth;

	shiftedUV_Bottom.x /= size;
	shiftedUV_Bottom.y /= size;
}