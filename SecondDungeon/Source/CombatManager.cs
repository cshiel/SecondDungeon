using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp.DiceNotation;

namespace SecondDungeon.Source
{
	public class MagicAttack
	{
		private Figure _caster;
		private Texture2D _fire;
		private float _radius = 0;
		private bool _done = false;
		private int _damage;
		private float _speed;
		private int _range;
		private float _targetX;
		private float _targetY;

		public MagicAttack(Figure caster, int textureID, int damage, int tx, int ty, int range = 6, float speed = 0.25f)
		{
			_caster = caster;
			//_xi = 1;
			//_yi = 0;// 0;
			_targetX = caster.X;
			_targetY = caster.Y;
			TileHelper.GetTileTexture(textureID, ref _fire);
			_damage = damage;
			_speed = speed;
			_range = range;
		}

		public void Draw(SpriteBatch spriteBatch, Figure figure)
		{
			if (_done)
				return;

			var level = LevelManager.GetCurrentLevel();
			var map = level.Map;
			int x = figure.X;
			int y = figure.Y;
			bool casterIsPlayer = (x == Global.Player.X && y == Global.Player.Y);
			//var cells = map.GetCellsInCircle(x, y, (int)_radius);
			//var cells = map.GetBorderCellsInCircle(x, y, (int)_radius);
			//var cells = map.GetCellsAlongLine(_caster.X, _caster.Y, _targetX, _targetY);
			
			_targetX += _speed;
			
			var cell = map.GetCell((int)_targetX, (int)_targetY);

			//foreach (var cell in cells)
			{
				if (cell.IsWalkable)
				{
					spriteBatch.Draw(_fire, new Vector2((cell.X * Global.SpriteWidth)+ Global.SpriteWidth/2, (cell.Y * Global.SpriteHeight)+ Global.SpriteHeight/2), null, null, null, 0.0f, Vector2.One * 0.5f, Color.White, SpriteEffects.None, LayerDepth.Figures);
					// if caster is player
					if (casterIsPlayer)
					{
						var npc = level.NpcAt(cell.X, cell.Y);
						if (npc != null)
						{
							Global.CombatManager.Attack(_caster, npc);
						}
					}
					else
					{
						// if caster is not player
						if (cell.X == Global.Player.X && cell.Y == Global.Player.Y)
						{
							Global.CombatManager.Attack(_caster, Global.Player);
						}
					}

				}
			}
			_radius += _speed;
			if (_radius >= _range)
			{
				_radius = 0;
				_done = true;
			}
		}
	}

	public class CombatManager
	{
		private readonly Player _player;
		private readonly List<Npc> _enemies;

		//private List<HitInfo> _hitInfo = new List<HitInfo>();
		private Dictionary<string, HitInfo> _hitInfo = new Dictionary<string, HitInfo>();

		public CombatManager(Player player, List<Npc> enemies)
		{
			_player = player;
			_enemies = enemies;
		}

		public void Attack(Figure attacker, Figure defender)
		{
			string info = " *MISS* ";

			//SoundPlayer.PlaySound(Sound.MeleeAttack);

			if (Dice.Roll("d20") + attacker.Info.AttackBonus >= defender.Info.ArmorClass)
			{
				int damage = attacker.Damage.Roll().Value;
				defender.Info.Health -= damage;
				info = " -" + damage.ToString() + " ";
				Debug.WriteLine("{0} hit {1} for {2} and he has {3} health remaining.", attacker.Info.Name, defender.Info.Name, damage, defender.Info.Health);

				if (defender.Info.Health <= 0)
				{
					if (defender is Npc)
					{
						var enemy = defender as Npc;
						enemy.Alive = false;
						_enemies.Remove(enemy);
						info = " *DEAD* ";

					}
					Debug.WriteLine("{0} killed {1}", attacker.Info.Name, defender.Info.Name);
				}
			}
			else
			{
				Debug.WriteLine("{0} missed {1}", attacker.Info.Name, defender.Info.Name);
			}

			// draw effect
			HitInfo hitInfo = new HitInfo();
			hitInfo._info = info;
			hitInfo._key = attacker.Info.Name;
			if (attacker.Info.Name == Global.Player.Info.Name)
			{
				hitInfo._color = Color.Red;
			}
			else
			{
				hitInfo._color = Color.Green;
			}

			hitInfo._position = new Vector2(defender.X * Global.SpriteWidth, defender.Y * Global.SpriteHeight);
			if (_hitInfo.ContainsKey(attacker.Info.Name) == false)
			{
				_hitInfo.Add(attacker.Info.Name, hitInfo);
			}
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			//for (int i = 0; i < _hitInfo.Count; i++)
			var hiLookupCopy = _hitInfo.Values.ToList<HitInfo>();
			foreach (var hi in hiLookupCopy)
			{
				//spriteBatch.DrawString(font, _hitInfo[i]._info, _hitInfo[i]._position, _hitInfo[i]._color);
				//_hitInfo[i]._position.Y -= 1;
				//_hitInfo[i]._TTL--;
				spriteBatch.DrawString(font, hi._info, hi._position, hi._color);
				var hiCopy = hi;
				hiCopy._position.Y -= 1;
				hiCopy._TTL--;
				_hitInfo[hi._key] = hiCopy;
			}
			
			foreach (var hi in hiLookupCopy)
			{
				if (hi._TTL <= 0)
				{
					_hitInfo.Remove(hi._key);
				}
			}
			//for (int i = _hitInfo.Count-1; i >= 0; i--)
			//{
				//if (_hitInfo[i]._TTL <= 0)
				//{
				//	_hitInfo.RemoveAt(i);
				//}
			//}
		}

		public Figure FigureAt(int x, int y)
		{
			if (IsPlayerAt(x, y))
			{
				return _player;
			}
			return EnemyAt(x, y);

		}

		public bool IsPlayerAt(int x, int y)
		{
			return (_player.X == x && _player.Y == y);
		}

		public Npc EnemyAt(int x, int y)
		{
			foreach (var enemy in _enemies)
			{
				if (enemy.X == x && enemy.Y == y)
				{
					return enemy;
				}
			}
			return null;
		}

		public bool IsEnemyAt(int x, int y)
		{
			return EnemyAt(x, y) != null;
		}
	}
}
