namespace S31.Models
{
	public class S3Modelo
	{
		public string? Key { get; set; }
		public DateTime LastModified { get; set; }
		public long Size { get; set; }
		public string? Bucket { get; set; }
	}

}
