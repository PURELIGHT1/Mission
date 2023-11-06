using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Web.WebPages;

namespace PMB.Controllers
{
    public class RumusAngsuranController : Controller
    {
        private readonly ILogger<RumusAngsuranController> _logger;
        RumusAngsuranDAO dao;
        public RumusAngsuranController(ILogger<RumusAngsuranController> logger)
        {
            _logger = logger;
            dao = new RumusAngsuranDAO();
        }

        public IActionResult GetRumusAngsuran(int id)
        {
            List<RmsAngsuran> data = null;
            data = dao.DetailRumusAngsuran(id);

            return Json(data);
        }

        public IActionResult GetDataRumusAngsuran(int id)
        {
            dynamic data = null;
            data = dao.DetailDataRumusAngsuran(id);

            return Json(data);
        }

        public JsonResult CariJaminanTgl(string id_rumus, int angsuran_ke)
        {
            var data = dao.GetJaminanTgl(id_rumus, angsuran_ke);
            return Json(data);
        }

        [HttpPost]
        public IActionResult SaveRumus([FromBody] StoreRmsAngsuran data)
        {
            try
            {
                int total = 0;
                List<int> presentasi = dao.CekTotalPresentasiRumus(data.id_rumus.ToString(), data.id_tagihan);
                if(presentasi.Count() > 0)
                {
                    presentasi.ForEach(item =>
                    {
                        total = total + item;
                    });
                }

                if (data.id_detail < 1)
                {
                    total = total + data.jml_persentase;
                }
                else
                {
                    dynamic dataRumus = dao.DetailDataRumusAngsuran(data.id_detail);
                    total = total - dataRumus.JML_PERSENTASE;
                    total = total + data.jml_persentase;
                }

                if (total < 101)
                {

                    if (dao.SimpanRumus(data))
                    {
                        if (data.id_detail < 1)
                        {
                            TempData["success"] = "Berhasil menambah data rumus angsuran!";
                        }
                        else
                        {
                            TempData["success"] = "Berhasil mengubah data rumus angsuran!";
                        }
                    }
                    else
                    {
                        if (data.id_detail < 1)
                        {
                            TempData["error"] = "Gagal menambah data rumus angsuran!";
                        }
                        else
                        {
                            TempData["error"] = "Gagal mengubah data rumus angsuran!";
                        }
                    }
                }
                else
                {
                    if (data.id_detail < 1)
                    {
                        TempData["error"] = "Gagal menambah data rumus angsuran, Total Persentase melebihi 100%!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah data rumus angsuran, Total Persentase melebihi 100%!";
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
        public IActionResult DeleteRmsAngsuran(int id)
        {
            try
            {
                if (dao.HapusRmsAngsuran(id))
                {
                    TempData["success"] = "Berhasil menghapus data rumus angsuran!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data rumus angsuran!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        //public IActionResult TambahRumus(int id_rumus)
        //{
        //    RmsAngsuranView objek = new RmsAngsuranView();
        //    objek.RmsAngsuran.id_rumus = id_rumus;

        //    objek.jlh_angsuran = dao.GetJmlAngsuran(id_rumus);
        //    objek.RmsAngsuranList = dao.GetRumusByID(id_rumus);
        //    objek.TagihanList = dao.GetTagihan();

        //    return View(objek);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult TambahRumus(RmsAngsuranView rumusAgs)
        //{
        //    int totalPersentase = (dao.GetJumlahPersentase(rumusAgs.RmsAngsuran.id_rumus, rumusAgs.RmsAngsuran.id_tagihan) + rumusAgs.RmsAngsuran.prosentase);
        //    int batasPersentase = 100 - dao.GetJumlahPersentase(rumusAgs.RmsAngsuran.id_rumus, rumusAgs.RmsAngsuran.id_tagihan);

        //    if (ModelState.IsValid)
        //    {
        //        if (totalPersentase > 100)
        //        {
        //            string jenis_tagihan = dao.GetJenisTagihan(rumusAgs.RmsAngsuran.id_tagihan);

        //            if(batasPersentase != 0)
        //            {
        //                TempData["error"] = $"Persentase { jenis_tagihan } tidak boleh lebih dari { batasPersentase }%!";
        //            }
        //            else
        //            {
        //                TempData["error"] = $"Tagihan { jenis_tagihan } sudah mencapai batas maksimum dan tidak bisa ditambahkan lagi!";
        //            }

        //            rumusAgs.RmsAngsuran.id_rumus = rumusAgs.RmsAngsuran.id_rumus;
        //            rumusAgs.jlh_angsuran = dao.GetJmlAngsuran(rumusAgs.RmsAngsuran.id_rumus);
        //            rumusAgs.TagihanList = dao.GetTagihan();
        //            return View(rumusAgs);
        //        }
        //        else
        //        {
        //            if (dao.SimpanRumus(rumusAgs))
        //            {
        //                TempData["success"] = "Berhasil menambahkan data rumus angsuran!";
        //            }
        //            else
        //            {
        //                TempData["error"] = "Gagal menambahkan data rumus angsuran!";
        //            }
        //        }

        //        return Redirect($"~/MstAngsuran/DetailMstAngsuran?id_rumus={ rumusAgs.RmsAngsuran.id_rumus }");
        //    }
        //    else
        //    {
        //        rumusAgs.RmsAngsuran.id_rumus = rumusAgs.RmsAngsuran.id_rumus;
        //        rumusAgs.jlh_angsuran = dao.GetJmlAngsuran(rumusAgs.RmsAngsuran.id_rumus);
        //        rumusAgs.TagihanList = dao.GetTagihan();

        //        return View(rumusAgs);
        //    }
        //}

        //public IActionResult UbahRumus(int id_detail)
        //{
        //    RmsAngsuranView objek = new RmsAngsuranView();
        //    objek.RmsAngsuran.id_detail = id_detail;

        //    objek.RmsAngsuran = dao.DetailRumus(id_detail);
        //    objek.TagihanList = dao.GetTagihan();

        //    return View(objek);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UbahRumus(RmsAngsuranView ubhRumus)
        //{
        //    // Total persentase per tagihan
        //    int historiPersentase = dao.GetJumlahPersentase(ubhRumus.RmsAngsuran.id_rumus, ubhRumus.RmsAngsuran.id_tagihan);

        //    // Persentase yang sedang diedit
        //    int recentPersentase = dao.GetPersentaseEdit(ubhRumus.RmsAngsuran.id_detail, ubhRumus.RmsAngsuran.id_tagihan);

        //    // Penghitungan total persentase
        //    int totalPersentase = (historiPersentase - recentPersentase) + ubhRumus.RmsAngsuran.prosentase;

        //    // Penentuan batas pengisian persentase
        //    int batasPersentase = 100 - (historiPersentase - recentPersentase);

        //    if (ModelState.IsValid)
        //    {
        //        if (totalPersentase > 100)
        //        {
        //            string jenis_tagihan = dao.GetJenisTagihan(ubhRumus.RmsAngsuran.id_tagihan);

        //            if (batasPersentase != 0)
        //            {
        //                TempData["error"] = $"Persentase { jenis_tagihan } tidak boleh lebih dari { batasPersentase }%!";
        //            }
        //            else
        //            {
        //                TempData["error"] = $"Tagihan { jenis_tagihan } sudah mencapai batas maksimum dan tidak bisa ditambahkan lagi!";
        //            }

        //            ubhRumus.RmsAngsuran.id_detail = ubhRumus.RmsAngsuran.id_detail;
        //            ubhRumus.RmsAngsuran.id_rumus = ubhRumus.RmsAngsuran.id_rumus;
        //            ubhRumus.RmsAngsuran.id_tagihan = ubhRumus.RmsAngsuran.id_tagihan;
        //            ubhRumus.jlh_angsuran = dao.GetJmlAngsuran(ubhRumus.RmsAngsuran.id_rumus);
        //            ubhRumus.TagihanList = dao.GetTagihan();

        //            return View(ubhRumus);
        //        }
        //        else
        //        {
        //            if (dao.UbahRmsAngsuran(ubhRumus))
        //            {
        //                TempData["success"] = "Berhasil mengubah data rumus angsuran!";
        //            }
        //            else
        //            {
        //                TempData["error"] = "Gagal mengubah data rumus angsuran!";
        //            }

        //        }
        //        return Redirect($"~/MstAngsuran/DetailMstAngsuran?id_rumus={ ubhRumus.RmsAngsuran.id_rumus }");
        //    }
        //    else
        //    {
        //        ubhRumus.RmsAngsuran.id_detail = ubhRumus.RmsAngsuran.id_detail;
        //        ubhRumus.RmsAngsuran.id_rumus = ubhRumus.RmsAngsuran.id_rumus;
        //        ubhRumus.jlh_angsuran = dao.GetJmlAngsuran(ubhRumus.RmsAngsuran.id_rumus);
        //        ubhRumus.TagihanList = dao.GetTagihan();

        //        return View(ubhRumus);
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteRmsAngsuran(RmsAngsuran rms, int id_detail)
        //{
        //    if (dao.HapusRmsAngsuran(id_detail))
        //    {
        //        TempData["success"] = "Berhasil menghapus data rumus angsuran!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Gagal menghapus data rumus angsuran!";
        //    }

        //    return Redirect($"~/MstAngsuran/DetailMstAngsuran?id_rumus={ rms.id_rumus }");
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
