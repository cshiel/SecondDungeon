using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp.DiceNotation;

namespace SecondDungeon.Source
{
	[Serializable]
	public class CellInfo
	{
		int cellType;
		bool isExplored;
		bool isInFov;
		bool isTransparent;
		bool isWalkable;
		int x, y;

		public CellInfo(Cell cell)
		{
			cellType = cell.CellType;
			isExplored = cell.IsExplored;
			isInFov = cell.IsInFov;
			isTransparent = cell.IsTransparent;
			isWalkable = cell.IsWalkable;
			x = cell.X;
			y = cell.Y;
		}

		public Cell GetCell()
		{
			Cell cell = new Cell(x, y, isTransparent, isWalkable, isInFov, cellType);
			return cell;
		}
	}

	public class LevelManager
	{
		private static Level _currentLevel;

		public static void LoadLevel(Level level)
		{
			_currentLevel = level;
		}

		public static Level GetCurrentLevel()
		{
			return _currentLevel;
		}
	}

	public class Level
	{
		private Map _map;

		private Door[,] _doors;
		private PickupItem[,] _items;
		private LevelObject[,] _objects;
		private Npc[,] _npcs;

		//private List<Npc> _allNPCs = new List<Npc>();

		public Map Map
		{
			get { return _map; }
		}

		private void ResetData()
		{
			_doors = new Door[Global.MapWidth, Global.MapHeight];
			_items = new PickupItem[Global.MapWidth, Global.MapHeight];
			_objects = new LevelObject[Global.MapWidth, Global.MapHeight];
			_npcs = new Npc[Global.MapWidth, Global.MapHeight];
		}

		public Level()
		{
			ResetData();
			//Cell startingCell = GetRandomEmptyCell();

			Texture2D tex = null;
			TileHelper.GetTileTexture(3863, ref tex);

			Global.Player = new Player()
			{
				X = 10,
				Y = 10,
				Sprite =  tex,
				Damage = Dice.Parse("2d4"),
				
			};


			Global.Player.Info.ArmorClass = 15;
			Global.Player.Info.AttackBonus = 1;
			Global.Player.Info.Health = 50;
			Global.Player.Info.Name = "Player";

			//Global.Player = _player;
			//_level.Init(_player);
			//Global.CombatManager = new CombatManager(Global.Player, GetNpcs());
		}

		//public void Init(Player player)
		//{
		//	Global.CombatManager = new CombatManager(player, GetNpcs());
		//}

		public void Update()
		{
			foreach (var enemy in GetAllNpcs())
			{
				enemy.Update();
			}
		}

		public Door GetDoor(int x, int y)
		{
			return _doors[x, y];
		}

		public PickupItem GetItem(int x, int y)
		{
			return _items[x, y];
		}

		public void RemoveItem(int x, int y)
		{
			_items[x, y] = null;
		}

		public LevelObject GetObject(int x, int y)
		{
			return _objects[x, y];
		}

