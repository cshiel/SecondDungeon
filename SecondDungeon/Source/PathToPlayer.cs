using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;

namespace SecondDungeon.Source
{
	public class PathToPlayer
	{
		private readonly Player _player;
		private readonly IMap _map;
		private readonly Texture2D _sprite;
		private readonly PathFinder _pathFinder;
		private Path _path;

		public PathToPlayer(Player player, IMap map, Texture2D sprite)
		{
			_player = player;
			_map = map;
			_sprite = sprite;
			_pathFinder = new PathFinder(map);
		}
		public Cell FirstCell
		{
			get
			{
				return (Cell)_path.CurrentStep;
			}
		}
		public void CreateFrom(int x, int y)
		{
			_path = _pathFinder.ShortestPath(_map.GetCell(x, y), _map.GetCell(_player.X, _player.Y));
			_path.TryStepForward();
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			if (_path != null && Global.GameState == GameStates.Debugging)
			{
				foreach (Cell cell in _path.Steps)
				{
					if (cell != null)
					{
						float multiplier = _sprite.Width;
						spriteBatch.Draw(_sprite, new Vector2(cell.X * multiplier, cell.Y * multiplier), null, null, null, 0.0f, Vector2.One, Color.Blue * .2f, SpriteEffects.None, LayerDepth.Paths);
					}
				}
			}
		}
	}
}
