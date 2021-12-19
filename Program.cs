using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeAI
{
	public static class GameData
	{
		public static (int w, int h) MapSize { get; private set; }
		public static (int x, int y) FruitPosition { get; private set; }
		private static Random Random { get; }

		static GameData()
		{
			Random = new Random(2137);
			MapSize = (20, 20);
			SetNewFruitPosition();
		}

		public static void SetNewFruitPosition() => FruitPosition = (Random.Next(0, MapSize.w), Random.Next(0, MapSize.h));

		public static bool IsPositionFruit((int x, int y) position) => (FruitPosition.x == position.x && FruitPosition.y == position.y);
	}

	public class HeadSnakeElement : SnakeElement
	{
		public HeadSnakeElement((int x, int y) position) : base(position) { }

		public override bool TryMove((int x, int y) position)
		{
			if(position.x < 0 || position.x >= GameData.MapSize.w)
			{
				return TryMove((GameData.MapSize.w - Math.Abs(position.x), position.y));
			}
			if (position.y < 0 || position.y >= GameData.MapSize.h)
			{
				return TryMove((position.x, GameData.MapSize.h - Math.Abs(position.y)));
			}
			if (position.x == GameData.FruitPosition.x && position.y == GameData.FruitPosition.y)
			{
				TryCreateSnakeElement();
				GameData.SetNewFruitPosition();
			}
			if (_nextElement?.IsPositionOccupied(position) ?? false)
			{
				return false;
			}
			return base.TryMove(position);
		}
	}

	public class SnakeElement
	{
		public (int x, int y) Position { get; protected set; }
		public int Count => _nextElement?.Count + 1 ?? 1;

		protected SnakeElement _nextElement;

		public SnakeElement((int, int) position) => Position = position;

		public virtual bool TryMove((int x, int y) position)
		{
			(int x, int y) lastPosition = (Position.x, Position.y);
			Position = position;
			return _nextElement?.TryMove(lastPosition) ?? true;
		}

		public void TryCreateSnakeElement()
		{
			if (_nextElement != null)
			{
				_nextElement.TryCreateSnakeElement();
				return;
			}
			_nextElement = new SnakeElement(Position);
		}

		public bool IsPositionOccupied((int x, int y) position)
		{
			if (position.x == Position.x && position.y == Position.y)
			{
				return true;
			}
			return _nextElement?.IsPositionOccupied(position) ?? false;
		}
	}

	public class Program
	{
		private static readonly Dictionary<ConsoleKey, (int x, int y)> InputVectorMap = new Dictionary<ConsoleKey, (int x, int y)>
		{
			{ ConsoleKey.W, (0, 1)  },
			{ ConsoleKey.S, (0, -1) },
			{ ConsoleKey.D, (1, 0)  },
			{ ConsoleKey.A, (-1, 0) }
		};

		private static (int x, int y) ReadInput((int x, int y) lastInput)
		{
			(int x, int y) input = (0, 0);
			ConsoleKey key;
			do key = Console.ReadKey(false).Key;
			while (!InputVectorMap.Where(_ => _.Value.x != -lastInput.x || _.Value.y != -lastInput.y).Select(_ => _.Key).Contains(key) || !InputVectorMap.TryGetValue(key, out input));
			return input;
		}

		private static void Repaint(HeadSnakeElement snakeHead)
		{
			Console.SetCursorPosition(0, 0);
			for (int y = GameData.MapSize.h - 1; y >= 0; y--)
			{
				for (int x = 0; x < GameData.MapSize.w; x++)
				{
					Console.Write(snakeHead.IsPositionOccupied((x, y)) ? 'X' : GameData.IsPositionFruit((x, y)) ? 'O' : '.');
				}
				Console.WriteLine();
			}
		}

		private static void Main(string[] args)
		{
			(int x, int y) moveDirection = (1, 1);
			bool continueGame;
			HeadSnakeElement snakeHead = new HeadSnakeElement((GameData.MapSize.w / 2, GameData.MapSize.h / 2));
			do
			{
				Repaint(snakeHead);
				moveDirection = ReadInput(moveDirection);
				continueGame = snakeHead.TryMove((snakeHead.Position.x + moveDirection.x, snakeHead.Position.y + moveDirection.y));
			}
			while (continueGame);
			Console.WriteLine("Wynik gry: {0}", snakeHead.Count);
		}
	}
}