		public IEnumerable<Door> GetAllDoors()
		{
			for (int y = 0; y < Global.MapHeight; y++)
			{
				for (int x = 0; x < Global.MapWidth; x++)
				{
					var obj = GetDoor(x, y);
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
		}

		public IEnumerable<PickupItem> GetAllItems()
		{
			for (int y = 0; y < Global.MapHeight; y++)
			{
				for (int x = 0; x < Global.MapWidth; x++)
				{
					var obj = GetItem(x, y);
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
		}

		public IEnumerable<LevelObject> GetAllObjects()
		{
			for (int y = 0; y < Global.MapHeight; y++)
			{
				for (int x = 0; x < Global.MapWidth; x++)
				{
					var obj = GetObject(x, y);
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
		}

		public List<Npc> GetNpcs()
		{
			var npcs = GetAllNpcs();
			var npcList = new List<Npc>();
			foreach (var npc in npcs)
			{
				npcList.Add(npc);
			}
			return npcList;
		}

		public IEnumerable<Npc> GetAllNpcs()
		{
			for (int y = 0; y < Global.MapHeight; y++)
			{
				for (int x = 0; x < Global.MapWidth; x++)
				{
					var obj = NpcAt(x, y);
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
		}

		public Map GenerateEmptyMap()
		{
			//IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(Global.MapWidth, Global.MapHeight, 100, 7, 3);
			//_map = Map.Create(mapCreationStrategy);
			ResetData();

			_map = new Map(Global.MapWidth, Global.MapHeight);

			for (int y = 0; y < Global.MapHeight; y++)
			{
				for (int x = 0; x < Global.MapWidth; x++)
				{
					Map.SetCellProperties(x, y, true, true, 257);
				}
			}

			for (int i = 0; i < Global.MapWidth; i++)
			{
				AddWallAt(i, 0, 200);
				AddWallAt(i, Global.MapHeight - 1, 200);
			}
			for (int i = 0; i < Global.MapHeight; i++)
			{
				AddWallAt(0, i, 200);
				AddWallAt(Global.MapWidth - 1, i, 200);
			}

			//AddDoorAt(0, 0);
			//AddDoorAt(10, 10, 100, 101);

			return _map;
		}

		public void AddNpc(Npc npc)
		{
			_npcs[npc.X, npc.Y] = npc;
		}

		public Npc NpcAt(int x, int y)
		{
			return _npcs[x, y];
		}

		public void AddTile(Cell cell)
		{
			_map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored, cell.CellType);
		}

		public void AddFloorAt(int x, int y, int type)
		{
			_map.SetCellProperties(x, y, true, true, true, type);
		}

		public void AddObjectAt(int x, int y, int objType)
		{
			Cell cell = (Cell)_map.GetCell(x, y);
			_map.SetCellProperties(x, y, false, false, true, cell.CellType);
			TileInfo tileInfo = null;
			TileHelper.GetTile(objType, ref tileInfo);
			LevelObject obj = new LevelObject();
			obj.Parameter = tileInfo._parameter;
			obj.Script = tileInfo._script;
			obj.X = x;
			obj.Y = y;
			_objects[x, y] = obj;
			obj.SpriteType = objType;
		}

		public void AddWallAt(int x, int y, int type)
		{
			_map.SetCellProperties(x, y, false, false, true, type);
		}

		public void AddDoorAt(int x, int y, int openType, int closedType)
		{
			Cell cell = (Cell)_map.GetCell(x, y);
			_map.SetCellProperties(x, y, false, true, true, cell.CellType);
			Door door = new Door();
			door.X = x;
			door.Y = y;
			_doors[x, y] = door;
			door.OpenType = openType;
			door.ClosedType = closedType;
		}

		public void AddItemAt(int x, int y, int type)
		{
			Cell cell = (Cell)_map.GetCell(x, y);
			_map.SetCellProperties(x, y, true, true, true, cell.CellType);
			PickupItem item = new PickupItem();
			TileInfo tileInfo = null;
			TileHelper.GetTile(type, ref tileInfo);
			item.Name = tileInfo._name;
			item.Description = tileInfo._description;
			item.UseOnPickup = tileInfo._useOnInteract;
			item.Parameter = tileInfo._parameter;
			item.Script = tileInfo._script;
			item.X = x;
			item.Y = y;
			_items[x, y] = item;
			item.SpriteType = type;
		}

		public bool IsWalkable(int x, int y)
		{
			Cell cell = (Cell)_map.GetCell(x, y);
			if (cell.IsWalkable == false)
				return false;

			var door = GetDoor(x, y);
			if (door != null)
			{
				door.Open();
				return door.IsOpen;
			}

			var obj = GetObject(x, y);
			if (obj != null)
			{
				obj.Interact();
				return false;
			}

			var item = GetItem(x, y);
			if (item != null)
			{
				item.Pickup();
				RemoveItem(x, y);
				return true;
			}

			return true;
		}

		public void Save()
		{
			var cells = _map.GetAllCells();

			List<CellInfo> cellInfos = new List<CellInfo>();
			foreach (var cell in cells)
			{
				CellInfo cellInfo = new CellInfo((Cell)cell);
				cellInfos.Add(cellInfo);
			}

			Stream s = File.Open("level.dat", FileMode.OpenOrCreate);
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(s, cellInfos);
			b.Serialize(s, _doors);
			b.Serialize(s, _items);
			b.Serialize(s, _objects);
			var npcs = GetNpcs();
			List<FigureInfo> npcInfos = new List<FigureInfo>();
			foreach (var npc in npcs)
			{
				npc.Info.X = npc.X;
				npc.Info.Y = npc.Y;
				npcInfos.Add(npc.Info);
			}
			b.Serialize(s, npcInfos);
			s.Close();
		}

		public void Load()
		{
			Stream s = File.Open("level.dat", FileMode.Open);
			BinaryFormatter b = new BinaryFormatter();
			var cellInfos = (List<CellInfo>)b.Deserialize(s);
			_doors = (Door[,])b.Deserialize(s);
			_items = (PickupItem[,])b.Deserialize(s);
			_objects = (LevelObject[,])b.Deserialize(s);

			_map = new Map(Global.MapWidth, Global.MapHeight);

			foreach (var cellInfo in cellInfos)
			{
				Cell cell = cellInfo.GetCell();
				AddTile(cell);
			}

			var npcs = (List<FigureInfo>)b.Deserialize(s);
			foreach (var info in npcs)
			{
				var npc = NpcCreator.CreateNpc(info, this, info.X, info.Y);
				Texture2D tex = null;
				TileHelper.GetTileTexture(npc.Info.TextureID, ref tex);
				npc.Sprite = tex;
				AddNpc(npc);
			}

			s.Close();

			Global.CombatManager = new CombatManager(Global.Player, GetNpcs());
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var enemy in GetAllNpcs())
			{
				if (Global.GameState == GameStates.Debugging || Map.IsInFov(enemy.X, enemy.Y))
				{
					enemy.Draw(spriteBatch);
				}
			}
		}
	}
}
