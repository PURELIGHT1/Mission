﻿using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;

namespace PMB.Controllers
{
    public class SPUController : Controller
    {
        private readonly ILogger<SPUController> _logger;
        SPUMhsDAO dao;
        PendaftarDAO pendaftarDAO = new PendaftarDAO();
        MstAngsuranDAO daoAngsuran;

        public SPUController(ILogger<SPUController> logger)
        {
            _logger = logger;
            dao = new SPUMhsDAO();
            daoAngsuran = new MstAngsuranDAO();
        }

        public JsonResult CariSPU(string kd_calon)
        {
            var data = dao.GetSPU(kd_calon);
            return Json(data);
        }

        //SPU
        public IActionResult SPUMahasiswa()
        {
            PendaftarView pendaftar = new PendaftarView();
            SPUMhsView objek = new SPUMhsView();
            objek.AllTahunAkademik = pendaftar.getAllTahunAkademik();
            objek.JalurList = pendaftarDAO.GetAllJalur();
            //objek.SPUMhsList = dao.GetAllSPU();

            return View(objek);
        }

        public IActionResult GetSPUMahasiswa(string ta, string jenjang, string jalur = null)
        {
            List<SPUMhs2> data = null;

            if (String.IsNullOrEmpty(ta))
            {
                String tahun = DateTime.Now.Year + "/" + (DateTime.Now.Year + 1);
                ta = tahun;
                if (DateTime.Now.Month > 7)
                {
                    ta = DateTime.Now.Year + 1 + "/" + (DateTime.Now.Year + 2);
                }
            }
            if (String.IsNullOrEmpty(jenjang))
            {
                jenjang = "s1";
            }
            if (String.IsNullOrEmpty(jalur))
            {
                jalur = "All";
            }
            data = dao.GetAllSPU(ta, jenjang, jalur);
            return Json(data);
        }

        public IActionResult GetDetailSPUMahasiswa(string kd_calon)
        {
            dynamic data = null;
            data = dao.GetDetailSPU(kd_calon);
            return Json(data);
        }

        public IActionResult GetJalurList()
        {
            List<Jalur> data = null;
            data = daoAngsuran.GetJalur();
            return Json(data);
        }
        public IActionResult TambahSPU()
        {
            SPUMhsView objek = new SPUMhsView();

            objek.PendaftarList = dao.GetPendaftar();

            return View(objek);
        }

        [HttpPost]
        public IActionResult CekProdiTarifSPU([FromBody] CekProdiSPUMhs data)
        {
            try
            {
                List<string> ListProdi = null;
                ListProdi = dao.CekProdiTarifSPU(data);
                if (ListProdi.Count() > 0)
                {
                    return Json(new { success = true, message = ListProdi });
                }
                else
                {
                    string lengkap = "Prodi Lengkap";
                    ListProdi.Add(lengkap);
                    return Json(new { success = false, message = ListProdi });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }


        [HttpPost]
        public IActionResult SaveDataSPU([FromBody] StoreSPUMhs data)
        {
            try
            {
                if (dao.SimpanDataSPU(data))
                {
                    TempData["success"] = "Berhasil menambah data SPU!";
                }
                else
                {
                    TempData["error"] = "Gagal menambah data SPU!";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SaveUbahSPU([FromBody] StoreSPUMhs data)
        {
            try
            {
                if (dao.SimpanDataSPU(data))
                {
                    TempData["success"] = "Berhasil mengubah data SPU!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data SPU!";
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
        public IActionResult TambahSPU(SPUMhsView spuMahasiswa)
        {
            if (ModelState.IsValid)
            {
                if (dao.SimpanSPU(spuMahasiswa))
                {
                    TempData["success"] = "Berhasil menambahkan data SPU!";
                }
                else
                {
                    TempData["error"] = "Gagal menambahkan data SPU!";
                }

                return RedirectToAction("SPUMahasiswa");
            }
            else
            {
                spuMahasiswa.PendaftarList = dao.GetPendaftar();
                return View(spuMahasiswa);
            }
        }

        public IActionResult UbahSPU(string kd_calon)
        {
            SPUMhsView objek = new SPUMhsView();
            objek.SPUCalonMhs.kd_calon = kd_calon;

            objek.SPUCalonMhs = dao.DetailSPU(kd_calon);
            objek.PendaftarList = dao.GetPendaftar();

            return View(objek);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UbahSPU(SPUMhsView spuMhs)
        {
            if (dao.UbahSPU(spuMhs))
            {
                TempData["success"] = "Berhasil mengubah data SPU!";
            }
            else
            {
                TempData["error"] = "Gagal mengubah data SPU!";
            }

            return RedirectToAction("SPUMahasiswa");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteSPU(string kd_calon)
        {
            if (dao.HapusSPU(kd_calon))
            {
                TempData["success"] = "Berhasil menghapus data SPU!";
            }
            else
            {
                TempData["error"] = "Gagal menghapus data SPU!";
            }

            return RedirectToAction("SPUMahasiswa");
        }

        [HttpPost]
        public IActionResult SimpanSplitAgs([FromBody] JadwalAngsuran jadwal)
        {
            if (dao.UbahJadwalSplitAgs(jadwal))
            {

                TempData["success"] = "Berhasil split angsuran sebanyak " + jadwal.banyak + " data!";
                return Json(new { success = true, kd_calon = jadwal.kd_calon });
            }
            else
            {
                TempData["error"] = "Gagal split angsuran sebanyak " + jadwal.banyak + " data!";
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public IActionResult SimpanUbahJadwalAgs([FromBody] JadwalAngsuran jadwal)
        {
            if (dao.UbahJadwalSplitAgs(jadwal))
            {
                TempData["success"] = "Berhasil mengubah jadwal bayar angsuran!";
                return Json(new { success = true, kd_calon = jadwal.kd_calon });
            }
            else
            {
                TempData["error"] = "Gagal mengubah jadwal bayar angsuran!";
                return Json(new { success = false });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

