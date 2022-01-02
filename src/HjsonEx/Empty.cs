using Hjson;

namespace HjsonEx
{
	/// <summary>
	/// 空オブジェクト
	/// </summary>
	public static class Empty
	{
		/// <summary>
		/// 配列
		/// </summary>
		public static Hjson.JsonArray Array { get; } = new JsonArray();

		/// <summary>
		/// オブジェクト
		/// </summary>
		public static Hjson.JsonValue Value { get; } = new JsonObject();
	}
}