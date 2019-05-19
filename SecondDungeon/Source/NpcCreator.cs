using Microsoft.Xna.Framework.Graphics;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	// todo merge with aggressive enemy
	[Serializable]
	public class FigureInfo
	{
		public int AttackBonus { get; set; }
		public int ArmorClass { get; set; }
		public int MeleeAttack { get; set; }
		public int RangedAttack { get; set; }
		public int Health { get; set; }
		public int XP { get; set; }
		public string Name { get; set; }
		public int Gold { get; set; }
		public int TextureID { get; set; }
		public bool IsMerchant { get; set; }
		public bool IsEnemy { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public List<int> Inventory { get; set; }
		public string DialogueRoot { get; set; }
	}

	public class NpcCreator
	{
		private static List<FigureInfo> _templates = new List<FigureInfo>();

		public static void AddNpcType(FigureInfo info)
		{
			_templates.Add(info);
		}

		public static Npc CreateNpc(FigureInfo info, Level level, int x, int y)
		{
			Npc npc = null;
			//Level level = LevelManager.GetCurrentLevel();
			//if (info.IsEnemy)
			{
				Texture2D tex = null;
				TileHelper.GetTileTexture(0, ref tex);
				var pathFromAggressiveEnemy =
					new PathToPlayer(Global.Player, level.Map, tex);
				pathFromAggressiveEnemy.CreateFrom(x, y);

				npc = new Npc(level.Map, level, pathFromAggressiveEnemy)
				{
					X = x,
					Y = y,
					Sprite = UIState._selectedTexture,
					Damage = Dice.Parse("d3"),
				};

				npc.Info = info;

				if (npc.Info.IsMerchant)
				{
					npc.Info.Inventory = new List<int>();
					npc.Info.Inventory.Add(2569);
					npc.Info.Inventory.Add(2569);
					npc.Info.Inventory.Add(2569);
				}
				//level.AddNpc(npc);
			}
			//else
			//{
			//}

			return npc;
		}
	}
}
