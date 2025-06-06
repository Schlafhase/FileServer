using Microsoft.AspNetCore.Mvc;

namespace UploadBackend;

public class FileController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}