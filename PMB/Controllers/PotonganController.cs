using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;

namespace PMB.Controllers
{
    public class PotonganController : Controller
    {
        private readonly ILogger<PotonganController> _logger;
        PotonganDAO dao;
        RumusAngsuranDAO rumusAngsuranDAO;

        public PotonganController(ILogger<PotonganController> logger)
        {
            _logger = logger;
            dao = new PotonganDAO();
            rumusAngsuranDAO = new RumusAngsuranDAO();
        }

        public IActionResult Potongan()
        {
            return View();
        }

        public IActionResult GetPotongan()
        {
            List<dynamic> data = null;
            data = dao.GetPotonganMandiri();
            return Json(data);
        }

        public IActionResult GetListTagihan()
        {
            List<Tagihan> data = null;
            data = rumusAngsuranDAO.GetTagihan();
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
        
    }
}
