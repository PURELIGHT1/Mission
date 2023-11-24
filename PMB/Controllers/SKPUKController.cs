using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using PMB.DAO;
using PMB.Models;
using Rotativa.AspNetCore;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Dynamic;

namespace PMB.Controllers
{
    public class SKPUKController : Controller
    {
        private readonly ILogger<SKPUKController> _logger;
        SKPUKMhsDAO dao;
        
        public SKPUKController(ILogger<SKPUKController> logger)
        {
            _logger = logger;
            dao = new SKPUKMhsDAO();
        }

        //SKPUK
        public IActionResult SKPUK()
        {
            SKPUKView objek = new SKPUKView();
            objek.SKPUKList = dao.GetAllSKPUK();

            return View(objek);
        }

        public IActionResult TambahSKPUK()
        {
            SKPUKView objek = new SKPUKView();
            objek.SKPUKList = dao.GetAllSKPUK();
            objek.ProdiList = dao.GetProdi();
            objek.TagihanList = dao.GetTagihan();

            return View(objek);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TambahSKPUK(SKPUKView dataSKPUK)
        {
            if (ModelState.IsValid)
            {
                if (dao.DetailSKPUK(dataSKPUK.SKPUK.id_skpu) != null)
                {
                    string nama_prodi = dao.GetNamaProdi(dataSKPUK.SKPUK.id_prodi);
                    TempData["error"] = $"Data SKPUK dengan prodi { nama_prodi } pada tahun akademik { dataSKPUK.SKPUK.thnakademik } sudah ada!";
                    
                    dataSKPUK.ProdiList = dao.GetProdi();
                    dataSKPUK.TagihanList = dao.GetTagihan();
                    return View(dataSKPUK);
                }
                else
                {
                    if (dao.SimpanSKPUK(dataSKPUK))
                    {
                        TempData["success"] = "Berhasil menambahkan data SKPUK!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal menambahkan data SKPUK!";
                    }

                    return RedirectToAction("SKPUK");
                }
            }
            else
            {
                dataSKPUK.ProdiList = dao.GetProdi();
                dataSKPUK.TagihanList = dao.GetTagihan();
                
                return View(dataSKPUK);
            }
        }

        public IActionResult UbahSKPUK(int id_skpu)
        {
            SKPUKView objek = new SKPUKView();

            objek.SKPUK = dao.DetailSKPUK(id_skpu);
            objek.ProdiList = dao.GetProdi();
            objek.TagihanList = dao.GetTagihan();

            return View(objek);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UbahSKPUK(SKPUKView dataSKPUK)
        {
            if (dao.UbahSKPUK(dataSKPUK))
            {
                TempData["success"] = "Berhasil mengubah data SKPUK!";
            }
            else
            {
                TempData["error"] = "Gagal mengubah data SKPUK!";
            }

            return RedirectToAction("SKPUK");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSKPUK(string id_prodi, string thnakademik, int id_tagihan)
        {
            if (dao.HapusSKPUK(id_prodi, thnakademik, id_tagihan))
            {
                TempData["success"] = "Berhasil menghapus data SKPUK!";
            }
            else
            {
                TempData["error"] = "Gagal menghapus data SKPUK!";
            }

            return RedirectToAction("SKPUK");
        }

        public IActionResult CetakSKPUK(string kd_calon)
        {
            string halaman = "Cetak";

            //List<AngsuranSKPUK> ListDataAngsuran = dao.GetAngsuranSKPUK(kd_calon);
            CetakSKPUKView data = new CetakSKPUKView();

            data.JenisPembayaran = dao.GetBayarSKPUK(kd_calon);
            data.DataMhs = dao.GetMahasiswaCetakSKPUK(kd_calon);
            data.Pejabat = dao.GetTTDRektor();
            int total = 0;
            int jml = 0;
            int jmlPotongan = 0;
            for (int i = 0;  i < data.JenisPembayaran.Count(); i++)
            {
                total = total + data.JenisPembayaran[i].jumlah;
                
            }

            int uang_angsuran1 = 0;
            for (int i = 0; i < data.JenisPembayaran.Count(); i++)
            {
                if (data.JenisPembayaran[i].ket_angsuran.Contains("SPU"))
                {
                    data.DataMhs.jaminan = data.JenisPembayaran[i].is_jaminan;
                    data.DataMhs.tanggal_jaminan = data.JenisPembayaran[i].batas_waktu;
                    break;
                }
            }
            
            data.DataMhs.jml_sebelum_potongan = total;

            data.Potongan = dao.GetPotonganSKPUK(kd_calon);
            for (int k = 0; k < data.Potongan.Count(); k++)
            {
                jmlPotongan = jmlPotongan + data.Potongan[k].jlh_total;
            }
            data.DataMhs.jml_potongan = jmlPotongan;
            data.DataMhs.jml_setelah_potongan = total - jmlPotongan;
            data.ListAngsuranMhs = dao.GetAngsuranSKPUK(kd_calon);
            if(data.ListAngsuranMhs != null)
            {
                for (int j = 0; j < data.ListAngsuranMhs.Count(); j++)
                {
                    jml = jml + data.ListAngsuranMhs[j].jmluang;
                }

                if (data.DataMhs.jaminan)
                {

                    uang_angsuran1 = uang_angsuran1 + data.ListAngsuranMhs[0].jmluang;
                    foreach(var item in data.ListAngsuranMhs)
                    {
                        item.angsuranke = item.angsuranke - 1;
                    }
                    data.ListAngsuranMhs.RemoveAt(0);
                }
            }

            data.DataMhs.uang_jaminan = uang_angsuran1;
            data.DataMhs.jml_angsuran = jml;
            if (data.DataMhs.jaminan)
            {
                data.DataMhs.jml_angsuran = jml - uang_angsuran1;
            }

            string terbilangJmlStlhPotongan = SKPUKMhsDAO.Terbilang(data.DataMhs.jml_setelah_potongan);
            data.DataMhs.terbilangJmlStlhPotongan = terbilangJmlStlhPotongan;

            return new ViewAsPdf(halaman, data)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = {
                         Left = 20,
                         Right = 20,
                         Top = 15,
                         Bottom = 10
                 },
                //FileName = $"Surat Ketetapan Uang Kuliah {data.DataMhs.nm_calon}.pdf"
            };
        }

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

