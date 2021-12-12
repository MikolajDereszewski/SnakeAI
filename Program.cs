using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeAI
{
	public static class GameData
	{
		public static (int, int) MapSize { get; private set; }
		public static (int, int) FruitPosition { get; private set; }
		private static Random Random { get; }

		static GameData()
		{
			Random = new Random(2137);
			MapSize = (20, 20);
			SetNewFruitPosition();
		}

		public static void SetNewFruitPosition() => FruitPosition = (Random.Next(0, MapSize.Item1), Random.Next(0, MapSize.Item2));

		public static bool IsPositionFruit((int, int) position) => (FruitPosition.Item1 == position.Item1 && FruitPosition.Item2 == position.Item2);
	}

	public class HeadSnakeElement : SnakeElement
	{
		public HeadSnakeElement((int, int) position) : base(position) { }

		public override bool TryMove((int, int) position)
		{
			if(position.Item1 < 0 || position.Item1 >= GameData.MapSize.Item1)
			{
				return TryMove((GameData.MapSize.Item1 - Math.Abs(position.Item1), position.Item2));
			}
			if (position.Item2 < 0 || position.Item2 >= GameData.MapSize.Item2)
			{
				return TryMove((position.Item1, GameData.MapSize.Item2 - Math.Abs(position.Item2)));
			}
			if (position.Item1 == GameData.FruitPosition.Item1 && position.Item2 == GameData.FruitPosition.Item2)
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
		public (int, int) Position { get; protected set; }
		public int Count => _nextElement?.Count + 1 ?? 1;

		protected SnakeElement _nextElement;

		public SnakeElement((int, int) position) => Position = position;

		public virtual bool TryMove((int, int) position)
		{
			(int, int) lastPosition = (Position.Item1, Position.Item2);
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

		public bool IsPositionOccupied((int, int) position)
		{
			if (position.Item1 == Position.Item1 && position.Item2 == Position.Item2)
			{
				return true;
			}
			return _nextElement?.IsPositionOccupied(position) ?? false;
		}
	}

	public class Program
	{
		private static readonly Dictionary<ConsoleKey, (int, int)> InputVectorMap = new Dictionary<ConsoleKey, (int, int)>
		{
			{ ConsoleKey.W, (0, 1)  },
			{ ConsoleKey.S, (0, -1) },
			{ ConsoleKey.D, (1, 0)  },
			{ ConsoleKey.A, (-1, 0) }
		};

		private static (int, int) ReadInput((int, int) lastInput)
		{
			(int, int) input = (0, 0);
			ConsoleKey key;
			do key = Console.ReadKey(false).Key;
			while (!InputVectorMap.Where(_ => _.Value.Item1 != -lastInput.Item1 || _.Value.Item2 != -lastInput.Item2).Select(_ => _.Key).Contains(key) || !InputVectorMap.TryGetValue(key, out input));
			return input;
		}

		private static void Repaint(HeadSnakeElement snakeHead)
		{
			Console.SetCursorPosition(0, 0);
			for (int i = GameData.MapSize.Item2 - 1; i >= 0; i--)
			{
				for (int j = 0; j < GameData.MapSize.Item1; j++)
				{
					Console.Write(snakeHead.IsPositionOccupied((j, i)) ? 'X' : GameData.IsPositionFruit((j, i)) ? 'O' : '.');
				}
				Console.WriteLine();
			}
		}

		private static void Main(string[] args)
		{
			(int, int) moveDirection = (1, 1);
			bool continueGame;
			HeadSnakeElement snakeHead = new HeadSnakeElement((GameData.MapSize.Item1 / 2, GameData.MapSize.Item2 / 2));
			do
			{
				Repaint(snakeHead);
				moveDirection = ReadInput(moveDirection);
				continueGame = snakeHead.TryMove((snakeHead.Position.Item1 + moveDirection.Item1, snakeHead.Position.Item2 + moveDirection.Item2));
			}
			while (continueGame);
			Console.WriteLine("Wynik gry: {0}", snakeHead.Count);
		}
	}
}
