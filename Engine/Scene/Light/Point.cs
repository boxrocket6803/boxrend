namespace Scene.Light;

public class Point : Base {
	public Vector3 Color { //TODO replace with Color type
		get; 
		set {
			if (value == field)
				return;
			//TODO Scene.Attributes.Set($"Lights[{Index}].Color", value);
			field = value;
		}
	}
}