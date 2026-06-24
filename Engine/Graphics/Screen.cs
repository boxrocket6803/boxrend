namespace Graphics;

public static class Screen {
	public static Vector2 Size {get; internal set;}
	public static float Ratio => Size.x / Size.y;
}