using System.Reflection;
using Godot;

public partial class Dev : Node {

	[Export] public ColorRect            grabHandle;
	[Export] public SubViewportContainer subViewportContainer;

	private Vector2 mousePos   = Vector2.Zero;
	private Vector2 subviewPos = Vector2.Zero;
	private Vector2 clickPos   = Vector2.Zero;
	private float   xMargin;
	private bool    isScaling;
	private bool    isMoving;

	public GridState   gridState;
	public FieldInfo[] fields;
	public bool        blockInput;

	public override void _Ready() {
		// Load game
		var scene     = ResourceLoader.Load<PackedScene>("res://Grid.tscn");
		var sceneNode = scene.Instantiate();
		var parent = GetNode<SubViewport>(new NodePath("Control/SubViewportContainer/SubViewport"));
		parent.AddChild(sceneNode);
		
		// Is it possible to pipe the viewport to an imgui image?

		OnMouseExit();
		subViewportContainer.MouseExited += OnMouseExit;

		if (sceneNode is Grid grid) {
			gridState = grid.State;
			fields = gridState.GetType().GetFields(BindingFlags.NonPublic | 
			                                       BindingFlags.Instance | 
			                                       BindingFlags.Public);
		}
		
		SetProcess(true);
	}

	public override void _Process(double delta) {
		if (subViewportContainer.GetRect().HasPoint(mousePos)) {
			grabHandle.Color = new Color(1f, 1f, 1f, 0.3f);
		}

		if (grabHandle.GetRect().HasPoint(mousePos)) {
			grabHandle.Color = new Color(1f, 1f, 1f, 0.8f);
		}

		Rect2 vpRect   = subViewportContainer.GetRect();
		Rect2 grabRect = grabHandle.GetRect();

		if (isScaling) {
			Vector2 scaleDt = mousePos - vpRect.Position;
			float   scale   = (scaleDt.X + xMargin) / 1600f;
			subViewportContainer.Scale = new Vector2(scale, scale);
		}
		if (isMoving) {
			Vector2 mouseDt = mousePos - clickPos;
			subViewportContainer.SetPosition(subviewPos + mouseDt);
		}

		grabHandle.SetPosition(vpRect.Position + vpRect.Size - grabRect.Size);
	}

	public override void _Input(InputEvent @event) {
		if (blockInput) {
			return;
		}
		
		if (@event is InputEventMouse mouseEvt) {
			mousePos = mouseEvt.Position;
		}
		if (@event is InputEventMouseButton {
				ButtonIndex: MouseButton.Left
			} mouseBtnEvt) {

			Rect2 grabRect = grabHandle.GetRect();
			Rect2 vpRect   = subViewportContainer.GetRect();

			if (mouseBtnEvt.IsPressed()) {
				if (grabRect.HasPoint(mousePos)) {
					isScaling = true;
					isMoving  = false;
				} else if (vpRect.HasPoint(mousePos)) {
					isScaling = false;
					isMoving  = true;
				}

				clickPos       = mouseBtnEvt.Position;
				subviewPos     = subViewportContainer.Position;
				xMargin = vpRect.Position.X + vpRect.Size.X - mousePos.X;
			}
			if (mouseBtnEvt.IsReleased()) {
				isScaling = false;
				isMoving  = false;
			}
		}
	}

	private void OnMouseExit() {
		grabHandle.Color = new Color(1f, 1f, 1f, 0.1f);
	}
}
