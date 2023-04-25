using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using S31.Models;
using System.Reflection;


namespace S31.Controllers
{
	public class S31Controller : Controller
	{
		private static string? Cubeta2,Nombre2,Contraseña2; 
		private readonly IAmazonS3 _s3Client;

		public S31Controller(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        } 


		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Dashboard(Usuario model)
		{
			Cubeta2 = model.Cubeta;
			Nombre2 = model.Nombre;
			Contraseña2 = model.Contraseña;
			if (model.Cubeta != string.Empty && model.Nombre != string.Empty && model.Contraseña != string.Empty)
			{
				try
				{
					var _s3Client = new AmazonS3Client(model.Nombre, model.Contraseña, Amazon.RegionEndpoint.USEast1);

					var listRequest = new ListObjectsV2Request
					{
						BucketName = model.Cubeta
					};

					var response = await _s3Client.ListObjectsV2Async(listRequest);
					var s3Objects = response.S3Objects;

					var S3Modelo = s3Objects.Select(o => new S3Modelo
					{
						Key = o.Key,
						LastModified = o.LastModified,
						Size = o.Size,
						Bucket = o.BucketName
					}).ToList();

					return View(S3Modelo);
				}
				catch (ArgumentException e)
				{
					TempData["ErrorCampos"] = "Algunos de los campos estan vacios.";
					return RedirectToAction("Index");
				}
				catch (AmazonS3Exception e)
				{
					TempData["ErrorCredenciales"] = "Error al iniciar, verifique sus credenciales.";
					return RedirectToAction("Index");
				}
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public async Task<IActionResult> Download(string key)
		{
			var _s3Client = new AmazonS3Client(Nombre2, Contraseña2, Amazon.RegionEndpoint.USEast1);


			var request = new GetObjectRequest()
			{
				BucketName = Cubeta2,
				Key = key
			};
			using GetObjectResponse response = await _s3Client.GetObjectAsync(request);
			using Stream responseStream = response.ResponseStream;
			var Stream = new MemoryStream();
			await responseStream.CopyToAsync(Stream);
			Stream.Position = 0;
			return new FileStreamResult(Stream, response.Headers["Content-Type"])
			{
				FileDownloadName = key
			};

		}







	}
}

