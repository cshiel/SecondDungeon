using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	public enum ScriptEvaluation
	{
		PlayerHasItem,
		PlayerTalkedToNpc,
		PlayerHasKilledNpc,
		PlayerHasKilledMultipleNpc,
		PlayerHasCompletedQuest
	}

	public class QuestTask
	{
		public string Description { set; get; }
		public bool Completed { get; }

		public ScriptEvaluation Script { get; set; }
		public string ObjectName { get; set; }
		public int Count { get; set; }

		public QuestTask(ScriptEvaluation script, string objectName, int count, string description)
		{
			Script = script;
			Description = description;
			ObjectName = objectName;
			Count = count;
		}

		public bool Evaluate()
		{
			bool completed = false;
			switch (Script)
			{
				case ScriptEvaluation.PlayerHasItem:
					{
						completed = Global.Player.HasItem(ObjectName, Count);
					}
					break;
				case ScriptEvaluation.PlayerHasKilledNpc:
					{
						int result = Global.Player.HasKilled(ObjectName);
						if (result >= 1)
						{
							completed = true;
						}
					}
					break;
				case ScriptEvaluation.PlayerHasKilledMultipleNpc:
					{
						int result = Global.Player.RecentKilled(ObjectName);
						if (result >= Count)
						{
							completed = true;
						}
					}
					break;
				case ScriptEvaluation.PlayerTalkedToNpc:
					break;
				default: break;
			}
			return completed;
		}
	}

	public class Quest
	{
		public enum QuestStage
		{
			NotStarted,
			Started,
			Doing,
			Completed,
			Failed,
			Aborted
		}

		private QuestStage _questStage = QuestStage.NotStarted;

		public string Name { get; set; }
		public List<QuestTask> Tasks { get; set; }
		public List<ScriptObject> IntroScripts { get; set; }
		public List<ScriptObject> SuccessScripts { get; set; }
		public List<ScriptObject> FailedScripts { get; set; }
		public bool Done()
		{
			if (_questStage == QuestStage.Completed || _questStage == QuestStage.Failed || _questStage == QuestStage.Aborted)
				return true;
			return false;
		}

		public Quest(string name)
		{
			Name = name;
			Tasks = new List<QuestTask>();
			IntroScripts = new List<ScriptObject>();
			SuccessScripts = new List<ScriptObject>();
			FailedScripts = new List<ScriptObject>();
		}

		public void Start()
		{
			_questStage = QuestStage.Started;

			foreach (var script in IntroScripts)
			{
				ScriptHelpers.Execute(script);
			}

			_questStage = QuestStage.Doing;
		}

		public void Update()
		{
			if (_questStage != QuestStage.Doing)
				return;

			if (Tasks.Count <= 0)
			{
				_questStage = QuestStage.Completed;
			}

			for (int i = Tasks.Count - 1; i >= 0; i--)
			{
				if (Tasks[i].Evaluate())
				{
					Tasks.RemoveAt(i);
				}
				else
				{
					_questStage = QuestStage.Failed;
				}
			}

			if (_questStage == QuestStage.Failed)
			{
				foreach (var script in FailedScripts)
				{
					ScriptHelpers.Execute(script);
				}
			}

			if (_questStage == QuestStage.Completed)
			{
				foreach (var script in SuccessScripts)
				{
					ScriptHelpers.Execute(script);
				}
			}
		}
	}

	public class QuestHelper
	{
		private static List<Quest> _quests = new List<Quest>();

		public static void AddQuest(Quest quest)
		{
			_quests.Add(quest);
		}

		public static void Update()
		{
			for (int i = _quests.Count - 1; i >= 0; i--)
			{
				_quests[i].Update();
				if (_quests[i].Done())
				{
					// remove quest
					_quests.RemoveAt(i);
				}
			}
		}
	}
}
