using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;

namespace PMB.Controllers
{
    public class WilayahController : Controller
    {
        private readonly ILogger<WilayahController> _logger;
        PendaftarDAO pendaftarDAO;
        WilayahDAO dao;
        public WilayahController(ILogger<WilayahController> logger)
        {
            _logger = logger;
            pendaftarDAO = new PendaftarDAO();
            dao = new WilayahDAO();
        }
        public IActionResult Provinsi()
        {
            return View();
        }

        public IActionResult GetPropinsi()
        {
            List<dynamic> data = null;
            data = pendaftarDAO.GetProvinsi();
            return Json(data);
        }

        [HttpPost]
        public IActionResult SavePropinsi([FromBody] StoreProvinsi data)
        {
            try
            {
                if (dao.TambahDataPropinsi(data))
                {
                    if (String.IsNullOrEmpty(data.kd_prop))
                    {
                        TempData["success"] = "Berhasil menambah data Propinsi!";
                    }
                    else
                    {
                        TempData["success"] = "Berhasil mengubah data Propinsi!";
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(data.kd_prop))
                    {
                        TempData["success"] = "Gagal menambah data Propinsi!";
                    }
                    else
                    {
                        TempData["success"] = "Gagal mengubah data Propinsi!";
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
        public IActionResult NonAktifPropinsi(string id)
        {
            try
            {
                if (dao.NonAktifDataPropinsi(id))
                {
                    TempData["success"] = "Berhasil menonaktifkan data Propinsi!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data Propinsi!";
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AktifPropinsi(string id)
        {
            try
            {
                if (dao.AktifDataPropinsi(id))
                {
                    TempData["success"] = "Berhasil menonaktifkan data Propinsi!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data Propinsi!";
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult HapusPropinsi(string id)
        {
            try
            {
                if (dao.HapusDataPropinsi(id))
                {
                    TempData["success"] = "Berhasil menonaktifkan data Propinsi!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data Propinsi!";
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        public IActionResult Kabupaten(string id)
        {
            Provinsi data = new Provinsi();
            data = pendaftarDAO.GetDetailProvinsi(id);
            return View(data);
        }
        public IActionResult GetKabupaten(string id)
        {
            List<dynamic> data = null;
            data = pendaftarDAO.GetKota(id);
            return Json(data);
        }

        [HttpPost]
        public IActionResult SaveKabupaten([FromBody] StoreKota data)
        {
            try
            {
                if (dao.TambahDataKabupaten(data))
                {
                    if (data.ID_KABUPATEN == 0)
                    {
                        TempData["success"] = "Berhasil menambah data Kabupaten!";
                    }
                    else
                    {
                        TempData["success"] = "Berhasil mengubah data Kabupaten!";
                    }
                }
                else
                {
                    if (data.ID_KABUPATEN == 0)
                    {
                        TempData["success"] = "Gagal menambah data Kabupaten!";
                    }
                    else
                    {
                        TempData["success"] = "Gagal mengubah data Kabupaten!";
                    }
                }

                return Json(new { success = true, kd_prop = data.KD_PROP });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult NonAktifKabupaten(int id)
        {
            try
            {
                if (dao.NonAktifDataKab(id))
                {
                    TempData["success"] = "Berhasil menonaktifkan data Kabupaten!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data Kabupaten!";
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AktifKabupaten(int id)
        {
            try
            {
                if (dao.AktifDataKab(id))
                {
                    TempData["success"] = "Berhasil menonaktifkan data Kabupaten!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data Kabupaten!";
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

    }
}
