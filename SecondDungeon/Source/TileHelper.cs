using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SecondDungeon.Source
{
	[Serializable]
	public class TileInfo
	{
		public int _tileType;
		public float _layerDepth;
		public float _scale = 1.0f;
		public bool _walkable;
		public bool _useOnInteract;
		public bool _isDoor;
		public bool _isPickupItem;
		public bool _isObject;
		public string _name;
		public string _description;
		public ScriptDescription _script;
		public object _parameter;
		public int _goldValue;

		//public bool _canWield;
		//public int _damage;
		//public int _protection;
	}

	public class TileHelper
	{
		public static Texture2D LoadTexture(GraphicsDevice graphics, Texture2D bigTexture, int x, int y)
		{
			int xoffset = x * Global.SpriteWidth;
			int yoffset = y * Global.SpriteHeight;
			Microsoft.Xna.Framework.Rectangle sourceRectangle = new Microsoft.Xna.Framework.Rectangle(xoffset, yoffset, Global.SpriteWidth, Global.SpriteHeight);
			var tex = new Texture2D(graphics, sourceRectangle.Width, sourceRectangle.Height);
			Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
			bigTexture.GetData(0, sourceRectangle, data, 0, data.Length);
			tex.SetData(data);
			return tex;
		}

		private static List<Texture2D> _textures = new List<Texture2D>();

		private static Dictionary<int, TileInfo> _tileInfo = new Dictionary<int, TileInfo>();

		public static void AddTile(int type, Texture2D texture, float layerDepth, bool walkable, bool destructs,
			bool isObject, bool isDoor, bool isItem, string name, ScriptDescription script, object parameter, int goldValue)
		{
			TileInfo tileInfo = new TileInfo();
			tileInfo._tileType = type;
			_textures.Add(texture);
			tileInfo._layerDepth = layerDepth;
			tileInfo._walkable = walkable;
			tileInfo._useOnInteract = destructs;
			tileInfo._isObject = isObject;
			tileInfo._isDoor = isDoor;
			tileInfo._isPickupItem = isItem;
			tileInfo._name = name;
			tileInfo._script = script;
			tileInfo._parameter = parameter;
			tileInfo._goldValue = goldValue;
			_tileInfo[type] = tileInfo;
		}

		public static void EditTile(int type, Texture2D texture, float scale, float layerDepth, bool walkable, bool destructs,
			bool isObject, bool isDoor, bool isItem, string name, string description, ScriptDescription script, object parameter, int goldValue)
		{
			if (_tileInfo.ContainsKey(type))
			{
				_tileInfo[type]._tileType = type;
				_textures[type] = texture;
				_tileInfo[type]._scale = scale;
				_tileInfo[type]._layerDepth = layerDepth;
				_tileInfo[type]._walkable = walkable;
				_tileInfo[type]._useOnInteract = destructs;
				_tileInfo[type]._isObject = isObject;
				_tileInfo[type]._isDoor = isDoor;
				_tileInfo[type]._isPickupItem = isItem;
				_tileInfo[type]._name = name;
				_tileInfo[type]._description = description;
				_tileInfo[type]._script = script;
				_tileInfo[type]._parameter = parameter;
				_tileInfo[type]._goldValue = goldValue;
			}
		}

		public static bool GetTile(int type, ref TileInfo tile)
		{
			if (_tileInfo.ContainsKey(type))
			{
				tile = _tileInfo[type];
				return true;
			}
			return false;
		}

		public static bool GetTileTexture(int type, ref Texture2D texture)
		{
			if (_tileInfo.ContainsKey(type))
			{
				var result = _tileInfo[type];
				texture = _textures[type];
				return true;
			}
			return false;
		}

		public static bool GetTile(int type, ref Texture2D texture, ref float layerDepth)
		{
			if (_tileInfo.ContainsKey(type))
			{
				var result = _tileInfo[type];
				//texture = result._primaryTexture;
				texture = _textures[type];
				layerDepth = result._layerDepth;
				return true;
			}
			return false;
		}

		public static void Save()
		{
			Stream s = File.Open("tiles.dat", FileMode.OpenOrCreate);
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(s, _tileInfo);
			s.Close();
		}

		public static void Load()
		{
			Stream s = File.Open("tiles.dat", FileMode.Open);
			BinaryFormatter b = new BinaryFormatter();
			_tileInfo = (Dictionary<int, TileInfo>)b.Deserialize(s);
			s.Close();
		}
	}
}
