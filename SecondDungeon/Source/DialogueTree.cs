using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	[Serializable]
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
		public static List<string> Dialogues = new List<string>();
		public static Dictionary<string, int> DialogueLookup = new Dictionary<string, int>();

		public static int NextID { get { return _id; } }
		public static int AddRoot(string name, string text)
		{
			_id++;
			Dialogues.Add(name);
			DialogueLookup[name] = _id;
			DialogueNode item = new DialogueNode(text, _id, null);
			if (DialogueHelper.DialogueItems.ContainsKey(_id))
			{
			}
			else
			{
				DialogueHelper.DialogueItems.Add(_id, item);
				Dialogues.Add(name);
			}
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

		public static int GetDialogueId(string rootName)
		{
			int id = DialogueLookup[rootName];
			return id;
		}

		public static List<DialogueNode> GetLeaves(int id)
		{
			return DialogueItems[id].Children;
		}
	}
}
