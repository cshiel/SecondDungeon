using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.Random;

namespace SecondDungeon.Source
{
	public enum GameStates
	{
		None = 0,
		PlayerTurn = 1,
		EnemyTurn = 2,
		Dialogue = 3,
		Debugging = 4,
		Editor = 5,
		Quit = 6,
		Paused = 7,
		MainMenu = 8
	}
	public class Global
	{
		public static readonly int MapWidth = 50;
		public static readonly int MapHeight = 30;
		public static readonly int SpriteWidth = 32;
		public static readonly int SpriteHeight = 32;
		public static CombatManager CombatManager;
		public static readonly Camera Camera = new Camera();
		public static readonly IRandom Random = new DotNetRandom();
		public static GameStates GameState { get; set; }
		public static Player Player;
		public static bool StartedGame { get; set; }
	}

	public class ScreenLayout
	{
		public static int LeftPane = 20;
	}
}
