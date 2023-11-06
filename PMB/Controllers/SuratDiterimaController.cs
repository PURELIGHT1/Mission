using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PMB.DAO;
using PMB.Models;

namespace PMB.Controllers
{
    public class SuratDiterimaController : Controller
    {
        private readonly ILogger<SuratDiterimaController> _logger;
        SuratDiterimaDAO dao;
        MstAngsuranDAO mstAgsDAO;

        public SuratDiterimaController(ILogger<SuratDiterimaController> logger)
        {
            _logger = logger;
            dao = new SuratDiterimaDAO();
            mstAgsDAO = new MstAngsuranDAO();
        }

        public IActionResult SuratDiterima()
        {
            SuratDiterimaView data = new SuratDiterimaView();
            data.KetDiterimaList = dao.GetListRegis();

            return View(data);
        }

        public IActionResult TambahJadwalRegis()
        {
            SuratDiterimaView data = new SuratDiterimaView();
            
            data.JalurList = dao.GetJalur();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TambahJadwalRegis(SuratDiterimaView suratDiterima)
        {
            if (ModelState.IsValid)
            {
                if(dao.CekData(suratDiterima.KetDiterima.Kd_Jalur, suratDiterima.KetDiterima.Periode) != null)
                {
                    string jalur = mstAgsDAO.GetNamaJalur(suratDiterima.KetDiterima.Kd_Jalur);
                    TempData["error"] = $"Data surat jalur {jalur} periode ke-{suratDiterima.KetDiterima.Periode} sudah ada!";
                    suratDiterima.JalurList = dao.GetJalur();

                    return View(suratDiterima);
                }
                else
                {
                    if (dao.Insert(suratDiterima.KetDiterima))
                    {
                        TempData["success"] = "Berhasil menambahkan data surat!";

                        return RedirectToAction("SuratDiterima");
                    }
                    else
                    {
                        TempData["error"] = "Gagal menambahkan data surat!";
                        suratDiterima.JalurList = dao.GetJalur();

                        return View(suratDiterima);
                    }
                }
            }
            else
            {
                TempData["error"] = "Masih terdapat kesalahan!";
                suratDiterima.JalurList = dao.GetJalur();

                return View(suratDiterima);
            }
        }

        public IActionResult UbahJadwalRegis(string Kd_Jalur, string Periode)
        {
            SuratDiterimaView data = new SuratDiterimaView();
            data.KetDiterima.Kd_Jalur = Kd_Jalur;
            data.KetDiterima.Periode = Periode;

            data.KetDiterima = dao.DetailSurat(Kd_Jalur, Periode);

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UbahJadwalRegis(SuratDiterimaView suratDiterima)
        {
            if (dao.Update(suratDiterima.KetDiterima))
            {
                TempData["success"] = "Berhasil mengubah data surat!";

                return RedirectToAction("SuratDiterima");
            }
            else
            {
                TempData["error"] = "Gagal mengubah data surat!";

                return Redirect($"~/SuratDiterima/UbahJadwalRegis?Kd_Jalur={ suratDiterima.KetDiterima.Kd_Jalur }&&Periode={ suratDiterima.KetDiterima.Periode }");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteJadwalRegis(string Kd_Jalur, string Periode)
        {
            if (dao.Delete(Kd_Jalur, Periode))
            {
                TempData["success"] = "Berhasil menghapus data SPU!";
            }
            else
            {
                TempData["error"] = "Gagal menghapus data SPU!";
            }

            return RedirectToAction("SuratDiterima");
        }
    }
}
