using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	[Serializable]
	public class LevelObject
	{
		public LevelObject()
		{
			SpriteType = 150;
		}

		public void Interact()
		{
			ScriptHelpers.Execute(Script, Parameter);
		}

		public ScriptDescription Script;
		public object Parameter;

		public int X { get; set; }
		public int Y { get; set; }
		public int SpriteType { get; set; }
	}
}
