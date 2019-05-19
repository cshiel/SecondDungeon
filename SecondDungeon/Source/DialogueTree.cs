using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	public class DialogueNode
	{
		public DialogueNode Parent { get; set; }
		public List<DialogueNode> Children { get; set; }
		public string Text { get; set;  }
		public int Id { get; set; }
		public ScriptObject Script { get; set; }

		public DialogueNode(string text, int id, DialogueNode parent)
		{
			Text = text;
			Id = id;
			Parent = parent;
			Children = new List<DialogueNode>();
		}
	}

	public class DialogueHelper
	{
		private static int _id = 0;
		public static Dictionary<int, DialogueNode> DialogueItems = new Dictionary<int, DialogueNode>();

		public static int NextID { get { return _id; } }
		public static int AddRoot(string text)
		{
			_id++;
			DialogueNode item = new DialogueNode(text, _id, null);
			DialogueHelper.DialogueItems.Add(_id, item);
			return _id;
		}

		public static int AddLeaf(string text, int parentId)
		{
			_id++;
			var parent = DialogueHelper.DialogueItems[parentId];
			var leaf = new DialogueNode(text, _id, parent);
			parent.Children.Add(leaf);
			DialogueHelper.DialogueItems.Add(_id, leaf);
			return _id;
		}

		public static DialogueNode GetDialogue(int id)
		{
			return DialogueItems[id];
		}

		public static List<DialogueNode> GetLeaves(int id)
		{
			return DialogueItems[id].Children;
		}
	}
}
