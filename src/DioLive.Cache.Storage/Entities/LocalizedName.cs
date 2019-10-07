namespace DioLive.Cache.Storage.Entities
{
	public class LocalizedName
	{
		public LocalizedName(string culture, string name)
		{
			Culture = culture;
			Name = name;
		}

		public string Culture { get; set; }
		public string Name { get; set; }
	}
}