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
	public class Npc : Figure
	{
		private readonly PathToPlayer _path;
		private readonly IMap _map;
		private bool _isAwareOfPlayer;

		//private DialogueTree _dialogueTree;
		//public DialogueTree DialogueTree { get => _dialogueTree; set => _dialogueTree = value; }

		public int DialogueRoot { get; set; }

		public Npc(IMap map, PathToPlayer path)
		{
			_map = map;
			_path = path;
			_isAwareOfPlayer = false;
			//_dialogueTree = new DialogueTree();
			//Info = new FigureInfo();
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepth.Figures);
			_path.Draw(spriteBatch);
		}

		public void Update()
		{
			if (!_isAwareOfPlayer)
			{
				// When the enemy is not aware of the player
				// check the map to see if they are in field-of-view
				if (_map.IsInFov(X, Y))
				{
					_isAwareOfPlayer = true;
				}
			}
			// Once the enemy is aware of the player
			// they will never lose track of the player
			// and will pursue relentlessly
			if (_isAwareOfPlayer)
			{
				if (Info.IsEnemy)
				{
					_path.CreateFrom(X, Y);
					// Use the CombatManager to check if the player located
					// at the cell we are moving into
					if (Global.CombatManager.IsPlayerAt(_path.FirstCell.X, _path.FirstCell.Y))
					{
						// Make an attack against the player
						Global.CombatManager.Attack(this,
						  Global.CombatManager.FigureAt(_path.FirstCell.X, _path.FirstCell.Y));
					}
					else
					{
						// Since the player wasn't in the cell, just move into as normal
						X = _path.FirstCell.X;
						Y = _path.FirstCell.Y;
					}
				}
			}
		}
	}
}
