using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using PMB.DAO;
using PMB.Models;
using System.Text.Json.Nodes;

namespace PMB.Controllers
{
    public class CalonMhsController : Controller
    {
        private readonly ILogger<CalonMhsController> _logger;
        CalonMhsDAO dao;
        PendaftarDAO pendaftarDAO;
        private readonly IEmailService _emailService;
        public CalonMhsController(IEmailService emailService ,ILogger<CalonMhsController> logger)
        {
            _logger = logger;
            dao = new CalonMhsDAO();
            pendaftarDAO = new PendaftarDAO();
            _emailService = emailService;
        }
        public IActionResult CalonMhs()
        {
            PendaftarView pendaftar = new PendaftarView();

            String tahun = DateTime.Now.Year + "/" + (DateTime.Now.Year + 1);
            pendaftar.AllTahunAkademik = pendaftar.getAllTahunAkademik();
            pendaftar.JalurList = pendaftarDAO.GetAllJalur();
            return View(pendaftar);
        }


        public IActionResult GetAllCalonMhs()
        {
            List<dynamic> data = null;

            data = dao.GetAllFilterCalonMhs();

            return Json(data);
        }

        public IActionResult ExportExcelCalon(string ta, string jenjang)
        {
            if (ta.Equals("All"))
            {
                String tahun = DateTime.Now.Year + "/" + (DateTime.Now.Year + 1);
                ta = tahun;
            }
            List<dynamic> data = dao.GetAllExcelCalonMhs(ta, jenjang);
            try
            {
                using (var package = new ExcelPackage())
                {
                    // Tambahkan lembar Excel
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    // Tambahkan header
                    worksheet.Cells["A1"].Value = "KD Calon";
                    worksheet.Cells["B1"].Value = "Nama Calon";
                    worksheet.Cells["C1"].Value = "Jalur";
                    worksheet.Cells["D1"].Value = "Alamat Asal";
                    worksheet.Cells["E1"].Value = "NO KK";
                    worksheet.Cells["F1"].Value = "NIK";
                    worksheet.Cells["G1"].Value = "Golongan Darah";
                    worksheet.Cells["H1"].Value = "Agama";
                    worksheet.Cells["I1"].Value = "Kewarganegaraan";
                    worksheet.Cells["J1"].Value = "Tempat Lahir";
                    worksheet.Cells["K1"].Value = "Tanggal Lahir";
                    worksheet.Cells["L1"].Value = "Jenis Kelamin";
                    worksheet.Cells["M1"].Value = "No Hp";
                    worksheet.Cells["N1"].Value = "Periode";
                    worksheet.Cells["O1"].Value = "Tahun Akademik";
                    worksheet.Cells["P1"].Value = "Nama SMA";
                    worksheet.Cells["Q1"].Value = "Pilihan 1";
                    worksheet.Cells["R1"].Value = "Pilihan 2";
                    worksheet.Cells["S1"].Value = "Pilihan 3";
                    worksheet.Cells["T1"].Value = "Prodi Diterima";

                    // Isi data
                    var row = 2;
                    foreach (var item in data)
                    {
                        worksheet.Cells[string.Format("A{0}", row)].Value = item.kd_calon;
                        worksheet.Cells[string.Format("B{0}", row)].Value = item.nm_calon;
                        worksheet.Cells[string.Format("C{0}", row)].Value = item.nama_jalur;
                        worksheet.Cells[string.Format("D{0}", row)].Value = item.alamat;
                        worksheet.Cells[string.Format("E{0}", row)].Value = item.no_kk;
                        worksheet.Cells[string.Format("F{0}", row)].Value = item.nik;
                        worksheet.Cells[string.Format("G{0}", row)].Value = item.gol_darah;
                        worksheet.Cells[string.Format("H{0}", row)].Value = item.agama;
                        worksheet.Cells[string.Format("I{0}", row)].Value = item.kwrganegaraan;
                        worksheet.Cells[string.Format("J{0}", row)].Value = item.tmp_lahir;
                        worksheet.Cells[string.Format("K{0}", row)].Value = item.tgl_lahir;
                        worksheet.Cells[string.Format("L{0}", row)].Value = item.jns_kel;
                        worksheet.Cells[string.Format("M{0}", row)].Value = item.hp_pendaftar;
                        worksheet.Cells[string.Format("N{0}", row)].Value = item.periode;
                        worksheet.Cells[string.Format("O{0}", row)].Value = item.thnakademik;
                        worksheet.Cells[string.Format("P{0}", row)].Value = item.nama_sma;
                        worksheet.Cells[string.Format("Q{0}", row)].Value = item.pil_1;
                        worksheet.Cells[string.Format("R{0}", row)].Value = item.pil_2;
                        worksheet.Cells[string.Format("S{0}", row)].Value = item.pil_3;
                        worksheet.Cells[string.Format("T{0}", row)].Value = item.prodi_diterima;
                        row++;
                    }

                    // Konversi ke byte array
                    var excelBytes = package.GetAsByteArray();

                    // Kembalikan file Excel sebagai respons HTTP
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data Calon Mahasiswa Baru TA - "+ta+".xlsx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat menghasilkan file Excel: " + ex.Message);
            }
                
        }
        public IActionResult DetailAngsuranCalonMhs(string Kd_Calon)
        {
            PendaftarView objek = new PendaftarView();
            objek.Pendaftar.Kd_Calon = Kd_Calon;

            objek.AngsuranCalonMhs = dao.GetDetailAngsuranMhs(Kd_Calon);

            return View(objek);
        }

        public IActionResult DetailCalonMhs(string Kd_Calon)
        {
            StorePendaftar2 data = dao.DetailCalonMhs2(Kd_Calon);
            return View(data);
        }

        public IActionResult GetDataDiri(string Kd_Calon)
        {
            dynamic data = null;

            data = dao.GetDataDiri(Kd_Calon);

            return Json(data);
        }

        public IActionResult GetDataSMA(string Kd_Calon)
        {
            dynamic data = null;

            data = dao.GetDataSMA(Kd_Calon);

            return Json(data);
        }

        public IActionResult GetDataProdi(string Kd_Calon)
        {
            dynamic data = null;

            data = dao.GetDataProdi(Kd_Calon);

            return Json(data);
        }

        public IActionResult GetPrestasiCalonMhs(string Kd_Calon)
        {

            List<Prestasi> data = null;
            data = dao.GetListPrestasiCalonMhs(Kd_Calon);
            return Json(data);

        }

        public IActionResult GetUTBKCalonMhs(string Kd_Calon)
        {

            List<dynamic> data = null;
            data = dao.GetNilaiUBTK(Kd_Calon);
            return Json(data);

        }

        public IActionResult GetUpdateUTBKCalonMhs(string Kd_Calon)
        {

            List<dynamic> data = null;
            data = dao.GetUpdateNilaiUBTK(Kd_Calon);
            return Json(data);

        }

        public IActionResult GetSMT5CalonMhs(string Kd_Calon)
        {

            List<dynamic> data = null;
            data = dao.GetNilaiSMT5(Kd_Calon);
            return Json(data);

        }

        public IActionResult GetDetailPrestasiCalonMhs(int id)
        {

            Prestasi data = null;
            data = dao.GetDetailPrestasiCalonMhs(id);
            return Json(data);
        }

        public IActionResult GetProvinsiList()
        {
            List<Provinsi> data = null;

            data = pendaftarDAO.GetAllProvinsi();

            return Json(data);
        }

        public IActionResult GetDetailProvinsi(string id)
        {
            Provinsi data = null;

            data = pendaftarDAO.GetDetailProvinsi(id);

            return Json(data);
        }

        public IActionResult GetDetailKabupaten(int id)
        {
            Kota data = null;

            data = pendaftarDAO.GetDetailKota(id);

            return Json(data);
        }

        public IActionResult GetJasList()
        {
            List<Jas> data = null;

            data = dao.GetAllJas();

            return Json(data);
        }

        public IActionResult GetProdiList()
        {
            List<dynamic> data = null;

            data = pendaftarDAO.GetAllProdi();

            return Json(data);
        }

        public IActionResult GetDokumenList(string Kd_calon)
        {
            List<dynamic> data = null;

            data = dao.GetListDokumenCalonMhs(Kd_calon);

            return Json(data);
        }

        public IActionResult GetDataDokumen(int id_calon_online)
        {
            dynamic data = null;

            data = dao.DataDokumenCalonMhs(id_calon_online);

            return Json(data);
        }

        public IActionResult UbahDataDiriCalonMhs(string Kd_Calon)
        {
            StorePendaftar2 data = dao.DetailCalonMhs2(Kd_Calon);
            return View(data);
        }

        [HttpPost]
        public IActionResult SaveDataDiri([FromBody] DataDiri data)
        {
            try
            {
                if (dao.UbahCalonMhs(data))
                {
                    TempData["success"] = "Berhasil mengubah data diri!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data diri!";
                }

                return Json(new { success = true, kd_calon = data.Kd_Calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        public IActionResult UbahDataSMACalonMhs(string Kd_Calon)
        {
            StorePendaftar2 data = dao.DetailCalonMhs2(Kd_Calon);
            return View(data);
        }

        [HttpPost]
        public IActionResult saveDataSMA([FromBody] DataSMA data)
        {
            try
            {
                if (dao.UbahDataSMACalonMhs(data))
                {
                    TempData["success"] = "Berhasil mengubah data SMA!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data SMA!";
                }


                return Json(new { success = true, kd_calon = data.Kd_calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        public IActionResult UbahProgramStudiCalonMhs(string Kd_Calon) { 
             StorePendaftar2 data = dao.DetailCalonMhs2(Kd_Calon);
            return View(data);
        }
        
        [HttpPost]
        public IActionResult saveDataProdi([FromBody] DataProdi data)
        {
            try
            {
                if (dao.UbahDataProdiCalonMhs(data))
                {
                    TempData["success"] = "Berhasil mengubah data prodi calon!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data prodi calon!";
                }


                return Json(new { success = true, kd_calon = data.Kd_calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }
        public IActionResult TambahPrestasiCalonMhs(int id, string Kd_Calon)
        {
            PendaftarView data = new PendaftarView();
            data.StorePendaftar = dao.DetailCalonMhs2(Kd_Calon);
            data.prestasi_id = id;
            data.ListKategori = dao.GetListKategoriCalonMhs();
            return View(data);
        }

        public IActionResult UbahPrestasiCalonMhs(int id, string Kd_Calon)
        {
            PendaftarView data = new PendaftarView();
            data.StorePendaftar = dao.DetailCalonMhs2(Kd_Calon);
            data.prestasi_id = id;
            data.ListKategori = dao.GetListKategoriCalonMhs();
            return View(data);
        }

        [HttpPost]
        public IActionResult saveDataPrestasi([FromBody] Prestasi data)
        {
            try
            {
                if (data.id_prestasi < 1)
                {
                    if (dao.TambahPrestasiCalonMhs(data))
                    {
                        TempData["success"] = "Berhasil menambah data prestasi!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal menambah data prestasi!";
                    }

                }
                else
                {
                    if (dao.UbahPrestasiCalonMhs(data))
                    {
                        TempData["success"] = "Berhasil mengubah data prestasi!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah data prestasi!";
                    }

                }

                return Json(new { success = true, kd_calon = data.kd_calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeletePrestasiCalonMhs(int id, string Kd_Calon)
        {
            try
            {
                if (dao.HapusPrestasiCalonMhs(id))
                {
                    TempData["success"] = "Berhasil menghapus data prestasi!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data prestasi!";
                }

                return Json(new { success = true, kd_calon = Kd_Calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SubmitNoteDokumen([FromBody] Verifikasi berkas)
        {
            try
            {
                StorePendaftar2 data = dao.DetailCalonMhs2(berkas.kd_calon);
                var toEmail = data.email;
                var subject = "PMB UAJY - Verfikasi Berkas";
                var messageBody = berkas.content;
                if (dao.VerifikasiBerkasCalonMhs(berkas))
                {
                    _emailService.SendEmailAsync(toEmail, subject, messageBody);
                    TempData["success"] = "Berhasil mengirim verifikasi ke email calon!";
                 }
                else
                {
                    TempData["error"] = "Gagal mengirim verifikasi ke email calon!";
                }
                return Json(new { success = true, kd_calon = data.Kd_Calon });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, kd_calon = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UbahVerifikasiKelengkapan(string kd_calon, int id)
        {
            try
            {
                if (dao.VerifikasiKelengkapanBerkasCalonMhs(kd_calon, id))
                {
                    TempData["success"] = "Berhasil mengubah kelengkapan berkas calon!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah kelengkapan berkas calon!";
                }
                return Json(new { success = true, kd_calon = kd_calon });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, kd_calon = ex.Message });
            }
        }
        
        public IActionResult UbahDokumenCalonMhs(string Kd_Calon, int Id_dokumen)
        {

            StorePendaftar2 pendaftar = dao.DetailCalonMhs2(Kd_Calon);

            UploadDokumen data = new UploadDokumen();
            data.kd_calon = pendaftar.Kd_Calon;
            data.id_calon_online = pendaftar.Id_Calon_Online;
            data.id_ref_jenis_dokumen = Id_dokumen;

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDokumen([Bind("kd_calon,id_calon_online,id_ref_jenis_dokumen,jalur,pdf")] UploadDokumen uploadDokumen, IFormFile imageFile, IFormFile pdfFile)
        {
            try
            {
                if (pdfFile != null && pdfFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await pdfFile.CopyToAsync(memoryStream);
                        uploadDokumen.pdf = memoryStream.ToArray();
                    }
                    StorePendaftar2 pendaftar = dao.DetailCalonMhs2(uploadDokumen.kd_calon);
                    uploadDokumen.jalur = pendaftar.Kd_Jalur;

                    if (dao.UbahDokumenCalonMhs(uploadDokumen))
                    {
                        TempData["success"] = "Berhasil mengubah dokumen calon!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah dokumen calon!";
                    }
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        uploadDokumen.pdf = memoryStream.ToArray();
                    }
                    StorePendaftar2 pendaftar = dao.DetailCalonMhs2(uploadDokumen.kd_calon);
                    uploadDokumen.jalur = pendaftar.Kd_Jalur;

                    if (dao.UbahDokumenCalonMhs(uploadDokumen))
                    {
                        TempData["success"] = "Berhasil mengubah dokumen calon!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengubah dokumen calon!";
                    }
                }



                return Redirect($"~/CalonMhs/DetailCalonMhs?Kd_Calon={uploadDokumen.kd_calon}");
            }
            catch (Exception ex)
            {

                return Redirect($"~/CalonMhs/UbahDokumenCalonMhs?Kd_Calon={uploadDokumen.kd_calon}&&Id_dokumen={uploadDokumen.id_ref_jenis_dokumen}");
            }
        }

        //public IActionResult UbahNilaiCalonMhs([FromBody] JsonObject data)
        //{
        //    try
        //    {
        //        List<Nilai> ubah = new List<Nilai>();

        //        for (int i = 0; i < 2; i++)
        //        {
        //            string kd_calon = data["kelas10"][i]["calonMhs"].ToString();
        //            string kelas = data["kelas10"][i]["kelas"].ToString();
        //            string rangking = "";
        //            if(data["kelas10"][i]["rank"] != null)
        //            {
        //                rangking = data["kelas10"][i]["rank"].ToString();
        //            }
        //            string semester = data["kelas10"][i]["semester"].ToString();
        //            decimal matematika = 0;
        //            decimal kkm_matematika = 0;
        //            decimal bhs_inggris = 0;
        //            decimal kkm_inggris = 0;
        //            decimal bahasa = 0;
        //            decimal kkm_indonesia = 0;
        //            if (data["kelas10"][i]["mm"] != null)
        //            {
        //                matematika = (decimal)data["kelas10"][i]["mm"];
        //            }
        //            if (data["kelas10"][i]["kk_mm"] != null)
        //            {
        //                kkm_matematika = (decimal)data["kelas10"][i]["kk_mm"];
        //            }
        //            if (data["kelas10"][i]["bing"] != null)
        //            {
        //                bhs_inggris = (decimal)data["kelas10"][i]["bing"];
        //            }
        //            if (data["kelas10"][i]["kkm_bing"] != null)
        //            {
        //                kkm_inggris = (decimal)data["kelas10"][i]["kkm_bing"];
        //            }
        //            if (data["kelas10"][i]["id"] != null)
        //            {
        //                bahasa = (decimal)data["kelas10"][i]["id"];
        //            }
        //            if (data["kelas10"][i]["kkm_id"] != null)
        //            {
        //                kkm_indonesia = (decimal)data["kelas10"][i]["kkm_id"];
        //            }
        //            //decimal kkm_matematika = (decimal)data["kelas10"][i]["kk_mm"];
        //            //decimal bhs_inggris = (decimal)data["kelas10"][i]["bing"];
        //            //decimal kkm_inggris = (decimal)data["kelas10"][i]["kkm_bing"];
        //            //decimal bahasa = (decimal)data["kelas10"][i]["id"];
        //            //decimal kkm_indonesia = (decimal)data["kelas10"][i]["kkm_id"];
        //            int id_prestasi = 0;
        //            if ((int)data["kelas10"][i]["prestasi"] != 0 || (int)data["kelas10"][i]["prestasi"] != null)
        //            {
        //                id_prestasi = (int)data["kelas10"][i]["prestasi"];
        //            }
        //            Nilai dataNilai = new Nilai();
        //            dataNilai.id_prestasi = id_prestasi;
        //            dataNilai.kd_calon = kd_calon;
        //            dataNilai.kelas = kelas;
        //            dataNilai.rangking = rangking;
        //            dataNilai.semester = semester;
        //            dataNilai.matematika = matematika;
        //            dataNilai.kkm_matematika = kkm_matematika;
        //            dataNilai.bhs_inggris = bhs_inggris;
        //            dataNilai.kkm_inggris = kkm_inggris;
        //            dataNilai.bahasa = bahasa;
        //            dataNilai.kkm_indonesia = kkm_indonesia;

        //            ubah.Add(dataNilai);
        //        }

        //        for (int j = 0; j < 2; j++)
        //        {
        //            int id_prestasi = (int) data["kelas11"][j]["prestasi"];
        //            string kd_calon = data["kelas11"][j]["calonMhs"].ToString();
        //            string kelas = data["kelas11"][j]["kelas"].ToString();
        //            string rangking = data["kelas11"][j]["rank"].ToString();
        //            string semester = data["kelas11"][j]["semester"].ToString();
        //            decimal matematika = (decimal)data["kelas11"][j]["mm"];
        //            decimal kkm_matematika = (decimal)data["kelas11"][j]["kk_mm"];
        //            decimal bhs_inggris = (decimal)data["kelas11"][j]["bing"];
        //            decimal kkm_inggris = (decimal)data["kelas11"][j]["kkm_bing"];
        //            decimal bahasa = (decimal)data["kelas11"][j]["id"];
        //            decimal kkm_indonesia = (decimal)data["kelas11"][j]["kkm_id"];

        //            Nilai dataNilai = new Nilai();
        //            dataNilai.id_prestasi = id_prestasi;
        //            dataNilai.kd_calon = kd_calon;
        //            dataNilai.kelas = kelas;
        //            dataNilai.rangking = rangking;
        //            dataNilai.semester = semester;
        //            dataNilai.matematika = matematika;
        //            dataNilai.kkm_matematika = kkm_matematika;
        //            dataNilai.bhs_inggris = bhs_inggris;
        //            dataNilai.kkm_inggris = kkm_inggris;
        //            dataNilai.bahasa = bahasa;
        //            dataNilai.kkm_indonesia = kkm_indonesia;

        //            ubah.Add(dataNilai);
        //        }

        //        if (dao.UbahNilaiCalonMhs(ubah))

        //        {
        //            TempData["success"] = "Berhasil mengubah data nilai mahasiswa!";
        //        }
        //        else
        //        {
        //            TempData["error"] = "Gagal mengubah data nilai mahasiswa!";
        //        }

        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }

        //}

        [HttpPost]
        public IActionResult UbahNilaiUTBK([FromBody] List<UTBK> data)
        {
            try
            {
                if (dao.UbahUTBKCalonMhs(data))
                {
                    TempData["success"] = "Berhasil mengubah data nilai!";
                }
                else
                {
                    TempData["error"] = "Gagal mengubah data nilai!";
                }

                return Json(new { success = true, kd_calon = data[0].Kd_calon });
            }
            catch (Exception ex)
            {
                return StatusCode(500, TempData["error"] = ex.Message);
            }

        }

        public IActionResult GenerateAngsuran(AngsuranMhs angsuranMhs)
        {
            if (dao.GenerateAngsuran(angsuranMhs) == 1)
            {
                TempData["success"] = $"Berhasil generate angsuran { angsuranMhs.Nm_Calon }!";
                return Redirect($"~/CalonMhs/DetailAngsuranCalonMhs?Kd_Calon={ angsuranMhs.Kd_Calon }&&Nm_Calon={ angsuranMhs.Nm_Calon }");
            }
            else if (dao.GenerateAngsuran(angsuranMhs) == 2)
            {
                TempData["error"] = $"Data {angsuranMhs.Nm_Calon} sudah tergenerate!";
                return Redirect($"~/CalonMhs/DetailAngsuranCalonMhs?Kd_Calon={angsuranMhs.Kd_Calon}&&Nm_Calon={angsuranMhs.Nm_Calon}");
            }
            else if(dao.GenerateAngsuran(angsuranMhs) == 3)
            {
                TempData["warning"] = $"Tidak ada angsuran bagi { angsuranMhs.Nm_Calon }!";
                return Redirect($"~/CalonMhs/DetailAngsuranCalonMhs?Kd_Calon={ angsuranMhs.Kd_Calon }&&Nm_Calon={ angsuranMhs.Nm_Calon }");
            }
            else
            {
                TempData["error"] = $"Gagal generate angsuran { angsuranMhs.Nm_Calon }!";
                return Redirect($"~/CalonMhs/DetailAngsuranCalonMhs?Kd_Calon={ angsuranMhs.Kd_Calon }&&Nm_Calon={ angsuranMhs.Nm_Calon }");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteAngsuran(AngsuranMhs angsuranMhs)
        {
            string tagihBank = dao.GetIsTagihBank(angsuranMhs.Kd_Calon);
            string catatanBayar = dao.GetCatatanBayar(angsuranMhs.Kd_Calon);

            if (tagihBank == "1" && (catatanBayar != null || catatanBayar != ""))
            {
                TempData["error"] = "Gagal menghapus data calon mahasiswa!";
            }
            else
            {
                if (dao.HapusAngsuran(angsuranMhs.Kd_Calon))
                {
                    TempData["success"] = "Berhasil menghapus data calon mahasiswa!";
                }
                else
                {
                    TempData["error"] = "Gagal menghapus data calon mahasiswa!";
                }
            }

            return Redirect($"~/CalonMhs/DetailAngsuranCalonMhs?Kd_Calon={ angsuranMhs.Kd_Calon }&&Nm_Calon={ angsuranMhs.Nm_Calon }");
        }
    }

    internal class ApplicationDbContext
    {
    }
}
