using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	[Serializable]
	public class PickupItem
	{
		public enum PickupType
		{
			Consumable,
			Wearable,

		}

		public PickupItem()
		{
			SpriteType = 200;
		}

		public PickupItem(TileInfo info)
		{
			Name = info._name;
			Description = info._description;
			Script = info._script;
			UseOnPickup = info._useOnInteract;
			SpriteType = info._tileType;
		}

		public void Pickup()
		{
			if (UseOnPickup)
			{
				Use();
			}
			else
			{
				Global.Player.AddItem(this);
			}
		}

		public void Use()
		{
			ScriptHelpers.Execute(Script, Parameter);
		}

		public bool UseOnPickup { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ScriptDescription Script;
		public object Parameter;
		public int X { get; set; }
		public int Y { get; set; }
		public int SpriteType { get; set; }
	}
}
