using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;

namespace SecondDungeon.Source
{
	public class HitInfo
	{
		public string _info;
		public Vector2 _position;
		public int _TTL = 80;
		public Color _color = Color.Yellow;
	}

	[Serializable]
	public class PlayerStats
	{
		public Dictionary<string, int> TotalKilled { get; set; }
		public Dictionary<string, int> TempKilled { get; set; }
		public PlayerStats()
		{
			TempKilled = new Dictionary<string, int>();
			TotalKilled = new Dictionary<string, int>();
		}
	}

	public class Player : Figure
	{
		private List<HitInfo> _hitInfo = new List<HitInfo>();
		public PlayerStats PlayerStats { get; set; }

		private List<PickupItem> _inventory = new List<PickupItem>();
		public List<PickupItem> Inventory { get => _inventory; set => _inventory = value; }

		public Player()
		{
			Global.Player = this;
			Info = new FigureInfo();
			PlayerStats = new PlayerStats();
			
		}

		//public void RemoveItem(string itemName, int count = 1)
		//{
		//	foreach (var item in Inventory)
		//	{
		//		if (item.Name == itemName)
		//		{
		//			foundCount++;
		//		}
		//	}
		//}

		public void ResetTempStats()
		{
			PlayerStats.TempKilled.Clear();
		}

		public int HasKilled(string name)
		{
			if (PlayerStats.TotalKilled.ContainsKey(name))
			{
				return PlayerStats.TotalKilled[name];
			}
			return 0;
		}

		public int RecentKilled(string name)
		{
			if (PlayerStats.TempKilled.ContainsKey(name))
			{
				return PlayerStats.TempKilled[name];
			}
			return 0;
		}

		public bool HasItem(string itemName, int count = 1)
		{
			int foundCount = 0;
			foreach (var item in Inventory)
			{
				if (item.Name == itemName)
				{
					foundCount++;
				}
			}
			if (foundCount >= count)
			{
				return true;
			}
			return false;
		}

		public void AddItem(PickupItem item, bool effect = true)
		{
			Inventory.Add(item);

			// draw effect
			if (effect == true)
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo._info = "+" + item.Name;
				hitInfo._position = new Vector2(X * Global.SpriteWidth, Y * Global.SpriteHeight);
				_hitInfo.Add(hitInfo);
			}
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			for (int i = 0; i < _hitInfo.Count; i++)
			{
				spriteBatch.DrawString(font, _hitInfo[i]._info, _hitInfo[i]._position, _hitInfo[i]._color);
				_hitInfo[i]._position.Y -= 1;
				_hitInfo[i]._TTL--;
				//_hitInfo[i]._color *= 0.25f;
			}
			for (int i = _hitInfo.Count - 1; i >= 0; i--)
			{
				if (_hitInfo[i]._TTL <= 0)
				{
					_hitInfo.RemoveAt(i);
				}
			}

			spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepth.Figures);
		}

		public bool HandleInput(InputState inputState, Level level)
		{
			UI ui = UIState.GetUI();
			if (inputState.IsLeft(PlayerIndex.One))
			{
				int tempX = X - 1;
				if (level.IsWalkable(tempX, Y))
				{
					var npc = level.NpcAt(tempX, Y);
					if (npc == null)
					{
						X = tempX;
					}
					else
					{
						if (npc.Info.IsMerchant)
						{
							UIState._activeNpc = npc;
							ui.DrawShopWindow(true);
						}
						else if (npc.Info.IsEnemy)
						{
							Global.CombatManager.Attack(this, npc);
						}
						else
						{
							UIState._activeNpc = npc;
							ui.DrawDialogueWindow(true);
						}

					}
				}
				return true;
			}
			else if (inputState.IsRight(PlayerIndex.One))
			{
				int tempX = X + 1;
				if (level.IsWalkable(tempX, Y))
				{
					var npc = level.NpcAt(tempX, Y);
					if (npc == null)
					{
						X = tempX;
					}
					else
					{
						if (npc.Info.IsMerchant)
						{
							UIState._activeNpc = npc;
							ui.DrawShopWindow(true);
						}
						else if (npc.Info.IsEnemy)
						{
							Global.CombatManager.Attack(this, npc);
						}
						else
						{
							UIState._activeNpc = npc;
							ui.DrawDialogueWindow(true);
						}
					}
				}
				return true;
			}
			else if (inputState.IsUp(PlayerIndex.One))
			{
				int tempY = Y - 1;
				if (level.IsWalkable(X, tempY))
				{
					var npc = level.NpcAt(X, tempY);
					if (npc == null)
					{
						Y = tempY;
					}
					else
					{
						if (npc.Info.IsMerchant)
						{
							UIState._activeNpc = npc;
							ui.DrawShopWindow(true);
						}
						else if (npc.Info.IsEnemy)
						{
							Global.CombatManager.Attack(this, npc);
						}
						else
						{
							UIState._activeNpc = npc;
							ui.DrawDialogueWindow(true);
						}
					}
				}
				return true;
			}
			else if (inputState.IsDown(PlayerIndex.One))
			{
				int tempY = Y + 1;
				if (level.IsWalkable(X, tempY))
				{
					var npc = level.NpcAt(X, tempY);
					if (npc == null)
					{
						Y = tempY;
					}
					else
					{
						if (npc.Info.IsMerchant)
						{
							UIState._activeNpc = npc;
							ui.DrawShopWindow(true);
						}
						else if (npc.Info.IsEnemy)
						{
							Global.CombatManager.Attack(this, npc);
						}
						else
						{
							UIState._activeNpc = npc;
							ui.DrawDialogueWindow(true);
						}
					}
				}
				return true;
			}
			return false;
		}
	}

}
