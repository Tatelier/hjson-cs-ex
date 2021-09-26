using System.IO;
using System.Text;
using Hjson;

namespace HjsonEx
{
	/// <summary>
	/// Hjson拡張
	/// 要素がないときはnullで返却するようにし、使う側がnull許容で処理する思想に変更
	/// </summary>
	public static class HjsonEx
	{
		/// <summary>
		/// Hjsonファイルを読み込む
		/// </summary>
		/// <remarks>
		/// エンコードも自動判別する
		/// </remarks>
		/// <param name="path">ファイルパス</param>
		/// <returns>Jsonオブジェクト</returns>
		public static Hjson.JsonValue LoadEx(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			var encoding = Utility.GetEncodingFromFile(path);
			var stream = new StreamReader(path, encoding, false);
			return Hjson.HjsonValue.Load(stream);
		}

		static void SaveSetObject(StringBuilder sb, int tabCount, Hjson.JsonValue json)
		{
			switch (json.JsonType)
			{
				case JsonType.Boolean:
				case JsonType.Number:
					{
						var str = json.Qstr();
						sb.Append($"{str},");
					}
					break;
				case JsonType.String:
					{
						var str = json.Qs();

						if (str.IndexOf('\n') != -1)
						{
							sb.AppendLine("'''");
							foreach (var s in str.Split('\n'))
							{
								sb.Append(new string('\t', tabCount));
								sb.Append($"{s}");
							}
							sb.Append("''',");
						}
						else
						{
							sb.Append($"\"{str}\",");
						}
					}
					break;
				case JsonType.Object:
					{
						sb.Append("{");
						if (json.Count > 0)
						{
							foreach (System.Collections.Generic.KeyValuePair<string, JsonValue> item in json)
							{
								sb.AppendLine();
								sb.Append(new string('\t', tabCount + 1));
								sb.Append(item.Key);
								sb.Append(": ");
								SaveSetObject(sb, tabCount + 1, item.Value);
							}
							sb.AppendLine();
							sb.Append(new string('\t', tabCount));
						}
						else
						{
							sb.AppendLine();
							sb.AppendLine();
							sb.Append(new string('\t', tabCount));
						}
						sb.Append("},");
					}
					break;
				case JsonType.Array:
					{
						sb.Append("[");
						if (json.Count > 0)
						{
							foreach (JsonValue item in json)
							{
								sb.AppendLine();
								sb.Append(new string('\t', tabCount + 1));
								SaveSetObject(sb, tabCount + 1, item);
							}
							sb.AppendLine();
							sb.Append(new string('\t', tabCount));
						}
						sb.Append("],");
					}
					break;
			}

			if (tabCount == 0)
			{
				sb.Remove(sb.Length - 1, 1);
			}
		}

		/// <summary>
		/// ファイルに保存する
		/// </summary>
		/// <param name="json"></param>
		/// <param name="path"></param>
		/// <param name="format"></param>
		public static void SaveEx(this Hjson.JsonValue json, string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
            if (dir.Length == 0)
            {
				dir = "./";
            }
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			StringBuilder sb = new StringBuilder();

			SaveSetObject(sb, 0, json);

			File.WriteAllText(path, $"{sb}");
		}

		/// <summary>
		/// bool?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static bool? EQb(this JsonValue val, string key)
		{
			val = EQv(val, key);
			return val?.JsonType == JsonType.Boolean ? val.Qb() : (bool?)null;
		}

		/// <summary>
		/// int?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static int? EQi(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			return val.ToValue() is long v ? (int)v : (int?)null;
		}

		/// <summary>
		/// int?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static uint? EQui(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			return val.ToValue() is long v ? (uint)v : (uint?)null;
		}

		/// <summary>
		/// long?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static long? EQl(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			return val.ToValue() is long v ? v : (long?)null;
		}

		/// <summary>
		/// ulong?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static ulong? EQul(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			return val.ToValue() is long v ? (ulong)v : (ulong?)null;
		}

		/// <summary>
		/// float?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static float? EQf(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			switch (val.ToValue())
			{
				case double dv:
					return (float)dv;
				case long lv:
					return (float)lv;
				default:
					return null;
			}
		}

		/// <summary>
		/// double?型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static double? EQd(this JsonValue val, string key)
		{
			val = EQv(val, key);
			if (val?.JsonType != JsonType.Number) return null;
			return val.ToValue() is double v ? v : (double?)null;
		}


		/// <summary>
		/// string型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static string EQs(this JsonValue val)
		{
			return val?.JsonType == JsonType.String ? val.Qs() : null;
		}

		/// <summary>
		/// string型で取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static string EQs(this JsonValue val, string key)
		{
			val = EQv(val, key);
			return val.EQs();
		}

		/// <summary>
		/// Jsonの配列を取得する
		/// </summary>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static Hjson.JsonArray EQa(this JsonValue val, string key)
		{
			val = EQv(val, key);
			return val?.JsonType == JsonType.Array ? val.Qa() : null;
		}

		/// <summary>
		/// Jsonのオブジェクトを取得する
		/// </summary>
		/// <remarks>
		/// キーは"."で次の階層の探索ができる
		/// </remarks>
		/// <param name="val">探索オブジェクト</param>
		/// <param name="key">キー</param>
		/// <returns>値がない場合:null, それ以外:取得できた値</returns>
		public static JsonValue EQv(this JsonValue val, string key)
		{
			if (val == null) return null;

			var split = key.Split('.');

			JsonValue result = val;
			for (int i = 0; i < split.Length; i++)
			{
				result = result.ContainsKey(split[i]) ? result[split[i]] : null;
				if (result == null) return null;
			}

			return result;
		}
	}
}