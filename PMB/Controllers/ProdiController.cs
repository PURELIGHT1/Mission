using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMB.DAO;
using PMB.Models;
using System.IO;

namespace PMB.Controllers
{
    public class ProdiController : Controller
    {
        private readonly ILogger<ProdiController> _logger;
        PendaftarDAO pendaftarDAO;
        ProdiDAO dao;

        public ProdiController(ILogger<ProdiController> logger)
        {
            _logger = logger;
            dao = new ProdiDAO();
            pendaftarDAO = new PendaftarDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Diterima() 
        {
            ProdiDiterimaView data = new ProdiDiterimaView();
            data.AllTahunAkademik = data.getAllTahunAkademik();
            data.JalurList = pendaftarDAO.GetAllJalur();
            data.ProdiList = pendaftarDAO.GetAllProdi();

            return View(data);
        }

        public IActionResult GetPilProdiMhs(string ta, string jalur, string prodi, bool excel = false) 
        {
            if (String.IsNullOrEmpty(ta))
            {
                String tahun = DateTime.Now.Year + "/" + (DateTime.Now.Year + 1);
                ta = tahun;
                if (DateTime.Now.Month > 7)
                {
                    ta = DateTime.Now.Year + 1 + "/" + (DateTime.Now.Year + 2);
                }
            }

            if (String.IsNullOrEmpty(jalur))
            {
                jalur = "All";
            }

            if (String.IsNullOrEmpty(prodi))
            {
                prodi = "All";
            }

            if(excel == true)
            {
                List<ProdiDiterimaExcel> data = new List<ProdiDiterimaExcel> ();
                string serializedData = TempData["ProdiDiterimaData"] as string;
                if (!string.IsNullOrEmpty(serializedData))
                {
                    data = JsonConvert.DeserializeObject<List<ProdiDiterimaExcel>>(serializedData);
                }
                return Json(data);
            }
            else
            {
                List<dynamic> data = null;
                data = dao.GetPilProdiMhs(ta, jalur, prodi);
                return Json(data);
            }
        }

        [HttpPost]
        public IActionResult GetNamaProdi(string id_prodi)
        {
            try
            {
                string prodi = pendaftarDAO.GetNamaProdi(id_prodi);
                if (prodi != null)
                {
                    return Json(new { success = true, message = prodi });
                }
                else
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UbahProdiDiterima([FromBody] ProdiDiterima data)
        {
            try
            {
                if (dao.UbahProdiDiterima(data))
                {
                    TempData["success"] = "Berhasil menerima calon menjadi prodi "+data.nm_prodi+"!";
                }
                else
                {
                    TempData["error"] = "Gagal menerima calon menjadi prodi " + data.nm_prodi + "!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult TambahProdiDiterima([FromBody] List<ProdiDiterimaExcel> data)
        {
            try
            {
                if (dao.TambahProdiDiterima(data))
                {
                    TempData["success"] = "Berhasil menambah prodi diterima!";
                }
                else
                {
                    TempData["error"] = "Gagal menambah prodi diterima!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcelProdiDiterima(IFormFile excelFile)
        {
            try
            {
                if (excelFile != null && excelFile.Length > 0)
                {
                    ProdiDiterimaView data = new ProdiDiterimaView();
                    using (var stream = new MemoryStream())
                    {
                        await excelFile.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            int rowCount = worksheet.Dimension.Rows;
                            int colCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                string kd_calon = worksheet.Cells[row, 1].Value?.ToString();
                                string masuk = worksheet.Cells[row, 2].Value?.ToString();

                                ProdiDiterimaExcel DataExcel = dao.CekDataExcelDatabase(kd_calon, masuk);
                                data.ListDataProdiExcel.Add(DataExcel);
                            } 

                        }
                    }
                    string serializedData = JsonConvert.SerializeObject(data.ListDataProdiExcel);

                    TempData["ProdiDiterimaData"] = serializedData;
                }

                return Redirect($"~/Prodi/Diterima?excel=true");
            }
            catch (Exception)
            {

                return Redirect($"~/Prodi/Diterima");
            }
        }

        public IActionResult TemplateExcel()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add header row
                worksheet.Cells["A1"].Value = "Kode_calon";
                worksheet.Cells["B1"].Value = "Prodi Diterima";

                // Add data rows
                worksheet.Cells["A2"].Value = "200710898";
                worksheet.Cells["B2"].Value = "Informatika";

                worksheet.Cells["A3"].Value = "200710905";
                worksheet.Cells["B3"].Value = "Teknik Industri";

                // Generate a FileStreamResult
                var stream = new MemoryStream(package.GetAsByteArray());
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "Template Excel.xlsx"
                };
            }
        }
    }
}
