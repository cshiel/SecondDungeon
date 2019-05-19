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
		private readonly Level _level;
		private bool _isAwareOfPlayer;

		public bool Alive { get; set; }

		//private DialogueTree _dialogueTree;
		//public DialogueTree DialogueTree { get => _dialogueTree; set => _dialogueTree = value; }

		public int DialogueRoot { get; set; }

		public Npc(IMap map, Level level, PathToPlayer path)
		{
			Alive = true;
			_map = map;
			_level = level;
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
			if (Alive == false)
			{
				_level.RemoveNpc(this);
				return;
			}

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
						SoundPlayer.PlaySound(Sound.MeleeAttack);
					}
					else
					{
						// Since the player wasn't in the cell, just move into as normal
						_level.MoveNpc(this, X, Y, _path.FirstCell.X, _path.FirstCell.Y);
						//X = _path.FirstCell.X;
						//Y = _path.FirstCell.Y;
					}
				}
			}
		}
	}
}
