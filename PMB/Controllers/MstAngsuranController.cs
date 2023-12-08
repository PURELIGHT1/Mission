using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System;
using System.Diagnostics;
using System.Dynamic;

namespace PMB.Controllers
{
    public class MstAngsuranController : Controller
    {
        private readonly ILogger<MstAngsuranController> _logger;
        MstAngsuranDAO dao;
        RumusAngsuranDAO daoRumus;
        public MstAngsuranController(ILogger<MstAngsuranController> logger)
        {
            _logger = logger;
            dao = new MstAngsuranDAO();
            daoRumus = new RumusAngsuranDAO();
        }

        public IActionResult MstAngsuran()
        {
            MstAngsuranView objek = new MstAngsuranView();
            objek.JalurList = dao.GetJalur();

            objek.AllTahunAkademik = objek.getAllTahunAkademik();
            return View(objek);
        }

        public IActionResult GetAllAngsuran()
        {
            List<MstAngsuran> data = null;
            data = dao.GetAllAngsuran();
            return Json(data);
        }

        [HttpPost]
        public IActionResult SaveMstAngsuran([FromBody] StoreMstAngsuran data)
        {
            try
            {
                if (dao.SimpanDataAngsuran(data))
                {
                    if (data.id_rumus == 0)
                    {
                        TempData["success"] = "Berhasil menambah data angsuran!";
                    }
                    else
                    {
                        TempData["success"] = "Berhasil mengubah data angsuran!";
                    }
                }
                else
                {
                    if (data.id_rumus == 0)
                    {
                        TempData["error"] = "Gagal menambah data angsuran!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah data angsuran!";
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
        public IActionResult HapusMstAngsuran(int id)
        {
            try
            {
                if (dao.HapusAngsuran(id))
                {
                 
                    TempData["success"] = "Berhasil menghapus data angsuran!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data angsuran!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        public IActionResult DetailMstAngsuran(int id_rumus)
        {
            RmsAngsuranView objek = new RmsAngsuranView();
            objek.RmsAngsuran.id_rumus = id_rumus;
            objek.RmsAngsuran.jlh_angsuran = dao.JumlahAngsuran(id_rumus);
            objek.TagihanList = daoRumus.GetTagihan();
            return View(objek);
        }

        //public IActionResult TambahMstAngsuran()
        //{
        //    MstAngsuranView objek = new MstAngsuranView();

        //    int countMstAngsuran = dao.CountMstAngsuran() + 1;
        //    objek.MstAngsuran.id_rumus = countMstAngsuran.ToString();

        //    return View(objek);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult TambahMstAngsuran(MstAngsuranView ags)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (dao.GetJalurTahunPeriode(ags.MstAngsuran.kd_jalur, ags.MstAngsuran.thnakademik, ags.MstAngsuran.periode) != null)
        //        {
        //            string jalur = dao.GetNamaJalur(ags.MstAngsuran.kd_jalur);
        //            TempData["error"] = $"Data master angsuran { jalur } dengan tahun akademik { ags.MstAngsuran.thnakademik } dan periode { ags.MstAngsuran.periode } sudah ada!";

        //            ags.JalurList = dao.GetJalur();
        //            return View(ags);
        //        }
        //        else
        //        {
        //            if (dao.SimpanAngsuran(ags))
        //            {
        //                TempData["success"] = "Berhasil menambahkan data angsuran!";
        //            }
        //            else
        //            {
        //                TempData["error"] = "Gagal menambahkan data angsuran!";
        //            }
        //        }

        //        return RedirectToAction("MstAngsuran");
        //    }
        //    else
        //    {
        //        ags.JalurList = dao.GetJalur();
        //        return View(ags);
        //    }
        //}

        //public IActionResult UbahMstAngsuran(string id_rumus)
        //{
        //    MstAngsuranView objek = new MstAngsuranView();
        //    objek.MstAngsuran.id_rumus = id_rumus;

        //    objek.MstAngsuran = dao.DetailAngsuran(id_rumus);

        //    return View(objek);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UbahMstAgs(MstAngsuranView ags)
        //{
        //    if (dao.UbahAngsuran(ags))
        //    {
        //        TempData["success"] = "Berhasil mengubah data angsuran!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Gagal mengubah data angsuran!";
        //    }

        //    return RedirectToAction("MstAngsuran");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteMstAngsuran(string id_rumus)
        //{
        //    if (dao.HapusAngsuran(id_rumus))
        //    {
        //        TempData["success"] = "Berhasil menghapus data angsuran!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Gagal menghapus data angsuran!";
        //    }

        //    return RedirectToAction("MstAngsuran");
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

