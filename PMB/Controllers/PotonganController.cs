using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PMB.DAO;
using PMB.Models;

namespace PMB.Controllers
{
    public class PotonganController : Controller
    {
        private readonly ILogger<PotonganController> _logger;
        PotonganDAO dao;
        PendaftarDAO pendaftarDAO;

        public PotonganController(ILogger<PotonganController> logger)
        {
            _logger = logger;
            dao = new PotonganDAO();
            pendaftarDAO = new PendaftarDAO();
        }

        public IActionResult Potongan()
        {
            PotonganView data = new PotonganView();
            data.AllTahunAkademik = data.getAllTahunAkademik();
            data.JalurList = pendaftarDAO.GetAllJalur();
            return View(data);
        }

        

        public IActionResult GetPotongan(string ta, string jalur)
        {
            List<dynamic> data = null;
            if (String.IsNullOrEmpty(ta))
            {
                String tahun = DateTime.Now.Year + "/" + (DateTime.Now.Year + 1);
                ta = tahun;
                if (DateTime.Now.Month > 7)
                {
                    ta = DateTime.Now.Year + 1 + "/" + (DateTime.Now.Year + 2);
                }
            }

            if (String.IsNullOrEmpty (jalur))
            {
                jalur = "All";
            }
            data = dao.GetPotonganMandiri(ta, jalur);
            return Json(data);
        }

        public IActionResult GetPotonganById(int id)
        {
            dynamic data = null;
            data = dao.GetPotonganMandiriById(id);
            return Json(data);
        }

        public IActionResult GetListTagihan(string id)
        {
            List<dynamic> data = null;
            data = dao.GetTagihan(id);
            return Json(data);
        }

        public IActionResult CekKodeCalon(string id)
        {
            if (dao.CekKodeCalon(id))
            {
                return Json(new { success = true });
            }
            else
            {

                return Json(new { success = false });
            }
        }

        [HttpPost]
        public IActionResult SaveDataPotongan([FromBody] PotonganMhs mhs)
        {
            try
            {
                if (dao.SaveDataPotongan(mhs))
                {
                    if (mhs.id_potongan < 1)
                    {
                        TempData["success"] = "Berhasil menambah data potongan mahasiswa!";
                    }
                    else
                    {
                        TempData["success"] = "Berhasil mengubah data potongan mahasiswa!";
                    }
                }
                else
                {
                    if (mhs.id_potongan < 1)
                    {
                        TempData["error"] = "Gagal menambah data potongan mahasiswa!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah data potongan mahasiswa!";
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult HapusPotongan(int id)
        {
            try
            {
                if (dao.HapusPotongan(id))
                {
                    TempData["success"] = "Berhasil menghapus data potongan!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data potongan!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        public IActionResult ExportExcelPotongan(string ta, string jalur)
        {

            List<dynamic> data = null;
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

            data = dao.GetPotonganMandiri(ta, jalur);
            try
            {
                using (var package = new ExcelPackage())
                {
                    // Tambahkan lembar Excel
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    // Tambahkan header
                    worksheet.Cells["A1"].Value = "KD Calon";
                    worksheet.Cells["B1"].Value = "Jalur";
                    worksheet.Cells["C1"].Value = "Tahun Akademik";
                    worksheet.Cells["D1"].Value = "Jenis Potongan";
                    worksheet.Cells["E1"].Value = "Tagihan Terpotong";
                    worksheet.Cells["F1"].Value = "Total";
                    worksheet.Cells["G1"].Value = "Keterangan";

                    // Isi data
                    var row = 2;
                    foreach (var item in data)
                    {
                        worksheet.Cells[string.Format("A{0}", row)].Value = item.kd_calon;
                        worksheet.Cells[string.Format("B{0}", row)].Value = item.nama_jalur;
                        worksheet.Cells[string.Format("C{0}", row)].Value = "TA "+item.thnakademik;
                        worksheet.Cells[string.Format("D{0}", row)].Value = item.jenis;
                        worksheet.Cells[string.Format("E{0}", row)].Value = item.nama_tagihan;
                        worksheet.Cells[string.Format("F{0}", row)].Value = item.jlh_total;
                        worksheet.Cells[string.Format("G{0}", row)].Value = item.jns_potongan;
                        row++;
                    }

                    // Konversi ke byte array
                    var excelBytes = package.GetAsByteArray();

                    // Kembalikan file Excel sebagai respons HTTP
                    if (jalur.Equals("All"))
                    {
                        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data Potongan Mahasiswa Baru TA - " + ta + ".xlsx");
                    }
                    else
                    {
                        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data Potongan Mahasiswa Baru TA - " + ta + " & Jalur "+jalur+".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat menghasilkan file Excel: " + ex.Message);
            }
        }

    }
}
