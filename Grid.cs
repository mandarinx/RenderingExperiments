using Godot;
using RenderingExperiments.Editor;

public class GridState {
	public Vector2 clickPosLocal;
	[Space]
	public int     cellsWidth  = 16;
	public int     cellsHeight = 9;
	public int     cellSize    = 90;
	[ReadOnly]
	public Vector2 gridSize;
	[ReadOnly]
	public Vector2 gridExtents;
	//[ImGuiButton]
	//private void RecalcGrid() {
	//  gridSize = ...
	//}
}

public partial class Grid : Node2D {

	private int[] burnable;

	private GridState state = new();
	public  GridState State => state;
	
	public override void _Ready() {
		burnable = new int[state.cellsWidth * state.cellsHeight];
		state.clickPosLocal = new Vector2(int.MinValue, int.MinValue);
		state.gridSize = new Vector2(state.cellsWidth * state.cellSize, state.cellsHeight * state.cellSize);
		state.gridExtents = new Vector2(state.cellsWidth * state.cellSize * 0.5f, state.cellsHeight * state.cellSize * 0.5f);
	}

	public override void _Input(InputEvent @event) {
		if (!@event.IsActionReleased("Click")) {
			return;
		}

		if (@event is InputEventMouseButton evtMouseBtn) {
			state.clickPosLocal = PixelsToLocal(evtMouseBtn.Position);

			if (IsOutsideGrid(state.clickPosLocal, state.gridExtents)) {
				return;
			}
			
			Vector2 coord = LocalToCoord(state.clickPosLocal, state.cellSize, state.gridExtents);
			int     i     = CoordToIndex(coord, state.cellsWidth);
			burnable[i] = 1000;

			QueueRedraw();
		}
	}

	public override void _Process(double delta) {
		state.gridSize = new Vector2(state.cellsWidth * state.cellSize, 
		                             state.cellsHeight * state.cellSize);
		state.gridExtents = new Vector2(state.cellsWidth * state.cellSize * 0.5f, 
		                                state.cellsHeight * state.cellSize * 0.5f);

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
			Vector2 coord         = IndexToCoord(i, state.cellsWidth);
			Vector2 local         = CoordToLocal(coord);
			DrawRect(new Rect2(local, new Vector2(state.cellSize, state.cellSize)), 
					 new Color(valueRelative, 0f, 0f, valueRelative));
		}
	}

	private void DrawGrid() {
		// vertical
		for (int i = 0; i <= state.cellsWidth; ++i) {
			float col = -state.gridExtents.X + i * state.cellSize;
			DrawLine(new Vector2(col, -state.gridExtents.Y), 
					 new Vector2(col,  state.gridExtents.Y), 
					 Colors.White);
		}
		// horizontal
		for (int i = 0; i <= state.cellsHeight; ++i) {
			float row = -state.gridExtents.Y + i * state.cellSize;
			DrawLine(new Vector2(-state.gridExtents.X, row), 
					 new Vector2(state.gridExtents.X, row), 
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
		coord *= state.cellSize;
		coord -= state.gridExtents;
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
