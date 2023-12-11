using Dapper;
using PMB.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class SKPUKMhsDAO
    {
        //Tabel SKPU
        public List<SKPUK> GetAllSKPUK()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT S.[ID_SKPU], S.[ID_PRODI], P.[NM_PRODI] AS NAMA_PRODI, S.[THNAKADEMIK], S.[ID_TAGIHAN], T.[nama_tagihan], S.[NOMINAL]
                                        FROM [dbo].[TBL_SKPU] S LEFT JOIN [dbo].[REF_PRODI] P
                                        ON S.[ID_PRODI] = P.[ID_PRODI]
                                        LEFT JOIN [dbo].[REF_TAGIHAN] T
                                        ON S.[ID_TAGIHAN] = T.[id_tagihan]
								     ORDER BY S.[THNAKADEMIK] DESC, P.[ID_PRODI] ASC";

                    var data = conn.Query<SKPUK>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public CetakSKPUKMhs GetMahasiswaCetakSKPUK(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                      a.kd_calon,
									  b.jenjang,
                                      a.thnakademik,
                                      a.kd_jalur,
                                      b.nama_jalur,
                                      a.nm_calon,
                                      d.nm_fakultas,
                                      c.nm_prodi,
									  a.kwrganegaraan,
                                      FORMAT(e.registrasi_selesai,'dd MMMM yyyy','id-id') as ket_tgl_regis,
									  FORMAT(f.tgl_cetak,'dd MMMM yyyy','id-id') as tgl_cetak
                                    FROM mhs_pendaftar a  
                                    left outer join ref_jalur b ON a.kd_jalur = b.kd_jalur
                                    left outer join ref_prodi c ON a.masuk = c.id_prodi
                                    left outer join ref_fakultas d ON c.id_fakultas = d.id_fakultas
                                    inner join tbl_jadwal_jalur e ON a.kd_jalur = e.kd_jalur
                                    left outer join spu f ON a.kd_calon = f.kd_calon
                                    WHERE a.kd_calon = @kd_calon";

                    var data = conn.QueryFirstOrDefault<CetakSKPUKMhs>(query, new { kd_calon = id });

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public bool SimpanSKPUK(SKPUKView simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"INSERT INTO [dbo].[TBL_SKPU]
	                                    ([ID_PRODI]
	                                    ,[THNAKADEMIK]
	                                    ,[ID_TAGIHAN]
	                                    ,[NOMINAL])
                                    VALUES (@id_prodi, @thnakademik, @id_tagihan, @nominal)";

                    var data = conn.Execute(query, simpan.SKPUK);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public SKPUK DetailSKPUK(int id_skpu)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) S.[ID_SKPU],
                                        S.[ID_PRODI],
                                        P.[NM_PRODI] AS NAMA_PRODI,
                                        S.[THNAKADEMIK],
                                        S.[ID_TAGIHAN],
                                        T.[nama_tagihan],
                                        S.[NOMINAL]
                                     FROM [dbo].[TBL_SKPU] S FULL JOIN [dbo].[REF_PRODI] P
                                     ON S.[ID_PRODI] = P.[ID_PRODI]
                                     FULL JOIN [dbo].[REF_TAGIHAN] T
                                     ON S.[ID_TAGIHAN] = T.[id_tagihan]
                                     WHERE S.[ID_SKPU] = @id_skpu";

                    var param = new { id_skpu = id_skpu };
                    var data = conn.QueryFirstOrDefault<SKPUK>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool UbahSKPUK(SKPUKView ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE [dbo].[TBL_SKPU] SET
                                          [ID_PRODI] = @id_prodi,
                                          [ID_TAGIHAN] = @id_tagihan,
                                          [NOMINAL] = @nominal
                                    WHERE [ID_SKPU] = @id_skpu";

                    var data = conn.Execute(query, ubah.SKPUK);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool HapusSKPUK(string id_prodi, string thnakademik, int id_tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[TBL_SKPU] WHERE ID_PRODI = @id_prodi AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN = @id_tagihan";

                    var data = conn.Execute(query, new { id_prodi = id_prodi, thnakademik = thnakademik, id_tagihan = id_tagihan });

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        //Tabel Ref Prodi -> Prodi apa saja yang ada
        public List<Prodi> GetProdi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT *, [NM_PRODI] as nama_prodi
                                     FROM [dbo].[REF_PRODI]
                                     WHERE [ID_PRODI] != 0";

                    var data = conn.Query<Prodi>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        //Tabel Ref Tagihan -> Tagihan apa saja yang ada
        public List<Tagihan> GetTagihan()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [id_tagihan], [nama_tagihan]
                                     FROM [Mission].[dbo].[REF_TAGIHAN]";

                    var data = conn.Query<Tagihan>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public string GetNamaProdi(string id_prodi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT P.[NM_PRODI]
                                      FROM [dbo].[TBL_SKPU] S JOIN [dbo].[REF_PRODI] P
                                      ON S.[ID_PRODI] = P.[ID_PRODI]
                                      WHERE S.[ID_PRODI] = @id_prodi";

                    var data = conn.QueryFirstOrDefault<string>(query, new { id_prodi = id_prodi });

                    return data;
                }
                catch (Exception e)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public dynamic GetTTDRektor()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"select top(1) nama, foto, jabatan from ref_pejabat;";
                    var data = conn.QueryFirstOrDefault<dynamic>(query);
                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public List<PembayaranSKPUK> GetBayarSKPUK(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT ANGSURAN_MHS.kd_calon, SUM(ANGSURAN_MHS.JMLUANG) AS jumlah,  SUM(ANGSURAN_MHS.JUMLAH) AS total, 
 SUM(ANGSURAN_MHS.potongan) AS potongan, 
ANGSURAN_MHS.ID_REF_TAGIHAN, ref_tagihan.nama_tagihan ket_angsuran, ref_tagihan.id_tagihan,
FORMAT(MAX(batas_waktu),'dd MMMM yyyy','id-id') batas_waktu,
CONCAT(FORMAT(MIN(tgl_buka),'dd MMMM yyyy','id-id'),' - ', FORMAT(MAX(batas_waktu),'dd MMMM yyyy','id-id')) as batas_bayar, max(cast(is_jaminan as int)) is_jaminan
FROM ANGSURAN_MHS INNER JOIN
 ref_tagihan ON ANGSURAN_MHS.ID_REF_TAGIHAN = ref_tagihan.id_tagihan
GROUP BY ANGSURAN_MHS.kd_calon, ANGSURAN_MHS.ID_REF_TAGIHAN, ref_tagihan.nama_tagihan, ref_tagihan.id_tagihan 
HAVING (ANGSURAN_MHS.kd_calon = @kd_calon)
ORDER BY ref_tagihan.id_tagihan ";

                    var data = conn.Query<PembayaranSKPUK>(query, new { kd_calon = id }).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        //public List<PembayaranSKPUK> GetBayarSKPUK(string id)
        //{
        //    using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
        //    {
        //        try
        //        {
        //            List<PembayaranSKPUK> dataSKPUK = new List<PembayaranSKPUK>();
        //            string ambil = @"select top(1) thnakademik, kd_jalur 
        //                            from mhs_pendaftar 
        //                            where kd_calon = @kd_calon";
        //            MhsPendaftar2 data1 = conn.QueryFirstOrDefault<MhsPendaftar2>(ambil, new { kd_calon = id });

        //            string ambilIdRumus = @"select top(1) id_rumus 
        //                                    from mst_angsuran 
        //                                    where thnakademik = @ta and kd_jalur = @jalur";
        //            var data2 = conn.QueryFirstOrDefault<int>(ambilIdRumus, new { ta = data1.thnakademik, jalur =  data1.kd_jalur });

        //            string queryTagihan = @"select distinct b.nama_tagihan 
        //                                    from detail_rumus_angsuran a join ref_tagihan b ON a.id_tagihan = b.id_tagihan 
        //                                    where id_rumus = @id_rumus";

        //            List<string> data3 = conn.Query<string>(queryTagihan, new { id_rumus = data2 }).AsList();
        //            if (data3.Count() > 0)
        //            {
        //                for(int i = 0; i < data3.Count(); i++)
        //                {
        //                    string query = @"select top(1)
        //                                        is_jaminan,
        //                                        case 
	       //                                         when ket_angsuran like '%SPU%' then (select sum(jumlah) from angsuran_mhs where kd_calon = @kd_calon and ket_angsuran like '%SPU%')
	       //                                         else jumlah
	       //                                     end as jumlah, 
        //                                        ket_angsuran, 
        //                                        tgl_buka, 
        //                                        FORMAT(batas_waktu,'dd MMMM yyyy','id-id') batas_waktu
        //                                    from angsuran_mhs
        //                                    where ket_angsuran LIKE '%" + data3[i] + "%' and kd_calon = @kd_calon and sks is not null;";

        //                    var data = conn.QueryFirstOrDefault<PembayaranSKPUK>(query, new { kd_calon = id });

        //                    if(data != null)
        //                    {
        //                        dataSKPUK.Add(data);
        //                    }
        //                    continue;
        //                }
        //            }
        //            return dataSKPUK;

        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            conn.Dispose();
        //        }
        //    }
        //}

        public List<AngsuranSKPUK> GetAngsuranSKPUK(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    List<AngsuranSKPUK> data = new List<AngsuranSKPUK>();
                    string query1 = @"select distinct angsuranke from angsuran_mhs where kd_calon = @kd_calon order by angsuranke asc;";

                    var angsuranKe = conn.Query<int>(query1, new { kd_calon = id }).AsList();

                    if(angsuranKe.Count() > 0)
                    {
                        for (int i = 0; i < angsuranKe.Count(); i++)
                        {
                            AngsuranSKPUK dataAngsuran = new AngsuranSKPUK();
                            dataAngsuran.angsuranke = angsuranKe[i];
                            string ket_angsuran = "";
                            string query2 = @"select sum(jmluang) as total from angsuran_mhs where kd_calon = @kd_calon and angsuranke = @angsuranke and sks is not null";

                            var total = conn.QueryFirstOrDefault<int>(query2, new { kd_calon = id, angsuranke = angsuranKe[i] });
                            dataAngsuran.jmluang = total;

                            string query3 = @"select ket_angsuran from angsuran_mhs where kd_calon = @kd_calon and angsuranke = @angsuranke and sks is not null";

                            var dataKetAngsuran = conn.Query<string>(query3, new { kd_calon = id, angsuranke = angsuranKe[i] }).AsList();

                            for(int j = 0; j < dataKetAngsuran.Count(); j++)
                            {
                                if(ket_angsuran == "")
                                {
                                    ket_angsuran = dataKetAngsuran[j];
                                }
                                else
                                {
                                    ket_angsuran = ket_angsuran + " + " + dataKetAngsuran[j];
                                }
                                continue;
                            }
                            dataAngsuran.ket_angsuran = ket_angsuran;

                            string query4 = @"select top(1) 
                                                CONCAT(FORMAT(tgl_buka,'dd MMMM yyyy','id-id'),' - ',FORMAT(batas_waktu,'dd MMMM yyyy','id-id')) as batas_bayar
                                            from angsuran_mhs 
                                            where kd_calon = @kd_calon and angsuranke = @angsuranke and sks is not null";
                            var batas_bayar = conn.QueryFirstOrDefault<string>(query4, new { kd_calon = id, angsuranke = angsuranKe[i] });

                            dataAngsuran.batas_bayar = batas_bayar;

                            if(dataAngsuran != null)
                            {
                                data.Add(dataAngsuran);
                            }
                        }
                    }
                    return data;

                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<PotonganSKPUK> GetPotonganSKPUK(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT 
                                        a.jns_potongan, 
                                        CASE	
	                                        WHEN a.id_tagihan is not null THEN (select nama_tagihan from ref_tagihan where id_tagihan = a.id_tagihan)
	                                        ELSE a.jenis
                                        END as jenis_potongan,
                                        a.jlh_total
                                    FROM potongan a
                                    WHERE kd_calon = @kd_calon";
                    var data = conn.Query<PotonganSKPUK>(query, new { kd_calon = kd_calon }).AsList();
                    return data;

                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<string> GetKodeCalonSKPUK(StoreSPUMhs mhs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT distinct mhs.kd_calon
                                    FROM mhs_pendaftar mhs
                                    INNER JOIN angsuran_mhs ags ON mhs.kd_calon = ags.kd_calon
                                    INNER JOIN tr_tarif trf ON mhs.masuk = trf.id_prodi AND mhs.th_masuk = trf.thmasuk
                                    WHERE (ags.kd_calon between @calon1 and @calon2)
                                    AND kd_jalur = @kd_jalur AND masuk not in ('', 00) AND trf.iscurrent = '1' ";

                    var data = conn.Query<string>(query, new
                    {
                        calon1 = mhs.kode_calon_awal,
                        calon2 = mhs.kode_calon_akhir,
                        kd_jalur = mhs.kd_jalur
                    }).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public CetakSKPUK GetCetakSKPUK(string kd_calon, string thnakademik, string masuk, string periode, string kd_jalur)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT MHS.[THNAKADEMIK], MHS.[KD_JALUR], JLR.[nama_jalur], MHS.[NM_CALON], MHS.[KD_CALON], MHS.[MASUK],
		                                       FAK.[NM_FAKULTAS], PRD.[NM_PRODI],

											   (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'SPU')) AS SPU,
											   (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'SPP Tetap')) AS SPPTETAP,
											   (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'SPP Variabel')) AS SPPVARIABEL,
		                                       (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'Buku')) AS BUKU,
											   (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'Courseware')) AS COURSEWARE,
											   (SELECT NOMINAL FROM TBL_SKPU WHERE ID_PRODI = @masuk AND THNAKADEMIK = @thnakademik AND ID_TAGIHAN IN
													(SELECT ID_TAGIHAN FROM REF_TAGIHAN WHERE nama_tagihan = 'Kemahasiswaan')) AS KEMAHASISWAAN,
											   
                                               (SELECT JLH_TOTAL FROM POTONGAN WHERE KD_CALON = @kd_calon AND JNS_POTONGAN IN
				                                    (SELECT JNS_POTONGAN FROM REF_POTONGAN WHERE nama_potongan = 'Unggulan')) AS potUnggulan,
		                                       (SELECT JLH_TOTAL FROM POTONGAN WHERE KD_CALON = @kd_calon AND JNS_POTONGAN IN
				                                    (SELECT JNS_POTONGAN FROM REF_POTONGAN WHERE nama_potongan = 'Keluarga ALumni')) AS potKeluarga,
		                                       (SELECT JLH_TOTAL FROM POTONGAN WHERE KD_CALON = @kd_calon AND JNS_POTONGAN IN
				                                    (SELECT JNS_POTONGAN FROM REF_POTONGAN WHERE nama_potongan = 'Prestasi')) AS potPrestasi,
		                                       (SELECT JLH_TOTAL FROM POTONGAN WHERE KD_CALON = @kd_calon AND JNS_POTONGAN IN
				                                    (SELECT JNS_POTONGAN FROM REF_POTONGAN WHERE nama_potongan = 'Keringanan')) AS potKeringananSPU,
		                                       (SELECT JLH_TOTAL FROM POTONGAN WHERE KD_CALON = @kd_calon AND JNS_POTONGAN IN
				                                    (SELECT JNS_POTONGAN FROM REF_POTONGAN WHERE nama_potongan = 'Konsesi')) AS potKonsesi,
											   (SELECT tgl_kelompok_1 FROM suratDiterima WHERE periode = @periode AND kd_jalur = @kd_jalur) AS KETERANGAN

	                                    FROM [dbo].[MHS_PENDAFTAR] MHS
	                                    LEFT JOIN [dbo].[POTONGAN] PTG ON MHS.[KD_CALON] = PTG.[KD_CALON]
	                                    LEFT JOIN [dbo].[TBL_SKPU] SKPUK ON MHS.[MASUK] = SKPUK.[ID_PRODI]
	                                    LEFT JOIN [dbo].[SPU] SPU ON MHS.[KD_CALON] = SPU.[KD_CALON]
	                                    LEFT JOIN [dbo].[REF_JALUR] JLR ON MHS.[KD_JALUR] = JLR.[kd_jalur]
	                                    LEFT JOIN [dbo].[REF_PRODI] PRD ON MHS.[MASUK] = PRD.[ID_PRODI]
	                                    LEFT JOIN [dbo].[REF_FAKULTAS] FAK ON PRD.[ID_FAKULTAS] = FAK.[ID_FAKULTAS]
	                                    LEFT JOIN [dbo].[REF_POTONGAN] RPTG ON PTG.[JNS_POTONGAN] = RPTG.[JNS_POTONGAN]
										LEFT JOIN [dbo].[suratDiterima] SURAT ON JLR.[kd_jalur] = SURAT.[kd_jalur]
	                                    WHERE MHS.[KD_CALON] = @kd_calon AND MHS.[MASUK] = @masuk AND SKPUK.[THNAKADEMIK] = @thnakademik";

                    var param = new { kd_calon = kd_calon, thnakademik = thnakademik, masuk = masuk, periode = periode, kd_jalur = kd_jalur };
                    var data = conn.QueryFirstOrDefault<CetakSKPUK>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool InsertUpdateTglCetak(SPUMhs simpanUbah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"IF(NOT EXISTS(SELECT [TGL_CETAK] FROM [dbo].[SPU] WHERE [KD_CALON] = @kd_calon))
	                                    INSERT
		                                    INTO [dbo].[SPU] ([KD_CALON],[TGL_CETAK])
	                                    VALUES (@kd_calon,GETDATE())
                                     ELSE
	                                    UPDATE [dbo].[SPU] 
		                                    SET [TGL_CETAK] = GETDATE()
	                                    WHERE [KD_CALON] = @kd_calon";

                    var data = conn.Execute(query, simpanUbah);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<CetakSKPUK> GetListCetakAngsuran(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DECLARE @t TABLE
                                        (
                                            ANGSURANKE INT,
                                            KET VARCHAR(100),
	                                        JMLUANG NUMERIC(18,0),
	                                        TGL_BUKA DATETIME,
	                                        BATAS_WAKTU DATETIME
                                        )
                                        INSERT INTO @t (ANGSURANKE, KET, JMLUANG, TGL_BUKA, BATAS_WAKTU)
	                                        (SELECT [ANGSURANKE], [KET_ANGSURAN], [JMLUANG], [TGL_BUKA], 
                                                [BATAS_WAKTU] FROM [dbo].[ANGSURAN_MHS] WHERE [KD_CALON] = @kd_calon)

                                        DECLARE @jaminan INT
                                        SET @jaminan = (SELECT TOP 1 [IS_JAMINAN] FROM [dbo].[ANGSURAN_MHS] WHERE [KD_CALON] = @kd_calon)

                                        SELECT CASE
			                                        WHEN [ANGSURANKE] = 1 AND @jaminan = 1 THEN 'Uang Jaminan'
			                                        WHEN [ANGSURANKE] = 2 AND @jaminan = 1 THEN 'Angsuran I'
			                                        WHEN [ANGSURANKE] = 3 AND @jaminan = 1 THEN 'Angsuran II'
			                                        WHEN [ANGSURANKE] = 4 AND @jaminan = 1 THEN 'Angsuran III'
			                                        WHEN [ANGSURANKE] = 5 AND @jaminan = 1 THEN 'Angsuran IV'
			                                        WHEN [ANGSURANKE] = 6 AND @jaminan = 1 THEN 'Angsuran V'
			                                        WHEN [ANGSURANKE] = 7 AND @jaminan = 1 THEN 'Angsuran VI'
			                                        WHEN [ANGSURANKE] = 8 AND @jaminan = 1 THEN 'Angsuran VII'
			                                        WHEN [ANGSURANKE] = 9 AND @jaminan = 1 THEN 'Angsuran VIII'
			                                        WHEN [ANGSURANKE] = 10 AND @jaminan = 1 THEN 'Angsuran IX'
			                                        WHEN [ANGSURANKE] = 1 AND @jaminan = 0 THEN 'Angsuran I'
			                                        WHEN [ANGSURANKE] = 2 AND @jaminan = 0 THEN 'Angsuran II'
			                                        WHEN [ANGSURANKE] = 3 AND @jaminan = 0 THEN 'Angsuran III'
			                                        WHEN [ANGSURANKE] = 4 AND @jaminan = 0 THEN 'Angsuran IV'
			                                        WHEN [ANGSURANKE] = 5 AND @jaminan = 0 THEN 'Angsuran V'
			                                        WHEN [ANGSURANKE] = 6 AND @jaminan = 0 THEN 'Angsuran VI'
			                                        WHEN [ANGSURANKE] = 7 AND @jaminan = 0 THEN 'Angsuran VII'
			                                        WHEN [ANGSURANKE] = 8 AND @jaminan = 0 THEN 'Angsuran VIII'
			                                        WHEN [ANGSURANKE] = 9 AND @jaminan = 0 THEN 'Angsuran IX'
			                                        WHEN [ANGSURANKE] = 10 AND @jaminan = 0 THEN 'Angsuran X'
													WHEN [ANGSURANKE] = 1 AND @jaminan IS NULL THEN 'Angsuran I'
			                                        WHEN [ANGSURANKE] = 2 AND @jaminan IS NULL THEN 'Angsuran II'
			                                        WHEN [ANGSURANKE] = 3 AND @jaminan IS NULL THEN 'Angsuran III'
			                                        WHEN [ANGSURANKE] = 4 AND @jaminan IS NULL THEN 'Angsuran IV'
			                                        WHEN [ANGSURANKE] = 5 AND @jaminan IS NULL THEN 'Angsuran V'
			                                        WHEN [ANGSURANKE] = 6 AND @jaminan IS NULL THEN 'Angsuran VI'
			                                        WHEN [ANGSURANKE] = 7 AND @jaminan IS NULL THEN 'Angsuran VII'
			                                        WHEN [ANGSURANKE] = 8 AND @jaminan IS NULL THEN 'Angsuran VIII'
			                                        WHEN [ANGSURANKE] = 9 AND @jaminan IS NULL THEN 'Angsuran IX'
			                                        WHEN [ANGSURANKE] = 10 AND @jaminan IS NULL THEN 'Angsuran X'
	                                           END AS ANGSURANKE, STUFF((
                                                SELECT DISTINCT ' + ' + KET
                                                FROM @t
                                                WHERE ANGSURANKE = t.ANGSURANKE
                                                FOR XML PATH('')), 1, 2, '') AS KET_ANGSURAN, (SELECT SUM(JMLUANG) FROM @t WHERE ANGSURANKE = t.ANGSURANKE) AS JMLUANG,
			                                    (SELECT DISTINCT CONCAT(FORMAT([TGL_BUKA],'dd MMMM yyyy','id-id'), ' - ', FORMAT([BATAS_WAKTU],'dd MMMM yyyy','id-id'))
                                                    FROM @t WHERE ANGSURANKE = t.ANGSURANKE) AS BATAS_PEMBAYARAN
                                    FROM (
                                        SELECT DISTINCT ANGSURANKE
                                        FROM @t
                                    ) t";

                    var param = new { kd_calon = kd_calon };
                    var data = conn.Query<CetakSKPUK>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public static string Terbilang(int nilai)
        {
            string[] bilangan = new string[] { "", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan", "sepuluh", "sebelas" };
            var kalimat = "";
            // 1 - 11
            if (nilai < 12)
            {
                kalimat = bilangan[nilai];
            }
            // 12 - 19
            else if (nilai < 20)
            {
                kalimat = bilangan[nilai - 10] + " belas";
            }
            // 20 - 99
            else if (nilai < 100)
            {
                var utama = nilai / 10;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = nilai % 10;
                kalimat = bilangan[depan] + " puluh " + bilangan[belakang];
            }
            // 100 - 199
            else if (nilai < 200)
            {
                kalimat = "seratus " + Terbilang(nilai - 100);
            }
            // 200 - 999
            else if (nilai < 1000)
            {
                var utama = nilai / 100;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = nilai % 100;
                kalimat = bilangan[depan] + " ratus " + Terbilang(belakang);
            }
            // 1,000 - 1,999
            else if (nilai < 2000)
            {
                kalimat = "seribu " + Terbilang(nilai - 1000);
            }
            // 2,000 - 9,999
            else if (nilai < 10000)
            {
                var utama = nilai / 1000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = nilai % 1000;
                kalimat = bilangan[depan] + " ribu " + Terbilang(belakang);
            }
            // 10,000 - 99,999
            else if (nilai < 100000)
            {
                var utama = nilai / 100;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 2));
                var belakang = nilai % 1000;
                kalimat = Terbilang(depan) + " ribu " + Terbilang(belakang);
            }
            // 100,000 - 999,999
            else if (nilai < 1000000)
            {
                var utama = nilai / 1000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 3));
                var belakang = nilai % 1000;
                kalimat = Terbilang(depan) + " ribu " + Terbilang(belakang);
            }
            // 1,000,000 - 	99,999,999
            else if (nilai < 100000000)
            {
                var utama = nilai / 1000000;
                var belakang = nilai % 1000000;
                kalimat =  Terbilang(utama) + " juta " + Terbilang(belakang) + " rupiah ";
            }

            var pisah = kalimat.Split(' ');
            List<string> full = new List<string>(); // = [];
            for (var i = 0; i < pisah.Length; i++)
            {
                if (pisah[i] != "") { full.Add(pisah[i]); }
            }
            return CombineTerbilang(full.ToArray()); // full.Concat(' '); .join(' ');
        }

        static string CombineTerbilang(string[] arr)
        {
            return string.Join(" ", arr);
        }

        public int GetJumlahAngsuran(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT COUNT(*) AS JML_ANGSURAN
                                     FROM [dbo].[ANGSURAN_MHS]
                                     WHERE [KD_CALON] = @kd_calon";

                    var param = new { kd_calon = kd_calon };
                    var data = conn.QuerySingleOrDefault<int>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return 0;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }


    }
}
