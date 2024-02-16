using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System.Collections.Generic;

namespace PMB.Controllers
{
    public class TarifController : Controller
    {
        private readonly ILogger<TarifController> _logger;
        TarifDAO dao;
        PendaftarDAO daoMaster;

        public TarifController(ILogger<TarifController> logger)
        {
            _logger = logger;
            dao = new TarifDAO();
            daoMaster = new PendaftarDAO();
        }

        public IActionResult Tarif()
        {
            TarifView data = new TarifView();
            data.ListProdi = daoMaster.GetAllProdi();

            data.ListTagihan = dao.GetAllTagihan();

            int tahunsekarang = DateTime.Now.Year + 1;
            for (int i = tahunsekarang+2; i >= tahunsekarang-7; i--)
            {
                String tahun = i.ToString();
                var dataTahun = new
                {
                    Value = tahun,
                    Text = tahun
                };
                data.ListTahun.Add(dataTahun);
            }
            return View(data);
        }

        public IActionResult GetAllTarifTagihan()
        {
            List<dynamic> data = null;
            List<string> tagihan = dao.GetAllTagihan();
            data = dao.GetAllTarifTagihan(tagihan);

            return Json(data);
        }

        public IActionResult GetAllRefTagihan(string id_prodi)
        {
            List<ListTagihan> data = null;
            data = dao.GetAllRefTagihan(id_prodi);
            return Json(data);
        }

        public IActionResult GetListTagihan()
        {
            List<dynamic> data = null;
            data = dao.GetListTagihan();
            return Json(data);
        }

        public IActionResult GetDetailRefTagihan(string id_prodi, int tahun)
        {
            List<string> dataString = new List<string>();

            List<ListTagihan> dataRef = dao.GetAllRefTagihan(id_prodi);
            dataRef.ForEach(item =>
            {
                dataString.Add(item.nama_tagihan);
            });

            dynamic data = dao.GetDetailTarifTagihan(id_prodi, tahun, dataString);

            return Json(new { tarif = data, tagihan = dataRef });
            //return Json(data);
        }

        [HttpPost]
        public IActionResult SaveTarifTagihan([FromBody] Tarif data)
        {
            try
            {
                if (dao.SimpanTarifTagihan(data))
                {
                    TempData["success"] = "Berhasil menambah data tarif!";
                }
                else
                {
                    TempData["error"] = "Gagal menambah data tarif!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UbahTarifTagihan([FromBody] Tarif data)
        {
            try
            {
                if (dao.UbahTarifTagihan(data))
                {
                    TempData["success"] = "Berhasil mengubah data tarif!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data tarif!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult NonAktifTarifTagihan(string prodi, int tahun)
        {
            try
            {
                if (dao.NonAktifTarifTagihan(prodi, tahun))
                {
                    TempData["success"] = "Berhasil menonaktifkan data tarif!";
                }
                else
                {
                    TempData["error"] = "Gagal menonaktifkan data tarif!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AktifTarifTagihan(string prodi, int tahun)
        {
            try
            {
                if (dao.AktifTarifTagihan(prodi, tahun))
                {
                    TempData["success"] = "Berhasil mengaktifkan data tarif!";
                }
                else
                {
                    TempData["error"] = "Gagal mengaktifkan data tarif!";
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
