using System.Text;

namespace TuiCommon;

public class MazeGeneratingPathfinder(Cell startPoint, int gridSize) {
	public bool GetRunning() => _running;
	private bool _running;
	public int VisitedCellCount = 1;

	private readonly Random _random = new();
	
	/// <exception cref="RunningException">thrown if it's already running</exception>
	public bool[,] Pathfind() {
		if (_running) throw new RunningException();
		_running = true;
		bool[,] walls;
		try {
			bool[,] objects = new bool[gridSize, gridSize];
			walls = new bool[gridSize * 2, gridSize];

			for (int y = 0; y < walls.GetLength(0); y++)
			for (int x = 0; x < walls.GetLength(1); x++)
				walls[y, x] = true;

			HashSet<Cell> memory = new();
			Stack<Cell> stepHistory = new();

			int[] dx = { -1, 1, 0, 0 };
			int[] dy = { 0, 0, 1, -1 };

			Cell runner = startPoint;
			memory.Add(runner);
			List<(Cell, int)> directions = new(4);
			while (true)
			{
				directions.Clear();
				for (int dir = 0; dir < 4; dir++)
				{
					Cell checking = new(dx[dir], dy[dir]);
					Cell newCoord = new(runner.X + checking.X, runner.Y + checking.Y);
					if (memory.Contains(newCoord) || !IsInBounds(objects, newCoord.X, newCoord.Y)) continue;
					directions.Add((newCoord, dir));
				}

				if (directions.Count == 0)
				{
					if (stepHistory.Count <= 1) break;
					stepHistory.Pop();
					runner = stepHistory.Peek();
					continue;
				}

				var rand = directions[_random.Next(0, directions.Count)];
				Cell wallFormatPoint = new(runner.X, runner.Y * 2);
				runner = rand.Item1;
				memory.Add(runner);
				VisitedCellCount++;
				stepHistory.Push(runner);

				objects[runner.Y, runner.X] = true;

				int direction = rand.Item2;
				Cell checkingWallFormat = new(direction == 0 ? 0 : dx[direction], dy[direction]);
				Cell wallCoord = new(wallFormatPoint.X + checkingWallFormat.X, wallFormatPoint.Y + checkingWallFormat.Y);
				walls[wallCoord.Y, wallCoord.X] = false;

				/*skip:
				string firstLine = $"{(int)((float)memory.Count/objects.Length * 100)}% {runner} {MazeGen.IsInBounds(objects, runner.X, runner.Y)}";
				Debug.Log(MazeGen.PrintBoolArray(objects, firstLine));
				Debug.Log(MazeGen.WallRep(walls, firstLine));*/
			}
		}
		finally {
			_running = false;
		}
		
		return walls;
	}
	
	public static string WallRepresentation(bool[,] array2D, string firstLine = "")
	{
		StringBuilder builder = new(firstLine.Length + array2D.Length * 4); // 4 is the number of characters each spacing takes
		if (!string.IsNullOrEmpty(firstLine))
			builder.AppendLine(firstLine);
        
		int lines = array2D.GetLength(0);
		int cells = array2D.GetLength(1);
		bool floor = false;
		for (int y = 1; y < lines; y++) {
			for (int x = 0; x < cells; x++)
				builder.Append(array2D[lines -1 -y, cells -1 -x] ? floor ? "----" : "   |" : "    ");
			builder.AppendLine();
			floor = !floor;
		}

		return builder.ToString();
	}
	
	public static string PrintBoolArray(bool[,] array2D, string firstLine = "")
	{
		string mraw = firstLine + "\n";
		int lines = array2D.GetLength(0);
		int cells = array2D.GetLength(1);
		for (int y = 0; y < lines; y++) {
			for (int x = 0; x < cells; x++)
				mraw += array2D[lines -1 - y, cells -1 - x] ? "W" : " ";
			mraw += "\n";
		}
		return mraw;
	}
    
	public static bool IsInBounds<T>(T[,] array, int x, int y){
		if (x < 0) return false;
		if (y < 0) return false;
		if (y >= array.GetLength(0)) return false;
		if (x >= array.GetLength(1)) return false;
		return true;
	}
    
	public static bool IsInBounds<T>(T[][] array, int x, int y){
		if (x < 0) return false;
		if (y < 0) return false;
		if (y >= array.Length) return false;
		if (x >= array[y].Length) return false;
		return true;
	}
}

public readonly struct Cell : IEquatable<Cell> {
	public readonly int X, Y;
	public Cell(int x, int y) { X = x; Y = y; }
	public bool Equals(Cell other) => X == other.X && Y == other.Y;
	public override bool Equals(object? obj) => obj is Cell other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(X, Y);
}

public class RunningException : Exception;