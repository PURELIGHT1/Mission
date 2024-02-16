using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace PMB.Controllers
{
    public class MstReferensiController : Controller
    {
        private readonly ILogger<MstReferensiController> _logger;
        MstReferensiDAO dao;
        PendaftarDAO pendaftarDAO;
        public MstReferensiController(ILogger<MstReferensiController> logger)
        {
            _logger = logger;
            dao = new MstReferensiDAO();
            pendaftarDAO = new PendaftarDAO();
        }

        public IActionResult MstReferensi(int id)
        {
            MstReferensiView data = new MstReferensiView();
            data.ProvinsiList = pendaftarDAO.GetAllProvinsi();
            if(id > 0)
            {
                data.MstReferensi = dao.DetailMstReferensi(id);
                data.title = "Ubah";
                data.KotaList = pendaftarDAO.GetAllKota(data.MstReferensi.KD_PROP);
                data.KecamatanList = pendaftarDAO.GetAllKecamatan(data.MstReferensi.ID_KABUPATEN);
            }
            else
            {
                data.title = "Tambah";
            }
            return View(data);
        }

        public IActionResult GetReferensiList()
        {
            List<dynamic> data = null;

            data = dao.GetFilterReferensi("semua");
            
             return Json(data);
        }

        public JsonResult GetKotaByProvinsiId(string provinsiId)
        {
            var kotaItems = pendaftarDAO.GetAllKota(provinsiId);

            return Json(kotaItems);
        }

        public JsonResult GetKecamatanByKotaId(int kotaId)
        {

            var kecamatanItems = pendaftarDAO.GetAllKecamatan(kotaId);

            return Json(kecamatanItems);
        }

        public JsonResult GetSMAByKotaId(int kotaId)
        {

            var smaItems = pendaftarDAO.GetAllSMA(kotaId);

            return Json(smaItems);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TambahMstReferensi(MstReferensiView data)
        {
            //if (ModelState.IsValid)
            //{
                data.MstReferensi.NAMA_SMA.ToUpper();
                data.MstReferensi.ALAMAT.ToUpper();
                if (dao.SimpanReferensi(data))
                {
                    TempData["success"] = "Berhasil menambahkan data referensi!";
                }
                else
                {
                    TempData["error"] = "Gagal menambahkan data referensi!";
                }

                return RedirectToAction("MstReferensi");
           
        }

        [HttpPost]
        public IActionResult DeleteMstReferensi(int id_sma)
        {
            if (dao.HapusReferensi(id_sma))
            {
                TempData["success"] = "Berhasil menghapus data referensi!";
            }
            else
            {
                TempData["error"] = "Gagal menghapus data referensi!";
            }

            return RedirectToAction("MstReferensi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UbahMstReferensi(MstReferensiView referensiView)
        {
            //referensiView.MstReferensi.NAMA_SMA.ToUpper();
            referensiView.MstReferensi.ALAMAT.ToUpper();
            if (dao.UbahReferensi(referensiView))
            {
                TempData["success"] = "Berhasil mengubah data referensi!";
            }
            else
            {
                TempData["error"] = "Gagal mengubah data referensi!";
            }

            return RedirectToAction("MstReferensi");
        }
    }
}
