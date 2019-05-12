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
	public class CombatManager
	{
		private readonly Player _player;
		private readonly List<Npc> _aggressiveEnemies;
		
		private List<HitInfo> _hitInfo = new List<HitInfo>();

		public CombatManager(Player player, List<Npc> aggressiveEnemies)
		{
			_player = player;
			_aggressiveEnemies = aggressiveEnemies;
		}

		public void Attack(Figure attacker, Figure defender)
		{
			string info = " *MISS* ";

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
						_aggressiveEnemies.Remove(enemy);
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
			if (attacker.Info.Name == Global.Player.Info.Name)
			{
				hitInfo._color = Color.Red;
			}
			else
			{
				hitInfo._color = Color.Green;
			}

			hitInfo._position = new Vector2(attacker.X * Global.SpriteWidth, attacker.Y * Global.SpriteHeight);
			_hitInfo.Add(hitInfo);
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			for (int i = 0; i < _hitInfo.Count; i++)
			{
				spriteBatch.DrawString(font, _hitInfo[i]._info, _hitInfo[i]._position, _hitInfo[i]._color);
				_hitInfo[i]._position.Y -= 1;
				_hitInfo[i]._TTL--;
			}
			for (int i = _hitInfo.Count-1; i >= 0; i--)
			{
				if (_hitInfo[i]._TTL <= 0)
				{
					_hitInfo.RemoveAt(i);
				}
			}
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
			foreach (var enemy in _aggressiveEnemies)
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
