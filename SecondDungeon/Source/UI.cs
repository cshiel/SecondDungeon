using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace SecondDungeon.Source
{
	public enum PaintMode
	{
		Tiles,
		NPCs,
	};

	public class UIState
	{
		public static TileInfo _selected;
		public static Texture2D _selectedTexture;
		public static PaintMode _paintMode = PaintMode.Tiles;

		public static int _mouseX;
		public static int _mouseY;
		public static int _beginIndex = 0;

		public static FigureInfo _npcTemplate;
		public static Npc _activeNpc;

		private static UI _ui;
		public static void AddUI(UI ui) { _ui = ui; }
		public static UI GetUI() { return _ui; }
	}

	public class UI
	{
		private Desktop _host;
		private Game _game;
		private GraphicsDeviceManager _graphics;
		private List<Texture2D> _textures;
		private Level _level;
		private Texture2D _whiteTex;
		private bool _painting = false;

		public UI(Game game, GraphicsDeviceManager graphics, List<Texture2D> textures, Level level)
		{
			_game = game;
			_graphics = graphics;
			_textures = textures;
			_level = level;
			TileHelper.GetTile(0, ref UIState._selected);
			TileHelper.GetTileTexture(0, ref UIState._selectedTexture);

			TileHelper.Load();

			SetupUI();
		}

		// EDITOR windows
		public class TilePropertiesWindow
		{
			public Button saveBtn;
			public RadioButton placeModeRadio;
			public RadioButton editModeRadio;
			public TextField layerDepthTF;
			public CheckBox walkableCB;
			public CheckBox destructsCB;
			public CheckBox isDoorCB;
			public CheckBox isItemCB;
			public CheckBox isObjCB;
			public TextField nameTF;
			public TextField descriptionTF;
			public ComboBox scriptCB;
			public TextField parameterTF;

			private ScrollPane scrollPane;

			public void Show(bool visible)
			{
				scrollPane.Visible = visible;
			}

			public TilePropertiesWindow(Grid parent)
			{
				scrollPane = new ScrollPane
				{
					GridPositionX = 3,
					GridPositionY = 2
				};

				var newTileGrid = new Grid
				{
					RowSpacing = 5,
					ColumnSpacing = 5
				};
				for (int i = 0; i < 11; i++)
				{
					newTileGrid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				newTileGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				newTileGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				layerDepthTF = new TextField
				{
					GridColumn = 1,
					GridPositionY = 0,
					Text = "0.7"
				};
				var layerTxt = new TextField
				{
					GridColumn = 0,
					Text = "Layer Depth",
					GridPositionY = 0
				};
				newTileGrid.Widgets.Add(layerTxt);
				walkableCB = new CheckBox
				{
					GridColumn = 1,
					Text = "Walkable",
					GridPositionY = 1
				};
				destructsCB = new CheckBox
				{
					GridColumn = 1,
					Text = "Use on pickup",
					GridPositionY = 2
				};
				isDoorCB = new CheckBox
				{
					GridColumn = 1,
					Text = "Is Door",
					GridPositionY = 3
				};
				isItemCB = new CheckBox
				{
					GridColumn = 1,
					Text = "Is Pickup Item",
					GridPositionY = 4
				};
				isObjCB = new CheckBox
				{
					GridColumn = 1,
					Text = "Is Object",
					GridPositionY = 5
				};
				nameTF = new TextField
				{
					GridColumn = 1,
					Text = "<Name>",
					GridPositionY = 6
				};
				var nameTxt = new TextField
				{
					GridColumn = 0,
					Text = "Name",
					GridPositionY = 6
				};
				newTileGrid.Widgets.Add(nameTxt);
				descriptionTF = new TextField
				{
					GridColumn = 1,
					Text = "<Description>",
					GridPositionY = 7
				};
				var descripTxt = new TextField
				{
					GridColumn = 0,
					Text = "Description",
					GridPositionY = 7
				};
				newTileGrid.Widgets.Add(descripTxt);
				scriptCB = new ComboBox
				{
					GridColumn = 1,
					GridPositionY = 8,
					SelectedIndex = 1
				};
				scriptCB.Items.Clear();
				foreach (var script in Enum.GetNames(typeof(ScriptDescription)))
				{
					scriptCB.Items.Add(new ListItem(script.ToString()));
				}
				parameterTF = new TextField
				{
					GridColumn = 1,
					GridPositionY = 9
				};
				var paramTxt = new TextField
				{
					GridColumn = 0,
					Text = "Parameter",
					GridPositionY = 9
				};
				newTileGrid.Widgets.Add(paramTxt);
				saveBtn = new Button
				{
					GridColumn = 1,
					Text = "Save Template",
					GridPositionY = 10,
				};

				placeModeRadio = new RadioButton
				{
					Text = "Place Objects",
					GridRow = 11,
					GridColumn = 0
				};

				editModeRadio = new RadioButton
				{
					Text = "Edit Objects",
					GridRow = 11,
					HorizontalAlignment = HorizontalAlignment.Right,
					GridColumn = 1
				};

				newTileGrid.Widgets.Add(saveBtn);
				newTileGrid.Widgets.Add(placeModeRadio);
				newTileGrid.Widgets.Add(editModeRadio);
				newTileGrid.Widgets.Add(layerDepthTF);
				newTileGrid.Widgets.Add(walkableCB);
				newTileGrid.Widgets.Add(destructsCB);
				newTileGrid.Widgets.Add(isDoorCB);
				newTileGrid.Widgets.Add(isItemCB);
				newTileGrid.Widgets.Add(isObjCB);
				newTileGrid.Widgets.Add(nameTF);
				newTileGrid.Widgets.Add(descriptionTF);
				newTileGrid.Widgets.Add(scriptCB);
				newTileGrid.Widgets.Add(parameterTF);

				saveBtn.Click += (s, a) =>
				{
					TileHelper.EditTile(UIState._selected._tileType, UIState._selectedTexture, LayerDepth.Cells,
						walkableCB.IsPressed, destructsCB.IsPressed, isObjCB.IsPressed,
						isDoorCB.IsPressed, isItemCB.IsPressed, nameTF.Text,
						(ScriptDescription)scriptCB.SelectedIndex, parameterTF.Text);

					TileHelper.Save();
				};
				scrollPane.Content = newTileGrid;
				parent.Widgets.Add(scrollPane);
			}

			public void UpdateState()
			{
				layerDepthTF.Text = UIState._selected._layerDepth.ToString();
				walkableCB.IsPressed = UIState._selected._walkable;
				destructsCB.IsPressed = UIState._selected._useOnInteract;
				isDoorCB.IsPressed = UIState._selected._isDoor;
				isItemCB.IsPressed = UIState._selected._isPickupItem;
				isObjCB.IsPressed = UIState._selected._isObject;
				nameTF.Text = UIState._selected._name;
				descriptionTF.Text = UIState._selected._description;
				scriptCB.SelectedIndex = (int)UIState._selected._script;
				parameterTF.Text = UIState._selected._parameter.ToString();
			}
		}

		public class TileWindow
		{
			private int INDEX_INC = 50;
			private ScrollPane tilesPane;
			private Panel tilesPanePanel;

			public void Show(bool visible)
			{
				tilesPane.Visible = visible;
				tilesPanePanel.Visible = visible;
			}

			public TileWindow(Grid parent, TilePropertiesWindow tilePropertiesWindow, List<Texture2D> textures)
			{
				tilesPane = new ScrollPane
				{
					GridPositionX = 1,
					GridPositionY = 2
				};
				var nextBtn = new Button
				{
					Text = "Next",
					VerticalAlignment = VerticalAlignment.Bottom,
					HorizontalAlignment = HorizontalAlignment.Right
				};
				var prevBtn = new Button
				{
					Text = "Prev",
					VerticalAlignment = VerticalAlignment.Bottom,
					HorizontalAlignment = HorizontalAlignment.Left
				};
				var indexText = new TextBlock
				{
					Text = "0",
					VerticalAlignment = VerticalAlignment.Bottom,
					HorizontalAlignment = HorizontalAlignment.Center
				};
				nextBtn.Click += (s, a) =>
				{
					UIState._beginIndex += INDEX_INC;
				};
				prevBtn.Click += (s, a) =>
				{
					UIState._beginIndex -= INDEX_INC;
				};
				tilesPanePanel = new Panel()
				{
					GridColumn = 1,
					GridRow = 2
				};
				tilesPane.MouseMoved += (s, a) =>
				{
					var xi = tilesPane.MousePosition.X / 32;
					var yi = tilesPane.MousePosition.Y / 32;
					UIState._mouseX = 32 * (xi);
					UIState._mouseY = 32 * (yi) - 16;
				};
				tilesPane.MouseDown += (s, a) =>
				{
					var xi = (tilesPane.MousePosition.X - 160) / 32;
					var yi = (tilesPane.MousePosition.Y - 280) / 32;
					int sizeX = 10;
					int index = (sizeX * yi + xi) + UIState._beginIndex;
					if (index < textures.Count && index >= 0)
					{
						TileHelper.GetTile(index, ref UIState._selected);
						TileHelper.GetTileTexture(index, ref UIState._selectedTexture);
						tilePropertiesWindow.UpdateState();
						indexText.Text = "Sprite Index: " + index.ToString() + " (" + xi.ToString() + "," + yi.ToString() + ")";
					}
				};
				tilesPanePanel.Widgets.Add(indexText);
				tilesPanePanel.Widgets.Add(nextBtn);
				tilesPanePanel.Widgets.Add(prevBtn);
				parent.Widgets.Add(tilesPanePanel);
				parent.Widgets.Add(tilesPane);
			}
		}

		public class CharacterWindow
		{
			Grid container;
			private List<TextField> fields;

			public void Show(bool visible)
			{
				container.Visible = visible;
			}

			private void AddRow(Grid grid, string label, string value, int index)
			{
				var txtBlock = new TextBlock
				{
					Text = label,
					GridColumn = 0,
					GridRow = index

				};
				var txtBox = new TextField
				{
					Text = value,
					GridColumn = 1,
					GridRow = index
				};
				fields.Add(txtBox);
				grid.Widgets.Add(txtBlock);
				grid.Widgets.Add(txtBox);
			}

			public CharacterWindow(Grid parent, Level level)
			{
				fields = new List<TextField>();
				container = new Grid
				{
					RowSpacing = 5,
					ColumnSpacing = 5,
					GridColumn = 3,
					GridRow = 2
				};
				for (int i = 0; i < 8; i++)
				{
					container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				AddRow(container, "Name", "Spearman", 0);
				AddRow(container, "Attack Bonus", "4", 1);
				AddRow(container, "Melee Attack", "15", 2);
				AddRow(container, "Ranged Attack", "1", 3);
				AddRow(container, "Health", "70", 4);
				AddRow(container, "Gold", "5", 5);

				var isMerchant = new CheckBox()
				{
					Text = "Is Merchant",
					GridColumn = 0,
					GridRow = 6
				};

				var merchantTypeCB = new ComboBox()
				{
					GridColumn = 1,
					GridRow = 6
				};
				merchantTypeCB.Items.Add(new ListItem("Weapons"));
				merchantTypeCB.Items.Add(new ListItem("Armour"));
				merchantTypeCB.Items.Add(new ListItem("Magic"));
				merchantTypeCB.Items.Add(new ListItem("Books"));
				merchantTypeCB.Items.Add(new ListItem("Junk"));
				merchantTypeCB.Visible = false;

				isMerchant.PressedChanged += (a, b) =>
				{
					merchantTypeCB.Visible = !merchantTypeCB.Visible;
				};

				var isEnemy = new CheckBox()
				{
					Text = "Is Hostile",
					GridColumn = 0,
					GridRow = 7
				};
				container.Widgets.Add(isEnemy);
				container.Widgets.Add(merchantTypeCB);
				container.Widgets.Add(isMerchant);

				var nextBtn = new Button()
				{
					Text = "Next",
					GridColumn = 0,
					GridRow = 8
				};
				var prevBtn = new Button()
				{
					Text = "Prev",
					GridColumn = 1,
					GridRow = 8
				};
				var saveBtn = new Button()
				{
					Text = "Save Type",
					GridColumn = 1,
					GridRow = 7
				};
				container.Widgets.Add(nextBtn);
				container.Widgets.Add(prevBtn);
				container.Widgets.Add(saveBtn);

				saveBtn.Click += (a, e) =>
				{
					FigureInfo info = new FigureInfo();
					info.IsMerchant = isMerchant.IsPressed;
					info.IsEnemy = isEnemy.IsPressed;
					info.Name = fields[0].Text;
					info.AttackBonus = int.Parse(fields[1].Text);
					info.MeleeAttack = int.Parse(fields[2].Text);
					info.RangedAttack = int.Parse(fields[3].Text);
					info.Health = int.Parse(fields[4].Text);
					info.Gold = int.Parse(fields[5].Text);
					info.TextureID = UIState._selected._tileType;
					NpcCreator.AddNpcType(info);
					UIState._npcTemplate = info;
				};

				parent.Widgets.Add(container);
			}
		}

		public class QuestsEditorWindow
		{
			Grid container;
			private List<TextField> fields;

			public bool IsVisible()
			{
				return container.Visible;
			}

			public void Show(bool visible)
			{
				container.Visible = visible;
			}

			private void DelTask(Grid grid, int index)
			{
				//grid.Widgets
			}

			private void AddTask(Grid grid, int index)
			{
				var scriptCB = new ComboBox
				{
					GridColumn = 1,
					GridRow = index
				};
				scriptCB.Items.Clear();
				foreach (var script in Enum.GetNames(typeof(ScriptEvaluation)))
				{
					scriptCB.Items.Add(new ListItem(script.ToString()));
				}

				var scriptTxt = new TextField
				{
					Text = "0",
					GridColumn = 2,
					GridRow = index
				};
				grid.Widgets.Add(scriptCB);
				grid.Widgets.Add(scriptTxt);
			}

			private void AddRow(Grid grid, string label, string value, int index)
			{
				var txtBlock = new TextBlock
				{
					Text = label,
					GridColumn = 0,
					GridRow = index

				};
				var txtBox = new TextField
				{
					Text = value,
					GridColumn = 1,
					GridRow = index
				};
				fields.Add(txtBox);
				grid.Widgets.Add(txtBlock);
				grid.Widgets.Add(txtBox);
			}

			public QuestsEditorWindow(Grid parent, Level level)
			{
				fields = new List<TextField>();
				container = new Grid
				{
					RowSpacing = 5,
					ColumnSpacing = 5,
					GridColumn = 2,
					GridRow = 2
				};
				for (int i = 0; i < 8; i++)
				{
					container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				var namesCB = new ComboBox
				{
					GridColumn = 0,
					GridRow = 0
				};
				namesCB.Items.Add(new ListItem("Main Quest"));
				namesCB.Items.Add(new ListItem("Kill 10 rats for Oggo"));
				container.Widgets.Add(namesCB);

				AddRow(container, "Quest Name", "", 1);

				var addBtn = new Button()
				{
					Text = "Add Task",
					GridColumn = 0,
					GridRow = 2
				};
				container.Widgets.Add(addBtn);

				int index = 2;
				addBtn.Click += (a, b) =>
				{
					AddTask(container, index);
					index++;
				};

				var delBtn = new Button
				{
					Text = "Delete Task",
					GridColumn = 0,
					GridRow = 3
				};
				container.Widgets.Add(delBtn);
				delBtn.Click += (a, b) =>
				{
					DelTask(container, index);
				};

				var nextBtn = new Button()
				{
					Text = "Next",
					GridColumn = 0,
					GridRow = 8
				};
				var prevBtn = new Button()
				{
					Text = "Prev",
					GridColumn = 1,
					GridRow = 8
				};
				var saveBtn = new Button()
				{
					Text = "Save Quest",
					GridColumn = 1,
					GridRow = 7
				};
				container.Widgets.Add(nextBtn);
				container.Widgets.Add(prevBtn);
				container.Widgets.Add(saveBtn);

				saveBtn.Click += (a, e) =>
				{
					Quest quest = new Quest("");
					QuestHelper.AddQuest(quest);
				};

				parent.Widgets.Add(container);
			}
		}
		QuestsEditorWindow questEditorWindow;

		public class DialogueEditorWindow
		{
			Grid container;
			private List<TextField> fields;

			public bool IsVisible()
			{
				return container.Visible;
			}

			public void Show(bool visible)
			{
				container.Visible = visible;
			}

			public DialogueEditorWindow(Grid parent, Level level)
			{
				container = new Grid
				{
					RowSpacing = 5,
					ColumnSpacing = 5,
					GridColumn = 2,
					GridRow = 2
				};
				for (int i = 0; i < 12; i++)
				{
					container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				var txt = new TextBlock
				{
					Text = "Dialogue Editor",
					GridRow = 0
				};

				//var scrollPane = new ScrollPane
				//{
				//	GridRow = 1
				//};

				var listBox = new ComboBox
				{
					GridRow = 1
				};

				listBox.Items.Add(new ListItem("Shopkeeper"));
				listBox.Items.Add(new ListItem("Villager1"));
				listBox.Items.Add(new ListItem("Villager2"));
				listBox.Items.Add(new ListItem("Villager3"));

				var nameTxt = new TextField
				{
					GridRow = 1,
					GridColumn = 1,
					Text = "<Name>"
				};

				var addBtn = new Button
				{
					GridRow = 2,
					GridColumn = 1,
					Text = "New Dialogue"
				};

				var delBtn = new Button
				{
					GridRow = 2,
					GridColumn = 2,
					Text = "Delete Dialogue"
				};
				container.Widgets.Add(delBtn);

				var saveBtn = new Button
				{
					GridRow = 2,
					GridColumn = 3,
					Text = "Save Dialogue"
				};
				container.Widgets.Add(saveBtn);

				var rootBtn = new Button
				{
					GridRow = 3,
					GridColumn = 1,
					Text = "Goto Root"
				};

				addBtn.Click += (a, b) =>
				{
					listBox.Items.Add(new ListItem(nameTxt.Text));
				};

				var txtField = new TextField
				{
					GridRow = 4,
					GridColumn = 1,
					Text = "<Start Conversation>"
				};
				var scriptCB = new ComboBox
				{
					GridRow = 5,
					GridColumn = 1
				};
				scriptCB.Items.Clear();
				foreach (var script in Enum.GetNames(typeof(ScriptDescription)))
				{
					scriptCB.Items.Add(new ListItem(script.ToString()));
				}
				var scriptTxt = new TextField
				{
					GridRow = 5,
					GridColumn = 2,
					Text = "0",
					Width = 200
				};
				var respBtn = new Button
				{
					GridRow = 6,
					GridColumn = 1,
					Text = "Add Speech"
				};
				var delSpeechBtn = new Button
				{
					GridRow = 6,
					GridColumn = 2,
					Text = "Delete Speech"
				};
				container.Widgets.Add(delSpeechBtn);

				var speechTxt = new TextField
				{
					GridRow = 7,
					GridColumn = 1,
					Text = "<Speech>",
					Width = 500,
					Height = 110
				};
				speechTxt.ClipToBounds = true;
				speechTxt.Multiline = true;
				var sp = new ScrollPane
				{
					GridRow = 7,
					GridColumn = 1
				};
				sp.Content = speechTxt;
				container.Widgets.Add(sp);

				int row = 8;
				respBtn.Click += (a, b) =>
				{
					var radioBtn = new RadioButton
					{
						Text = speechTxt.Text,
						GridRow = row,
						GridColumn = 1
					};
					row++;
					container.Widgets.Add(radioBtn);
				};

				container.Widgets.Add(rootBtn);
				container.Widgets.Add(respBtn);

				container.Widgets.Add(scriptTxt);
				container.Widgets.Add(scriptCB);

				container.Widgets.Add(txtField);

				container.Widgets.Add(addBtn);
				container.Widgets.Add(nameTxt);

				container.Widgets.Add(listBox);
				container.Widgets.Add(txt);

				parent.Widgets.Add(container);
			}
		}
		DialogueEditorWindow dialogueEditorWindow;
		//

		// GAME windows
		public class JournalWindow
		{
			Grid grid;

			public bool IsVisible()
			{
				return grid.Visible;
			}

			public void Show(bool visible)
			{
				if (grid.Visible == false && visible == true)
				{
					//Refresh(UIState._activeNpc);
				}
				grid.Visible = visible;
			}

			public JournalWindow(Grid parent)
			{
				grid = new Grid()
				{
					GridPositionX = 2,
					GridPositionY = 2
				};
				parent.Widgets.Add(grid);
			}
		}
		JournalWindow journalWindow;

		public class WorldMapWindow
		{
			Grid grid;
			public bool IsVisible()
			{
				return grid.Visible;
			}

			public void Show(bool visible)
			{
				if (grid.Visible == false && visible == true)
				{
				}
				grid.Visible = visible;
			}

			public WorldMapWindow(Grid parent)
			{
				grid = new Grid()
				{
					GridPositionX = 2,
					GridPositionY = 2
				};
				parent.Widgets.Add(grid);
			}
		}
		WorldMapWindow worldMapWindow;

		public class MainWindow
		{
			Grid container;

			public bool IsVisible()
			{
				return container.Visible;
			}

			public void Show(bool visible)
			{
				if (container.Visible == false && visible == true)
				{
					//Refresh(UIState._activeNpc);
				}
				container.Visible = visible;
			}

			public MainWindow(Grid parent)
			{
				container = new Grid()
				{
					GridColumn = 2,
					GridRow = 2
				};

				var newBtn = new TextButton
				{
					Text = "New",
					GridColumn = 1,
					GridRow = 0,
					HorizontalAlignment = HorizontalAlignment.Center
				};

				var loadBtn = new TextButton
				{
					Text = "Load",
					GridColumn = 1,
					GridRow = 1,
					HorizontalAlignment = HorizontalAlignment.Center
				};

				var saveBtn = new TextButton
				{
					Text = "Save",
					GridColumn = 1,
					GridRow = 2,
					HorizontalAlignment = HorizontalAlignment.Center
				};

				var quitBtn = new TextButton
				{
					Text = "Quit",
					GridColumn = 1,
					GridRow = 3,
					HorizontalAlignment = HorizontalAlignment.Center
				};

				container.ShowGridLines = true;
				container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.HorizontalAlignment = HorizontalAlignment.Center;
				container.VerticalAlignment = VerticalAlignment.Center;

				container.Widgets.Add(newBtn);
				container.Widgets.Add(loadBtn);
				container.Widgets.Add(saveBtn);
				container.Widgets.Add(quitBtn);

				parent.Widgets.Add(container);
			}
		}
		MainWindow mainWindow;

		public class AbilitiesWindow
		{
			Grid grid;

			public bool IsVisible()
			{
				return grid.Visible;
			}

			public void Show(bool visible)
			{
				if (grid.Visible == false && visible == true)
				{
					//Refresh(UIState._activeNpc);
				}
				grid.Visible = visible;
			}

			public AbilitiesWindow(Grid parent)
			{
				grid = new Grid()
				{
					GridPositionX = 2,
					GridPositionY = 2
				};
				parent.Widgets.Add(grid);
			}
		}
		AbilitiesWindow abilitiesWindow;

		public class ShopWindow
		{
			Grid grid;
			Grid shopInv;
			Grid playerInv;

			public bool IsVisible()
			{
				return grid.Visible;
			}

			public void Show(bool visible)
			{
				if (grid.Visible == false && visible == true)
				{
					Refresh(UIState._activeNpc);
				}
				grid.Visible = visible;
			}

			public void AddPlayerItem(string name, int index)
			{
				TextBlock tb1 = new TextBlock()
				{
					Text = name,
					GridColumn = 0,
					GridRow = index
				};
				playerInv.Widgets.Add(tb1);
				Button btn1 = new Button()
				{
					Text = "Sell",
					GridColumn = 1,
					GridRow = index
				};
				btn1.Click += (a, b) =>
				{
				};
				playerInv.Widgets.Add(btn1);
			}

			public void AddNPCItem(string name, int index)
			{
				TextBlock tb1 = new TextBlock()
				{
					Text = name,
					GridColumn = 0,
					GridRow = index
				};
				shopInv.Widgets.Add(tb1);
				Button btn1 = new Button()
				{
					Text = "Buy",
					GridColumn = 1,
					GridRow = index
				};
				btn1.Click += (a, b) =>
				{
				};
				shopInv.Widgets.Add(btn1);
			}

			public void Refresh(Npc shopKeeper)
			{
				shopInv.Widgets.Clear();
				playerInv.Widgets.Clear();

				int index = 0;
				foreach (var item in shopKeeper.Info.Inventory)
				{
					TileInfo tile = null;
					TileHelper.GetTile(item, ref tile);
					AddNPCItem(tile._name, ++index);
				}

				index = 0;
				foreach (var item in Global.Player.Inventory)
				{
					AddPlayerItem(item.Name, ++index);
				}
			}

			public ShopWindow(Grid parent)
			{
				grid = new Grid()
				{
					GridPositionX = 2,
					GridPositionY = 2
				};
				grid.ShowGridLines = true;
				grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				shopInv = new Grid()
				{
					GridColumn = 0,
					Width = 250
				};
				shopInv.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				shopInv.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				for (int i = 0; i < 100; i++)
				{
					shopInv.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}

				playerInv = new Grid()
				{
					GridColumn = 2,
					Width = 250
				};
				Panel middlePanel = new Panel()
				{
					GridColumn = 1,
					Width = 400,
					Height = 540
				};
				var leaveBtn = new Button()
				{
					Text = "Leave",
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Bottom
				};
				middlePanel.Widgets.Add(leaveBtn);
				leaveBtn.Click += (a, b) =>
				{
					Show(false);
					//UIState.ShowShopWindow = false;
				};

				playerInv.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				for (int i = 0; i < 100; i++)
				{
					playerInv.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				grid.Widgets.Add(shopInv);
				grid.Widgets.Add(middlePanel);
				grid.Widgets.Add(playerInv);


				parent.Widgets.Add(grid);
			}
		}
		ShopWindow shopWindow;

		public class DialogueWindow
		{
			Grid container;
			Level _level;
			TextBlock userText;
			TextBlock option1;
			TextBlock option2;
			TextBlock option3;
			public int YSelect = 0;
			int YMax = 2;
			DialogueNode startNode;
			int rootId = 0;

			public bool IsVisible()
			{
				return container.Visible;
			}

			public void HandleInput(InputState inputState)
			{
				if (inputState.IsDown(PlayerIndex.One))
				{
					YSelect++;
				}
				if (inputState.IsUp(PlayerIndex.One))
				{
					YSelect--;
				}
				if (YSelect < 0) YSelect = 0;
				if (YSelect > YMax) YSelect = YMax;

				PlayerIndex index;
				if (inputState.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Enter, PlayerIndex.One, out index))
				{
					if (startNode.Children[YSelect].Children.Count > 0)
					{
						UIState._activeNpc.DialogueRoot = startNode.Children[YSelect].Children[0].Id;
						Refresh(UIState._activeNpc, UIState._activeNpc.DialogueRoot);
					}
					else
					{
						UIState._activeNpc.DialogueRoot = rootId;
						Show(false);
					}
				}
			}

			public void Show(bool visible)
			{
				if (container.Visible == false && visible == true)
				{
					rootId = UIState._activeNpc.DialogueRoot;
					Refresh(UIState._activeNpc, UIState._activeNpc.DialogueRoot);
				}
				container.Visible = visible;

				if (visible == true)
				{
					Global.GameState = GameStates.Dialogue;
					//Global.Camera.AdjustZoom(1f);
					Global.Camera.CenterOn((Cell)_level.Map.GetCell(Global.Player.X, Global.Player.Y));
					//Global.Camera.CenterOn(new Vector2(Global.Player.X*32, Global.Player.Y*32));
				}
				else
				{
					//Global.Camera.AdjustZoom(0f);
					Global.GameState = GameStates.PlayerTurn;
				}
			}

			public void Refresh(Npc npc, int root)
			{
				var rootNode = DialogueHelper.GetDialogue(root);
				startNode = rootNode;
				userText.Text = rootNode.Text;

				option1.Text = "";
				option2.Text = "";
				option3.Text = "";

				if (rootNode.Children.Count > 0)
				{
					option1.Text = rootNode.Children[0].Text;
					YMax = 0;
				}
				if (rootNode.Children.Count > 1)
				{
					option2.Text = rootNode.Children[1].Text;
					YMax = 1;
				}
				if (rootNode.Children.Count > 2)
				{
					option3.Text = rootNode.Children[2].Text;
					YMax = 2;
				}
			}

			public DialogueWindow(Grid parent, Level level)
			{
				_level = level;
				container = new Grid()
				{
					GridColumn = 2,
					GridRow = 2,
					Left = 110,
					Top = 310,
				};
				container.ClipToBounds = true;
				userText = new TextBlock()
				{
					Text = "...?\n\n\n",
					GridRow = 0
				};

				option1 = new TextBlock()
				{
					Text = "",
					GridRow = 1
				};
				option2 = new TextBlock()
				{
					Text = "",
					GridRow = 2
				};
				option3 = new TextBlock()
				{
					Text = "",
					GridRow = 3
				};

				container.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 120));
				for (int i = 0; i < 10; i++)
				{
					container.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				container.Widgets.Add(userText);
				container.Widgets.Add(option1);
				container.Widgets.Add(option2);
				container.Widgets.Add(option3);
				parent.Widgets.Add(container);

			}
		}
		DialogueWindow dialogueWindow;

		public class InventoryWindow
		{
			Grid grid;

			public bool IsVisible()
			{
				return grid.Visible;
			}

			private void AddRow(Grid grid, string label, int index)
			{
				Grid row = new Grid()
				{
					GridRow = index
				};
				row.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				var txtBlock = new TextBlock
				{
					Text = label,
					GridColumn = 0,

				};
				var btn = new Button
				{
					Text = "Use",
					GridColumn = 1,
				};

				row.Widgets.Add(txtBlock);
				row.Widgets.Add(btn);
				grid.Widgets.Add(row);

				btn.Click += (a, s) =>
				{
					if (Global.Player.Inventory.Count >= 0)
					{
						//var item = Global.Player.Inventory[index];
						Global.Player.Inventory.RemoveAt(index);
						//Global.Player.Inventory[index] = null;
						grid.Widgets.RemoveAt(row.GridRow);
					}
				};
			}

			public void Show()
			{
				if (grid.Visible == false)
				{
					RefreshInventory();
				}
				grid.Visible = true;
			}

			public void Hide()
			{
				grid.Visible = false;
			}

			public void RefreshInventory()
			{
				int i = 0;

				//grid.Widgets.Clear();

				foreach (var item in Global.Player.Inventory)
				{
					AddRow(grid, item.Name + i.ToString(), i);
					i++;
				}
			}

			public InventoryWindow(Grid parent)
			{
				grid = new Grid
				{
					RowSpacing = 5,
					ColumnSpacing = 5,
					GridColumn = 2,
					GridRow = 2
				};
				for (int i = 0; i < 8; i++)
				{
					grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
				}
				grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

				parent.Widgets.Add(grid);
			}
		}
		InventoryWindow inventoryWindow;

		public class ConsoleWindow
		{
			Grid grid;
			public void Show(bool visible)
			{
				grid.Visible = visible;
			}

			public ConsoleWindow(Grid parent)
			{
				grid = new Grid()
				{
					GridColumn = 2,
					GridRow = 3
				};

				var consoleText = new TextBlock
				{
					Text = "* * * Welcome to Second Dungeon * * *",

				};
				var consolePane = new ScrollPane
				{
					GridPositionX = 0,
					GridPositionY = 0,
				};
				consolePane.Content = consoleText;
				consoleText.ClipToBounds = true;
				parent.Widgets.Add(grid);
				grid.Widgets.Add(consolePane);
			}
		}
		ConsoleWindow consoleWindow;

		public class StatsWindow
		{
			Grid grid;
			TextBlock statsText;
			public void Show(bool visible)
			{
				grid.Visible = visible;
			}

			public void Update()
			{
				string str =
							Global.Player.Info.Name + "\n" +
							"HP: " + Global.Player.Info.Health + "\n" +
							"XP: " + Global.Player.Info.XP + "\n" +
							"GOLD: " + Global.Player.Info.Gold;
				statsText.Text = str;
			}

			public StatsWindow(Grid parent)
			{
				grid = new Grid
				{
					GridColumn = 3,
					GridRow = 2
				};

				statsText = new TextBlock
				{
					Text = "HP:0\nXP:0\nGOLD",
					GridPositionX = 0,
					GridPositionY = 0,

				};
				statsText.ClipToBounds = true;
				grid.Widgets.Add(statsText);
				parent.Widgets.Add(grid);
			}


		}
		StatsWindow statsWindow;
		//

		void Paint()
		{
			Vector2 world = Global.Camera.ScreenToWorld(new Vector2(UIState._mouseX, UIState._mouseY));
			var wx = (int)(world.X / 32f);
			var wy = (int)(world.Y / 32f);

			if (UIState._paintMode == PaintMode.Tiles)
			{
				if (UIState._selected._isDoor)
				{
					_level.AddDoorAt(wx, wy, UIState._selected._tileType - 1, UIState._selected._tileType);
				}
				else if (UIState._selected._isPickupItem)
				{
					_level.AddItemAt(wx, wy, UIState._selected._tileType);
				}
				else if (UIState._selected._isObject)
				{
					_level.AddObjectAt(wx, wy, UIState._selected._tileType);
				}
				else if (UIState._selected._walkable)
				{
					_level.AddFloorAt(wx, wy, UIState._selected._tileType);
				}
				else if (!UIState._selected._walkable)
				{
					_level.AddWallAt(wx, wy, UIState._selected._tileType);
				}
			}
			else if (UIState._paintMode == PaintMode.NPCs)
			{
				var level = LevelManager.GetCurrentLevel();
				var npc = NpcCreator.CreateNpc(UIState._npcTemplate, level, wx, wy);
				level.AddNpc(npc);
				_painting = false;
			}
		}

		void SetupUI()
		{
			Myra.MyraEnvironment.Game = _game;

			_host = new Desktop();
			_host.Bounds = new Microsoft.Xna.Framework.Rectangle(0, 0, _game.GraphicsDevice.PresentationParameters.BackBufferWidth, _game.GraphicsDevice.PresentationParameters.BackBufferHeight);

			_whiteTex = new Texture2D(_graphics.GraphicsDevice, 1, 1);
			_whiteTex.SetData(new[] { Color.White });

			var grid = new Grid
			{
				RowSpacing = 5,
				ColumnSpacing = 5,
			};

			//grid.Background.Size = new Microsoft.Xna.Framework.Point(400, 400);
			grid.ShowGridLines = true;
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 160));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 310));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 960));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 310));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 160));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 100));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 160));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 540));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 160));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 100));

			statsWindow = new StatsWindow(grid);

			TilePropertiesWindow tilePropertiesWindow = new TilePropertiesWindow(grid);
			TileWindow tileWindow = new TileWindow(grid, tilePropertiesWindow, _textures);
			CharacterWindow characterWindow = new CharacterWindow(grid, _level);
			questEditorWindow = new QuestsEditorWindow(grid, _level);
			dialogueEditorWindow = new DialogueEditorWindow(grid, _level);
			inventoryWindow = new InventoryWindow(grid);
			consoleWindow = new ConsoleWindow(grid);
			shopWindow = new ShopWindow(grid);
			dialogueWindow = new DialogueWindow(grid, _level);
			journalWindow = new JournalWindow(grid);
			abilitiesWindow = new AbilitiesWindow(grid);
			worldMapWindow = new WorldMapWindow(grid);
			mainWindow = new MainWindow(grid);

			tilePropertiesWindow.Show(false);
			tileWindow.Show(false);
			characterWindow.Show(false);
			questEditorWindow.Show(false);
			//inventoryWindow.Show(false);
			inventoryWindow.Hide();
			statsWindow.Show(false);
			consoleWindow.Show(false);
			shopWindow.Show(false);
			dialogueWindow.Show(false);
			dialogueEditorWindow.Show(false);
			abilitiesWindow.Show(false);
			worldMapWindow.Show(false);
			journalWindow.Show(false);
			mainWindow.Show(false);

			var centrePanel = new Panel
			{
				GridPositionX = 2,
				GridPositionY = 2
			};
			centrePanel.MouseMoved += (s, a) =>
			{
				if (Global.GameState == GameStates.Editor)
				{
					UIState._mouseX = centrePanel.MousePosition.X;
					UIState._mouseY = centrePanel.MousePosition.Y;
					if (_painting == true)
					{
						Paint();
					}
				}
			};
			centrePanel.MouseDown += (s, a) =>
			{
				if (Global.GameState == GameStates.Editor)
				{
					Paint();
					_painting = !_painting;
				}
			};
			grid.Widgets.Add(centrePanel);

			var mainMenu = new HorizontalMenu()
			{
				GridRow = 0,
				GridColumn = 0,
				GridColumnSpan = 5
			};

			var fileMenu = new MenuItem("0", "File");
			var playMenu = new MenuItem("1", "Play");
			var runMenu = new MenuItem("2", "Run Game");
			var newMapMenu = new MenuItem("3", "New Map");
			var saveMapMenu = new MenuItem("4", "Save Map");
			var loadMapMenu = new MenuItem("5", "Load Map");
			var tileEditorMenu = new MenuItem("6", "Tile Editor");
			var charEditorMenu = new MenuItem("7", "Character Editor");
			var questEditorMenu = new MenuItem("8", "Quest Editor");
			var dialogueEditorMenu = new MenuItem("9", "Dialogue Editor");
			var quitMenu = new MenuItem("10", "Quit");

			playMenu.Selected += (s, a) =>
			{
				tilePropertiesWindow.Show(false);
				tileWindow.Show(false);
				characterWindow.Show(false);
				Global.GameState = GameStates.PlayerTurn;
			};

			runMenu.Selected += (s, a) =>
			{
				HideAllWindows();
				mainWindow.Show(true);
				Global.GameState = GameStates.MainMenu;
			};

			tileEditorMenu.Selected += (s, a) =>
			{
				tilePropertiesWindow.Show(true);
				tileWindow.Show(true);
				characterWindow.Show(false);
				Global.GameState = GameStates.Editor;
				UIState._beginIndex = 2569;
				UIState._paintMode = PaintMode.Tiles;
			};

			charEditorMenu.Selected += (s, a) =>
			{
				tilePropertiesWindow.Show(false);
				tileWindow.Show(true);
				characterWindow.Show(true);
				UIState._beginIndex = 3800;
				UIState._paintMode = PaintMode.NPCs;
				Global.GameState = GameStates.Editor;
			};

			questEditorMenu.Selected += (s, a) =>
			{
				HideAllWindows();
				questEditorWindow.Show(true);
				Global.GameState = GameStates.Editor;
			};

			dialogueEditorMenu.Selected += (s, a) =>
			{
				HideAllWindows();
				dialogueEditorWindow.Show(true);
				Global.GameState = GameStates.Editor;
			};

			newMapMenu.Selected += (s, a) =>
			{
				LevelManager.GetCurrentLevel().GenerateEmptyMap();
			};

			saveMapMenu.Selected += (s, a) =>
			{
				LevelManager.GetCurrentLevel().Save();
			};

			quitMenu.Selected += (s, a) =>
			{
				Global.GameState = GameStates.Quit;
			};

			fileMenu.Items.Add(playMenu);
			fileMenu.Items.Add(new MenuSeparator());
			fileMenu.Items.Add(runMenu);
			fileMenu.Items.Add(new MenuSeparator());
			fileMenu.Items.Add(newMapMenu);
			fileMenu.Items.Add(saveMapMenu);
			fileMenu.Items.Add(loadMapMenu);
			// automatic load
			fileMenu.Items.Add(new MenuSeparator());
			fileMenu.Items.Add(tileEditorMenu);
			fileMenu.Items.Add(charEditorMenu);
			fileMenu.Items.Add(questEditorMenu);
			fileMenu.Items.Add(dialogueEditorMenu);
			fileMenu.Items.Add(new MenuSeparator());
			fileMenu.Items.Add(quitMenu);

			mainMenu.Items.Add(fileMenu);
			grid.Widgets.Add(mainMenu);

			_host.Widgets.Add(grid);

		}

		public void ToggleInventoryWindow()
		{
			if (inventoryWindow.IsVisible())
			{
				inventoryWindow.Hide();
			}
			else
			{
				inventoryWindow.Show();
			}
		}

		public void HideAllWindows()
		{
			//tilePropertiesWindow.Show(false);
			//tileWindow.Show(false);
			//characterWindow.Show(false);
			questEditorWindow.Show(false);
			inventoryWindow.Hide();
			statsWindow.Show(false);
			consoleWindow.Show(false);
			shopWindow.Show(false);
			dialogueWindow.Show(false);
			dialogueEditorWindow.Show(false);
			abilitiesWindow.Show(false);
			worldMapWindow.Show(false);
			journalWindow.Show(false);
			mainWindow.Show(false);
		}

		public void DrawStatsWindow(bool draw)
		{
			statsWindow.Show(draw);
		}

		public void DrawShopWindow(bool draw)
		{
			shopWindow.Show(draw);
		}

		public void DrawDialogueWindow(bool draw)
		{
			dialogueWindow.Show(draw);
		}

		public void DrawMainWindow(bool draw)
		{
			mainWindow.Show(draw);
		}

		public void DrawJournalWindow(bool draw)
		{
			journalWindow.Show(draw);
		}

		public void DrawAbilitiesWindow(bool draw)
		{
			abilitiesWindow.Show(draw);
		}

		public void DrawMapWindow(bool draw)
		{
			worldMapWindow.Show(draw);
		}

		public bool IsInventoryWindowVisible()
		{
			return inventoryWindow.IsVisible();
		}

		public bool IsDialogueWindowVisible()
		{
			return dialogueWindow.IsVisible();
		}

		public bool IsShopWindowVisible()
		{
			return shopWindow.IsVisible();
		}

		public bool IsMainWindowVisible()
		{
			return mainWindow.IsVisible();
		}

		public bool IsJournalWindowVisible()
		{
			return journalWindow.IsVisible();
		}

		public bool IsAbilitiesWindowVisible()
		{
			return abilitiesWindow.IsVisible();
		}

		public void DrawConsoleWindow(bool draw)
		{
			consoleWindow.Show(draw);
		}

		void DrawUI(SpriteBatch spriteBatch)
		{
			if (_graphics.PreferredBackBufferWidth != _game.Window.ClientBounds.Width ||
			_graphics.PreferredBackBufferHeight != _game.Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = _game.Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = _game.Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

			int ySelect = 120 + (dialogueWindow.YSelect * 20);

			//_game.GraphicsDevice.Clear(new Color(20, 20, 20, 255));

			if (dialogueWindow.IsVisible())
			{
				spriteBatch.Begin();
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(480 + 100, 270 + 300, 800, 200), Color.Black);
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(585, 580 + ySelect, 790, 2), Color.Yellow);
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(585, 600 + ySelect, 790, 2), Color.Yellow);
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(585, 580 + ySelect, 2, 20), Color.Yellow);
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(1373, 580 + ySelect, 2, 20), Color.Yellow);
				spriteBatch.End();
			}
			else if (shopWindow.IsVisible() || inventoryWindow.IsVisible() || questEditorWindow.IsVisible() || dialogueEditorWindow.IsVisible())
			{
				spriteBatch.Begin();
				spriteBatch.Draw(_whiteTex, new Microsoft.Xna.Framework.Rectangle(480 - 1, 270 - 1, 960 + 1, 540 + 1), Color.Black);
				spriteBatch.End();
			}

			//_host.Bounds = new Microsoft.Xna.Framework.Rectangle(0, 0, _game.GraphicsDevice.PresentationParameters.BackBufferWidth, _game.GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}


		public void HandleInput(InputState inputState)
		{
			dialogueWindow.HandleInput(inputState);
		}

		public void DrawTilesEditor(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();
			int index = UIState._beginIndex;
			int xoffset = 160;
			int yoffset = 280;
			for (int y = 0; y < 15; y++)
			{
				for (int x = 0; x < 10; x++)
				{
					spriteBatch.Draw(_textures[index],
						new Microsoft.Xna.Framework.Rectangle(xoffset + (x * 32), yoffset + (y * 32), 32, 32), Color.White);
					index++;
				}
			}
			spriteBatch.Draw(UIState._selectedTexture, new Vector2(UIState._mouseX-16, UIState._mouseY-16), Color.White);
			spriteBatch.End();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			DrawUI(spriteBatch);
			if (Global.GameState == GameStates.Editor)
			{
				if (!questEditorWindow.IsVisible() && !dialogueEditorWindow.IsVisible())
				{
					DrawTilesEditor(spriteBatch);
				}
			}
		}
	}
}
