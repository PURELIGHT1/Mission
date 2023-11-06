using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;

namespace PMB.Controllers
{
    public class RumusPotonganController : Controller
    {
        private readonly ILogger<RumusPotonganController> _logger;
        PotonganDAO dao;
        MstAngsuranDAO daoAngsuran;
        RumusAngsuranDAO daoRumus;
        public RumusPotonganController(ILogger<RumusPotonganController> logger)
        {
            _logger = logger;
            dao = new PotonganDAO();
            daoAngsuran = new MstAngsuranDAO();
            daoRumus = new RumusAngsuranDAO();
        }
        public IActionResult RumusPotongan(int id_rumus)
        {
            ViewBag.id_rumus = id_rumus;
            return View();
        }

        public IActionResult GetAllPotonganByRumus(int id_rumus)
        {
            List<Potongan> data = null;
            data = dao.GetAllPotonganByRumus(id_rumus.ToString());
            return Json(data);
        }

        public IActionResult GetDetailRmsPotongan(int id) 
        {
            Potongan data = null;
            data = dao.GetDetailPotongan(id);
            return Json(data);
        }

        public IActionResult ListJalur()
        {
            List<Jalur> data = null;
            data = daoAngsuran.GetJalur();
            return Json(data);
        }

        public IActionResult ListTagihan()
        {
            List<Tagihan> data = null;
            data = daoRumus.GetTagihan();
            return Json(data);
        }

        [HttpPost]
        public IActionResult SaveDataRumus([FromBody] Potongan data)
        {
            try
            {
                if (dao.TambahUbahDataRmsPotongan(data))
                {
                    if(data.id_dtl_potongan < 1)
                    {
                        TempData["success"] = "Berhasil menambah data rumus potongan!";
                    }
                    else
                    {
                        TempData["success"] = "Berhasil mengubah data rumus potongan!";
                    }
                }
                else
                {
                    if (data.id_dtl_potongan < 1)
                    {
                        TempData["error"] = "Gagal menambah data rumus potongan!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah data rumus potongan!";
                    }
                }

                return Json(new { success = true, id_rumus = data.id_rumus });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }


        [HttpPost]
        public IActionResult DeleteDataRumus(int id, string id_rumus)
        {
            try
            {
                if (dao.HapusDataRmsPotongan(id))
                {
                    TempData["success"] = "Berhasil menghapus data rumus potongan!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data rumus potongan!";
                }

                return Json(new { success = true, id_rumus = id_rumus });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }
    }
}
