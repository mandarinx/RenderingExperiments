using System.Diagnostics;
using Godot;

public partial class Grid : Node2D {

	private Vector2 clickPosLocal;
	private int     cellsWidth  = 16;
	private int     cellsHeight = 9;
	private int     cellSize    = 90;
	private Vector2 gridSize;
	private Vector2 gridExtents;
	
	private int[]   burnable;

	public override void _Ready() {
		burnable = new int[cellsWidth * cellsHeight];
		clickPosLocal = new Vector2(int.MinValue, int.MinValue);
		gridSize = new Vector2(cellsWidth * cellSize, cellsHeight * cellSize);
		gridExtents = new Vector2(cellsWidth * cellSize * 0.5f, cellsHeight * cellSize * 0.5f);
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (!@event.IsActionReleased("Click")) {
			return;
		}

		if (@event is InputEventMouseButton evtMouseBtn) {
			clickPosLocal = PixelsToLocal(evtMouseBtn.Position);

			if (IsOutsideGrid(clickPosLocal, gridExtents)) {
				return;
			}
			
			Vector2 coord = LocalToCoord(clickPosLocal, cellSize, gridExtents);
			int     i     = CoordToIndex(coord, cellsWidth);
			burnable[i] = 1000;

			QueueRedraw();
		}
	}

	public override void _Process(double delta) {
		for (int i = 0; i < burnable.Length; ++i) {
			burnable[i] = Mathf.Max(burnable[i] - 1, 0);
		}
		QueueRedraw();
	}

	public override void _Draw() {
		DrawGrid();

		// Vector2 pixels = clickPosLocal + gridExtents;
		// int     x      = (int)(pixels.X - pixels.X % cellSize - gridExtents.X);
		// int     y      = (int)(pixels.Y - pixels.Y % cellSize - gridExtents.Y);
		//
		// DrawRect(new Rect2(new Vector2(x, y), 
		//                    new Vector2(cellSize, cellSize)), 
		//          Colors.Crimson);

		for (int i = 0; i < burnable.Length; ++i) {
			int value = burnable[i];
			if (value == 0) {
				continue;
			}
			
			float   valueRelative = Mathf.Clamp(value / 1000f, 0f, 1f);
			Vector2 coord         = IndexToCoord(i, cellsWidth);
			Vector2 local         = CoordToLocal(coord);
			DrawRect(new Rect2(local, new Vector2(cellSize, cellSize)), 
					 new Color(valueRelative, 0f, 0f, valueRelative));
		}
	}

	private void DrawGrid() {
		// vertical
		for (int i = 0; i <= cellsWidth; ++i) {
			float col = -gridExtents.X + i * cellSize;
			DrawLine(new Vector2(col, -gridExtents.Y), 
					 new Vector2(col,  gridExtents.Y), 
					 Colors.White);
		}
		// horizontal
		for (int i = 0; i <= cellsHeight; ++i) {
			float row = -gridExtents.Y + i * cellSize;
			DrawLine(new Vector2(-gridExtents.X, row), 
					 new Vector2(gridExtents.X, row), 
					 Colors.White);
		}
	}

	private static bool IsOutsideGrid(Vector2 local, Vector2 gridExtents) {
		return local.X > gridExtents.X
			   || local.X < -gridExtents.X
			   || local.Y > gridExtents.Y
			   || local.Y < -gridExtents.Y;
	}
	
	private Vector2 PixelsToLocal(Vector2 pixels) {
		Vector2 origin = GetViewportTransform().Origin;
		return pixels - origin;
	}
	
	private Vector2 CoordToLocal(Vector2 coord) {
		coord *= cellSize;
		coord -= gridExtents;
		return coord;
	}

	private static Vector2 LocalToCoord(Vector2 local, int cellSize, Vector2 gridExtents) {
		Vector2 coord = local + gridExtents;
		return new Vector2(Mathf.FloorToInt(coord.X / cellSize),
						   Mathf.FloorToInt(coord.Y / cellSize));
	}

	private static int CoordToIndex(Vector2 coord, int cellsWidth) {
		return Mathf.FloorToInt(coord.Y * cellsWidth + coord.X);
	}

	private static Vector2 IndexToCoord(int n, int cellsWidth) {
		int x = n % cellsWidth;
		int y = Mathf.FloorToInt(n / (float) cellsWidth);
		return new Vector2(x, y);
	}

	private static int GetValue(Vector2 coord, int cellsWidth, int[] arr) {
		int i = CoordToIndex(coord, cellsWidth);
		return arr[i];
	}
}
