using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class CalonMhsDAO
    {
        public List<Pendaftar> GetAllCalonMhs()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (5000) [KD_CALON]
                                          ,[NM_CALON]
                                          ,[JNS_KEL]
                                          ,[TMP_LAHIR]
                                          ,FORMAT([TGL_LAHIR],'dd MMMM yyyy','id-id') AS [TGL_LAHIR]
                                          ,[AGAMA]
                                          ,[KWRGANEGARAAN]
                                          ,[MASUK]
										  ,CASE
												WHEN [MASUK] = '00' THEN 'Tidak diterima'
												WHEN [MASUK] != '00' THEN 'Diterima'
												ELSE 'Dalam proses'
										   END AS KETERANGAN
										  ,[KD_JALUR]
										  ,[periode]
	                                      ,CASE 
												WHEN PRD.[NM_PRODI] = '' THEN '-'
												ELSE PRD.[NM_PRODI]
										   END AS NM_PRODI
                                          ,[THNAKADEMIK]
                                      FROM [dbo].[MHS_PENDAFTAR] MHS
                                      JOIN [dbo].[REF_PRODI] PRD ON MHS.[MASUK] = PRD.[ID_PRODI]
                                      ORDER BY CONVERT(INT,[KD_CALON]) DESC";

                    //WHERE [KD_CALON] = '22334455'";
                    var data = conn.Query<Pendaftar>(query).AsList();

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

        public List<dynamic> GetAllFilterCalonMhs()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [KD_CALON]
                                          ,[NM_CALON]
										  ,JLR.[NAMA_JALUR]
										  ,PRD_1.[NM_PRODI] as PILIHAN_1
										  ,PRD_2.[NM_PRODI] as PILIHAN_2
										  ,PRD_3.[NM_PRODI] as PILIHAN_3
										  ,MSK.[NM_PRODI] as MASUK
										  ,CASE
												WHEN [KEBUTUHAN_KHUSUS_MHS] is null THEN '-'
												WHEN [KEBUTUHAN_KHUSUS_MHS] = '' THEN '-'
												ELSE [KEBUTUHAN_KHUSUS_MHS]
										   END AS [KEBUTUHAN_KHUSUS_MHS] 
										  ,SMA.[NAMA_SMA]
										  ,CASE
												WHEN JLR.[JENJANG] = 's1' THEN 'S1'
												WHEN JLR.[JENJANG] = 's2' THEN 'S2'
                                                WHEN JLR.[JENJANG] = 's3' THEN 'S3'
												ELSE '-'
										   END AS JENJANG 
										  ,[THNAKADEMIK]
										  ,MHS.[KD_JALUR]
                                      FROM [dbo].[MHS_PENDAFTAR] MHS
                                      LEFT OUTER JOIN [dbo].[REF_PRODI] PRD_1 ON MHS.[PILIHAN_1] = PRD_1.[ID_PRODI]
									  LEFT OUTER JOIN [dbo].[REF_PRODI] PRD_2 ON MHS.[PILIHAN_2] = PRD_2.[ID_PRODI]
									  LEFT OUTER JOIN [dbo].[REF_PRODI] PRD_3 ON MHS.[PILIHAN_3] = PRD_3.[ID_PRODI]
									  LEFT OUTER JOIN [dbo].[REF_PRODI] MSK ON MHS.[MASUK] = MSK.[ID_PRODI]
                                      LEFT OUTER JOIN [dbo].[REF_JALUR] JLR ON MHS.[KD_JALUR] = JLR.[KD_JALUR]
									  LEFT OUTER JOIN [Mst_Referensi].[dbo].[REF_SMA] SMA ON MHS.[ID_SMA] = SMA.[ID_SMA]";
                                      //WHERE " ;
                    //if (String.IsNullOrEmpty(search))
                    //{
                    //    query = query + @"([THNAKADEMIK] = @thnAkademik)";
                    //}
                    //else if (String.IsNullOrEmpty(ta))
                    //{
                    //    query = query + @"([KD_CALON] LIKE '%"+HttpUtility.UrlEncode(search)+ "%' OR " +
                    //        "[NM_CALON] LIKE '%"+HttpUtility.UrlEncode(search)+"%')";
                    //} 
                    //else
                    //{
                    //    query = query + @"([THNAKADEMIK] = @thnAkademik and [KD_CALON] LIKE '%" + HttpUtility.UrlEncode(search) + "%' OR " +
                    //       "[NM_CALON] LIKE '%" + HttpUtility.UrlEncode(search) + "%')";
                    //}
                    query = query + @"ORDER BY CONVERT(INT, [KD_CALON]) DESC;";

                    //WHERE [KD_CALON] = '22334455'";
                    var data = conn.Query<dynamic>(query).AsList();

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

        public List<dynamic> GetAllExcelCalonMhs(string ta, string jenjang)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT a.kd_calon
                                           nm_calon,
	                                       c.nama_jalur,
	                                       b.alamat,
	                                       no_kk,
	                                       nik,
	                                       gol_darah,
	                                       agama,
	                                       kwrganegaraan,
	                                       tmp_lahir,
	                                       FORMAT(tgl_lahir,'dd MMMM yyyy','id-id') as tgl_lahir,
	                                       CASE 
			                                    WHEN a.jns_kel = 'L' THEN 'Laki-laki'
			                                    WHEN a.jns_kel = 'P' THEN 'Perempuan'
			                                    ELSE a.jns_kel
	                                       END jns_kel,
	                                       hp_pendaftar,
	                                       periode,
	                                       thnakademik,
                                           h.nama_sma,
	                                       d.nm_prodi as pil_1,
	                                       e.nm_prodi as pil_2,
	                                       f.nm_prodi as pil_3,
                                           g.nm_prodi as prodi_diterima
                                    FROM [MHS_PENDAFTAR] a
                                    LEFT OUTER JOIN dt_alamat_asal b ON a.kd_calon =  b.kd_calon
                                    LEFT OUTER JOIN ref_jalur c ON a.kd_jalur = c.kd_jalur
                                    LEFT OUTER JOIN ref_prodi d ON a.pilihan_1 = d.id_prodi
                                    LEFT OUTER JOIN ref_prodi e ON a.pilihan_2 = e.id_prodi
                                    LEFT OUTER JOIN ref_prodi f ON a.pilihan_3 = f.id_prodi
                                    LEFT OUTER JOIN ref_prodi g ON a.masuk = g.id_prodi
                                    LEFT OUTER JOIN Mst_Referensi.[dbo].ref_sma h ON a.id_sma = h.ID_SMA
                                    WHERE c.jenjang = @jenjang and a.thnakademik = @ta
                                    ORDER BY CONVERT(INT, a.kd_calon) ASC";

                    var data = conn.Query<dynamic>(query, new { jenjang = jenjang, ta = ta }).AsList();

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

        public List<AngsuranMhs> GetDetailAngsuranMhs(string Kd_Calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT MHS.[KD_CALON]
				                          ,MHS.[NM_CALON]
                                          ,[ANGSURANKE]
                                          ,[JMLUANG] AS Jml_Uang
                                          ,FORMAT([TGL_BUKA],'dd MMMM yyyy','id-id') AS [TGL_BUKA]
                                          ,FORMAT([BATAS_WAKTU],'dd MMMM yyyy','id-id') AS [BATAS_WAKTU]
                                          ,[KET_ANGSURAN]
                                          ,[JUMLAH]
                                          ,[POTONGAN]
                                          ,[SKS],
										  CASE
                                            WHEN [IS_JAMINAN] = 1 THEN 'Ya'
                                            WHEN [IS_JAMINAN] = 0 THEN 'Tidak'
										    ELSE 'Tidak'
										  END AS IS_JAMINAN,
										  CASE
                                            WHEN [STATUS] = 1 THEN 'Lunas'
                                            WHEN [STATUS] = 0 THEN 'Belum Lunas'
										    ELSE 'Belum Lunas'
										  END AS [STATUS]
                                      FROM [dbo].[ANGSURAN_MHS] AGS
									  JOIN [dbo].[MHS_PENDAFTAR] MHS ON AGS.[KD_CALON] = MHS.[KD_CALON]
                                      WHERE MHS.[KD_CALON] = @Kd_Calon";

                    var data = conn.Query<AngsuranMhs>(query, new { Kd_Calon = Kd_Calon }).AsList();

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

        public int GenerateAngsuran(AngsuranMhs angsuran)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string cek = @" SELECT
                                        M.[KD_CALON], R.[ANGSURAN_KE], CONVERT(NUMERIC(20,0),(R.[PROSENTASE] * S.[NOMINAL])/100), 
			                            R.[TGL_BUKA], R.[TGL_TUTUP],
			                            CASE
				                            WHEN T.[nama_tagihan] = 'SPU' THEN 'Cicilan SPU'
				                            WHEN T.[nama_tagihan] != 'SPU' THEN T.[nama_tagihan]
                                        END AS NAMA_TAGIHAN, 0,
			                            CASE
				                            WHEN R.[IS_JAMINAN] IS NULL THEN 0
				                            WHEN R.[IS_JAMINAN] IS NOT NULL THEN R.[IS_JAMINAN]
			                            END AS IS_JAMINAN,
			                            S.NOMINAL, PTG.[JLH_TOTAL]
		                                FROM
			                                [Mission].[dbo].[MST_ANGSURAN] A
			                                LEFT JOIN [Mission].[dbo].[DETAIL_RUMUS_ANGSURAN] R ON A.[ID_RUMUS] = R.[ID_RUMUS]
			                                LEFT JOIN [Mission].[dbo].[REF_JALUR] J ON A.[KD_JALUR] = J.[kd_jalur]
			                                LEFT JOIN [Mission].[dbo].[REF_TAGIHAN] T ON R.[ID_TAGIHAN] = T.[id_tagihan]
			                                LEFT JOIN [Mission].[dbo].[MHS_PENDAFTAR] M ON M.[KD_JALUR] = A.[KD_JALUR]
			                                LEFT JOIN [Mission].[dbo].[POTONGAN] PTG ON M.[KD_CALON] = PTG.[KD_CALON]
			                                LEFT JOIN [Mission].[dbo].[TBL_SKPU] S ON R.[ID_TAGIHAN] = S.[ID_TAGIHAN]
                                        WHERE 
			                                M.[KD_CALON] = @Kd_Calon AND S.[ID_PRODI] = @Masuk AND A.[PERIODE] = @Periode AND
			                                A.[KD_JALUR] = @Kd_Jalur AND A.[THNAKADEMIK] = @Thnakademik
                                        ORDER BY R.[ANGSURAN_KE] ASC; ";

                    var data = conn.QueryFirstOrDefault(cek, angsuran);

                    string berhasil = @"SELECT * FROM [Mission].[dbo].[ANGSURAN_MHS] WHERE [KD_CALON] = @Kd_Calon;";

                    var tersimpan = conn.QueryFirstOrDefault(berhasil, angsuran);

                    string query = @"INSERT INTO [Mission].[dbo].[ANGSURAN_MHS] (KD_CALON, ANGSURANKE, JMLUANG, TGL_BUKA, BATAS_WAKTU, KET_ANGSURAN, STATUS, IS_JAMINAN, JUMLAH, POTONGAN) 
                                    " + cek+
                                    " INSERT INTO [sikeuDB_dev].[dbo].[TBL_TAGIHAN] ([NPM],[KD_CALON], [SEMESTER], [ID_REF_TAGIHAN], [JUMLAH], [KURANG_DENDA], [TOTAL], [TGL_OPEN], [TGL_CLOSE], [IS_TAGIH_BANK])\r\n\t\t " +
                                        "SELECT '-' AS NPM, A.[KD_CALON], M.[TH_MASUK] + '1' AS TH_MASUK,\r\n\t\t" +
                                            "CASE\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Cicilan SPU' THEN '00'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'SPP Tetap' THEN '10'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'SPP Variabel' THEN '20'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Matrikulasi' THEN '59'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Biaya Herregistrasi' THEN '61'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Jaket' THEN '57'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Buku' THEN '55'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Courseware' THEN '56'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'SPP Variabel Internasional' THEN '20'\r\n\t\t\t" +
                                                "WHEN A.[KET_ANGSURAN] = 'Kemahasiswaan' THEN '63'\r\n\t\t" +
                                            "END AS ID_TAGIHAN,\r\n\t\t" +
                                            "A.[JMLUANG], 0 AS KURANG_DENDA, A.[JMLUANG] AS TOTAL,\r\n\t\t" +
                                            "[TGL_BUKA], [BATAS_WAKTU], 0\r\n\t\t" +
                                            "FROM [dbo].[ANGSURAN_MHS] A LEFT JOIN [Mission].[dbo].[MHS_PENDAFTAR] M\r\n\t\t" +
                                            "ON M.[KD_CALON] = A.[KD_CALON]\r\n\t\t" +
                                            "WHERE A.[KD_CALON] = @Kd_Calon; ";

                    if (data != null && tersimpan == null)
                    {
                        var insert = conn.Execute(query, angsuran);

                        return 1;
                    }
                    else if (data != null && tersimpan != null)
                    {
                        return 2;
                    }
                    else if (data == null && tersimpan == null)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
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

        public bool HapusAngsuran(string Kd_Calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [Mission].[dbo].[ANGSURAN_MHS] WHERE KD_CALON = @Kd_Calon;

                                     IF EXISTS(SELECT * FROM [sikeuDB_dev].[dbo].[TBL_TAGIHAN] WHERE KD_CALON = @Kd_Calon)
	                                     DELETE FROM [sikeuDB_dev].[dbo].[TBL_TAGIHAN] WHERE KD_CALON = @Kd_Calon;";

                    var data = conn.Execute(query, new { Kd_Calon = Kd_Calon });

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

        public string GetCatatanBayar(string Kd_Calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) [CATATAN_BYR] FROM [sikeuDB_dev].[dbo].[TBL_TAGIHAN] WHERE [KD_CALON] = @Kd_Calon";

                    var data = conn.QueryFirstOrDefault<string>(query, new { Kd_Calon = Kd_Calon });

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

        public string GetIsTagihBank(string Kd_Calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) [IS_TAGIH_BANK] FROM [sikeuDB_dev].[dbo].[TBL_TAGIHAN] WHERE [KD_CALON] = @Kd_Calon";

                    var data = conn.QueryFirstOrDefault<string>(query, new { Kd_Calon = Kd_Calon });

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

        public StorePendaftar2 DetailCalonMhs2(string id)
        {
            using(SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        a.KD_CALON,
                                        a.NM_CALON,
                                        a.KD_JALUR,
                                        a.ID_ORTU,
                                        a.ID_SMA,
                                        a.ID_CALON_ONLINE,
										a.ID_CALON_ONLINE_PASCA,
                                        b.JENJANG,
                                        a.EMAIL,
                                        b.nama_jalur,
                                        b.id_template_nilai,
										CASE 
											WHEN a.is_lengkap_berkas = 1 THEN 'Berkas Lengkap'
											ELSE 'Berkas Tidak Lengkap'
										END berkas
                                    FROM MHS_PENDAFTAR a
                                    JOIN REF_JALUR b ON a.KD_JALUR = b.KD_JALUR
                                    WHERE a.KD_CALON = @Kd_Calon";

                    var param = new { Kd_Calon = id };
                    var data = conn.QueryFirstOrDefault<StorePendaftar2>(query, param);

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

        public dynamic GetDataDiri(string id)
        {
            using(SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        a.KD_CALON,
                                        a.nisn,
	                                    d.nama_jalur,
                                        d.jenjang,
                                        a.EMAIL,
                                        a.periode,
                                        a.NM_CALON,
                                        a.NIK,
                                        a.NO_KK,
                                        a.TMP_LAHIR,
                                        FORMAT(a.TGL_LAHIR,'yyyy-MM-dd','id-id') AS TGL_LAHIR,
                                        CASE 
								            WHEN a.JNS_KEL = 'P' THEN 'Perempuan'
											WHEN a.JNS_KEL = 'L' THEN 'Laki-laki'
                                            ELSE '-'
										END AS JNS_KEL,
                                        a.AGAMA,
                                        a.KWRGANEGARAAN,
                                        a.NEGARA,
                                        a.HP_PENDAFTAR,
                                        c.NAMA_IBU,
                                        b.alamat,
                                        CASE 
								            WHEN a.IS_KPS = 'TRUE' THEN '1'
											WHEN a.IS_KPS = 'FALSE' THEN '0'
                                            ELSE '-'
										END AS IS_KPS,
                                        a.NO_KPS,
                                        CAST(a.JAS as varchar) as JAS,
										(SELECT TOP(1) ukuran FROM ref_jas where id = a.JAS) as ukuran,
                                        a.KEBUTUHAN_KHUSUS_MHS,
										a.alamat_surat,
										a.ID_KONSENTRASI,
										a.PILIHAN_1,
										b.kode_pos,
                                        b.id_kecamatan as kec_asal,
										b.id_kodya as kota_asal,
										(SELECT top(1) KD_PROP FROM Mst_Referensi.dbo.REF_KABUPATEN WHERE id_kabupaten = b.id_kodya) as kd_prop_asal
                                    FROM MHS_PENDAFTAR a
                                    LEFT OUTER JOIN DT_ALAMAT_ASAL b ON a.KD_CALON = b.kd_calon
                                    LEFT OUTER JOIN DT_ORTU c ON a.ID_ORTU = c.ID_ORTU
                                    LEFT OUTER JOIN ref_jalur d ON a.KD_JALUR = d.kd_jalur
                                    WHERE a.KD_CALON = @Kd_Calon";

                    var param = new { Kd_Calon = id };
                    var data = conn.QueryFirstOrDefault<dynamic>(query, param);

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

        public dynamic GetDataSMA(string id)
        {
            using(SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        a.KD_CALON,
                                        CAST(a.ID_SMA as int) as ID_SMA,
										d.NAMA_SMA,
                                        a.JUR_SMA_SMK,
                                        f.KD_PROP,
										f.NAMA_PROP,
										d.ID_KABUPATEN,
										e.NAMA_KABUPATEN
                                    FROM MHS_PENDAFTAR a
                                    LEFT OUTER JOIN Mst_Referensi.dbo.REF_SMA d ON a.ID_SMA = d.ID_SMA
									LEFT OUTER JOIN Mst_Referensi.dbo.REF_KABUPATEN e ON d.ID_KABUPATEN = e.ID_KABUPATEN
									LEFT OUTER JOIN Mst_Referensi.dbo.REF_PROPINSI f ON e.KD_PROP = f.KD_PROP
                                    WHERE a.KD_CALON = @Kd_Calon";

                    var param = new { Kd_Calon = id };
                    var data = conn.QueryFirstOrDefault<dynamic>(query, param);

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

        public dynamic GetDataProdi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        a.KD_CALON,
                                        a.PILIHAN_1,
										a.PILIHAN_2,
										a.PILIHAN_3,
                                        a.ID_KONSENTRASI,
										i.NM_PRODI as NAMA_PIL_1,
										j.NM_PRODI as NAMA_PIL_2,
										k.NM_PRODI as NAMA_PIL_3,
                                        l.KONSENTRASI_STUDI
                                    FROM MHS_PENDAFTAR a
                                    LEFT OUTER JOIN REF_PRODI i ON a.PILIHAN_1 = i.ID_PRODI
									LEFT OUTER JOIN REF_PRODI j ON a.PILIHAN_2 = j.ID_PRODI
									LEFT OUTER JOIN REF_PRODI k ON a.PILIHAN_3 = k.ID_PRODI
                                    LEFT OUTER JOIN TBL_KONSENTRASI_STUDI l ON a.ID_KONSENTRASI = l.ID_KONSENTRASI_STUDI
                                    WHERE a.KD_CALON = @Kd_Calon";

                    var param = new { Kd_Calon = id };
                    var data = conn.QueryFirstOrDefault<dynamic>(query, param);

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
        
        public List<dynamic> GetNilaiUBTK(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                      SUM(CASE WHEN id_ref_nilai_utbk = 1 THEN nilai ELSE 0 END) AS KPU,
                                      SUM(CASE WHEN id_ref_nilai_utbk = 2 THEN nilai ELSE 0 END) AS KMBM,
                                      SUM(CASE WHEN id_ref_nilai_utbk = 3 THEN nilai ELSE 0 END) AS PPU,
                                      SUM(CASE WHEN id_ref_nilai_utbk = 4 THEN nilai ELSE 0 END) AS PK
                                  FROM tbl_nilai_utbk
                                  WHERE kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = kd_calon }).AsList();

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

        public List<dynamic> GetUpdateNilaiUBTK(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT
                                      a.id_nilai_utbk, 
                                      a.id_ref_nilai_utbk,
                                      b.jenis_nilai_utbk,
                                      b.deskripsi,
                                      a.nilai
                                  FROM tbl_nilai_utbk a
                                  JOIN mission.dbo.ref_nilai_utbk b 
                                  ON a.id_ref_nilai_utbk = b.id_ref_nilai_utbk
                                  WHERE kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = kd_calon }).AsList();

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


        public List<dynamic> GetNilaiSMT5(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                      [id_prestasi],
                                      [kd_calon],
                                      CASE WHEN [kelas] = '12' THEN 'XII' ELSE 'XII' END kelas,
                                      CASE WHEN [semester] = '1' THEN '1' ELSE '1' END semester,
                                      [matematika],
                                      [bhs_inggris],
                                      [bahasa],
                                      CASE WHEN [rangking] is null THEN '0' ELSE rangking END rangking
                                  FROM dt_prestasi_pendidikan
                                  WHERE kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = kd_calon }).AsList();

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

        public List<dynamic> GetNilaiNUS(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                      [id_prestasi],
                                      [kd_calon],
                                      [matematika],
                                      [bhs_inggris],
                                      [bahasa]
                                  FROM dt_prestasi_pendidikan
                                  WHERE kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = kd_calon }).AsList();

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

        public List<dynamic> GetNilaiRapot(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT
                                      [id_prestasi],
                                      [kd_calon],
									   CASE
											WHEN [kelas] = '10' THEN 'X'
											WHEN [kelas] = '11' THEN 'XI'
											ELSE [kelas]
									  END kelas,
									  [semester],
									  [kkm_matematika],
                                      [matematika],
									  [kkm_inggris],
                                      [bhs_inggris],
									  [kkm_indonesia],
                                      [bahasa],
									  [rangking]
                                  FROM dt_prestasi_pendidikan
                                  WHERE kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = kd_calon }).AsList();

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

        public List<Prestasi> GetListPrestasiCalonMhs(string Kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT a.id_prestasi
                                      ,a.kd_calon
                                      ,a.jns_keg
                                      ,a.kelompok
                                      ,a.tingkat
                                      ,a.prestasi
                                      ,a.tahun
                                      ,a.ket_prestasi
                                      ,a.id_ref_prestasi
                                      ,a.id
                                      ,b.deskripsi
                                  FROM dt_prestasi a
                                  LEFT OUTER JOIN ref_prestasi b 
                                  ON a.id_ref_prestasi = b.id_ref_prestasi
                                  WHERE kd_calon = @Kd_calon";
                    var data = conn.Query<Prestasi>(query, new { Kd_calon = Kd_calon }).AsList();

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

        public Prestasi GetDetailPrestasiCalonMhs(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1) a.id_prestasi
                                      ,a.kd_calon
                                      ,a.jns_keg
                                      ,a.kelompok
                                      ,a.tingkat
                                      ,a.prestasi
                                      ,a.tahun
                                      ,a.ket_prestasi
                                      ,a.id_ref_prestasi
                                      ,a.id
                                      ,b.deskripsi
                                  FROM dt_prestasi a
                                  LEFT OUTER JOIN ref_prestasi b 
                                  ON a.id_ref_prestasi = b.id_ref_prestasi
                                  WHERE id = @Id";
                   
                    var data = conn.QueryFirstOrDefault<Prestasi>(query, new { Id = id });
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
        public List<Kategori> GetListKategoriCalonMhs()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT id_ref_prestasi, deskripsi FROM ref_prestasi";

                    var data = conn.Query<Kategori>(query).AsList();

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
        
        public List<dynamic> GetListDokumenCalonMhs(string Kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"select distinct
                                        a.kd_calon,
                                        a.id_calon_online,
                                        c.id_ref_jenis_dokumen,
                                        c.jenis_dokumen
                                    FROM mhs_pendaftar a
                                    LEFT JOIN tbl_dokumen_jalur b ON a.kd_jalur = b.kd_jalur
                                    LEFT JOIN ref_jenis_dokumen c ON b.id_ref_jenis_dokumen = c.id_ref_jenis_dokumen
                                    where a.kd_calon = @Kd_calon
                                    order by c.jenis_dokumen asc";

                    var data = conn.Query<dynamic>(query, new { Kd_calon = Kd_calon }).AsList();

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

        public dynamic DataDokumenCalonMhs(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) [id_calon_online]
                                      ,[foto]
                                      ,[foto_ijasah]
                                      ,[foto_nilai_x_1]
                                      ,[foto_nilai_x_2]
                                      ,[foto_nilai_xi_1]
                                      ,[foto_nilai_xi_2]
                                      ,[foto_toefl]
                                      ,[foto_nilai_nus_1]
                                      ,[foto_nilai]
                                      ,[foto_surat_keterangan]
                                      ,[foto_surat_keterangan_tidak_mampu]
                                      ,[foto_slip_gaji_ortu]
                                      ,[foto_surat_rekomendasi]
                                      ,[tulisan_singkat]
                                      ,[foto_surat_keterangan_prestasi]
                                      ,[foto_kartu_keluarga]
                                      ,[foto_identitas]
                                      ,[foto_kip]
                                      ,[foto_utbk]
                                  FROM tbl_pendaftar_online
                                  where id_calon_online = @id";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { id = id });
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

        public List<Jas> GetAllJas()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT CAST(id as varchar) as id, ukuran
                                    FROM REF_JAS 
                                    WHERE IS_PILIH = '1'
                                    ORDER BY id ASC";

                    var data = conn.Query<Jas>(query).AsList();

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

        public bool UbahDataSMACalonMhs(DataSMA ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE MHS_PENDAFTAR SET 
                                        [ID_SMA] = @Id_sma, 
                                        [JUR_SMA_SMK] = @Jurusan_sma
                                    WHERE [KD_CALON] = @Kd_Calon;";

                    var param = new
                    {
                        Kd_Calon = ubah.Kd_calon,
                        Id_sma = ubah.Id_sma,
                        Jurusan_sma = ubah.Jur_sma_smk,
                    };

                    var data = conn.Execute(query, param);
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

        public bool UbahCalonMhs(DataDiri ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE MHS_PENDAFTAR SET
                                 [NM_CALON] = @Nm_Calon,
                                 [TMP_LAHIR] = @Tmp_Lahir,
                                 [TGL_LAHIR] = @Tgl_Lahir,
                                 [JNS_KEL] = @Jns_Kel,
                                 [AGAMA] = @Agama,
                                 [KWRGANEGARAAN] = @Kwrganegaraan,
                                 [HP_PENDAFTAR] = @Hp_pendaftar,
                                 [EMAIL] = @Email,
                                 [periode] = @Periode,
                                 [JAS] = @Jas, 
                                 [KEBUTUHAN_KHUSUS_MHS] = @Kebutuhan_khusus_mhs,
                                 [NO_KPS] = @No_kps,
                                 [nisn] = @nisn,
                                 [ID_ORTU] = @id_ortu,
                                 [NEGARA] = @negara ";
                    if (String.IsNullOrEmpty(ubah.No_kps))
                    {
                        query = query + @", [IS_KPS] = 'false' WHERE [KD_CALON] = @Kd_Calon;";
                    }
                    else
                    {
                        query = query + @",[IS_KPS] = 'true' WHERE [KD_CALON] = @Kd_Calon;";
                    }

                    string cekAlamat = @"SELECT TOP(1) count(*) FROM mission.dbo.DT_ALAMAT_ASAL 
                                  WHERE [KD_CALON] = @Kd_Calon";
                    int dataAlamat = conn.QueryFirstOrDefault<int>(cekAlamat, new { Kd_Calon = ubah.Kd_Calon });

                    if (dataAlamat == 0)
                    {
                        query = query + @"INSERT INTO DT_ALAMAT_ASAL
                                     ([alamat]
                                     ,[kode_pos]
                                     ,[id_kodya]
								    	,[id_kecamatan]
                                     ,[kd_calon])
                             VALUES (@Alamat, @Kode_pos, @Kota_asal, @Kec_asal, @Kd_Calon)";
                    }
                    else
                    {
                        query = query + @"UPDATE DT_ALAMAT_ASAL SET
                                 [alamat] = @Alamat,
                                 [kode_pos] = @Kode_pos,
                                 [id_kodya] = @Kota_asal,
									[id_kecamatan] = @Kec_asal
                             WHERE [KD_CALON] = @Kd_Calon;";
                    }

                    string cekIbu = @"SELECT TOP(1) count(*) FROM DT_ORTU
                                  WHERE [ID_ORTU] = @id_ortu";
                    int dataOrtu = conn.QueryFirstOrDefault<int>(cekIbu, new { id_ortu = ubah.Id_ortu });

                    if (dataOrtu == 0)
                    {
                        string queryIdOrtu = @"SELECT TOP(1) ID_ORTU from DT_ORTU ORDER BY ID_ORTU DESC;";

                        int new_id_ortu = conn.QueryFirstOrDefault<int>(queryIdOrtu, new { id_ortu = ubah.Id_ortu });
                        ubah.Id_ortu = (new_id_ortu + 1).ToString();

                        query = query + @"INSERT INTO DT_ORTU
                                     ([NAMA_IBU],[ID_ORTU])
                             VALUES (@nama_ibu, @id_ortu);";
                    }
                    else
                    {
                        query = query + @"UPDATE DT_ORTU SET
                                 [NAMA_IBU] = @nama_ibu
                             WHERE [ID_ORTU] = @id_ortu;";
                    }

                    var param = new
                    {
                        Kd_Calon = ubah.Kd_Calon,
                        Nm_Calon = ubah.Nm_Calon,
                        Tmp_Lahir = ubah.Tmp_Lahir,
                        Tgl_Lahir = ubah.Tgl_Lahir,
                        Jns_Kel = ubah.Jns_Kel,
                        Agama = ubah.Agama,
                        Kwrganegaraan = ubah.Kwrganegaraan,
                        Hp_pendaftar = ubah.Hp_pendaftar,
                        Email = ubah.Email,
                        Periode = ubah.Periode,
                        Jas = ubah.Jas,
                        No_kps = ubah.No_kps,

                        Alamat = ubah.Alamat,
                        Kode_pos = ubah.Kode_pos,
                        Kota_asal = ubah.Kota_asal,
                        Kec_asal = ubah.Kec_asal,
                        Kebutuhan_khusus_mhs = ubah.Kebutuhan_khusus_mhs,
                        nisn = ubah.Nisn,

                        id_ortu = ubah.Id_ortu,
                        nama_ibu = ubah.Nama_ibu,
                        negara = ubah.Negara
                    };

                    var data = conn.Execute(query, param);
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

        public bool UbahDataProdiCalonMhs(DataProdi ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE MHS_PENDAFTAR SET 
                                        [PILIHAN_1] = @pil_1, 
                                        [PILIHAN_2] = @pil_2,
                                        [PILIHAN_3] = @pil_3
                                    WHERE [KD_CALON] = @Kd_Calon;";

                    var param = new
                    {
                        pil_1 = ubah.pil_1,
                        pil_2 = ubah.pil_2,
                        pil_3 = ubah.pil_3,
                        Kd_Calon = ubah.Kd_calon,
                    };

                    var data = conn.Execute(query, param);
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

        public bool HapusPrestasiCalonMhs(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM dt_prestasi WHERE id = @id;";

                    var data = conn.Execute(query, new { id = id });
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

        public bool TambahPrestasiCalonMhs(Prestasi tambah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryIdPrestasi = @"select top(1) id_prestasi from dt_prestasi where kd_calon=@Kd_calon ORDER BY id_prestasi DESC";


                    int id_prestasi = conn.QueryFirstOrDefault<int>(queryIdPrestasi, new { Kd_calon = tambah.kd_calon });

                    string query = @"INSERT INTO dt_prestasi
                                            (id_prestasi, kd_calon, jns_keg, kelompok, tingkat, prestasi, tahun, ket_prestasi, id_ref_prestasi)
                                    VALUES (@id_prestasi, @kd_calon, @jns_keg, @kelompok, @tingkat, @prestasi, @tahun, @ket_prestasi, @kategori);";

                    var param = new {
                        id_prestasi = id_prestasi+1,
                        kd_calon = tambah.kd_calon,
                        jns_keg = tambah.jns_keg,
                        kelompok = tambah.kelompok,
                        tingkat = tambah.tingkat,
                        prestasi = tambah.prestasi,
                        tahun = tambah.tahun,
                        ket_prestasi = tambah.ket_prestasi,
                        kategori = tambah.id_ref_prestasi
                    };

                    var data = conn.Execute(query, param);
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

        public bool UbahPrestasiCalonMhs(Prestasi tambah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryIdPrestasi = @"select top(1) id_prestasi from dt_prestasi where kd_calon=@Kd_calon ORDER BY id_prestasi DESC";


                    int id_prestasi = conn.QueryFirstOrDefault<int>(queryIdPrestasi, new { Kd_calon = tambah.kd_calon });

                    string query = @"UPDATE dt_prestasi SET
                                        jns_keg = @jns_keg,
                                        kelompok = @kelompok, 
                                        tingkat = @tingkat, 
                                        prestasi = @prestasi, 
                                        tahun = @tahun, 
                                        ket_prestasi = @ket_prestasi, 
                                        id_ref_prestasi = @kategori
                                     WHERE id = @id";

                    var param = new
                    {
                        id = tambah.id,
                        jns_keg = tambah.jns_keg,
                        kelompok = tambah.kelompok,
                        tingkat = tambah.tingkat,
                        prestasi = tambah.prestasi,
                        tahun = tambah.tahun,
                        ket_prestasi = tambah.ket_prestasi,
                        kategori = tambah.id_ref_prestasi
                    };

                    var data = conn.Execute(query, param);
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

        public bool VerifikasiBerkasCalonMhs(Verifikasi verifikasi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE MHS_PENDAFTAR SET
                                         verifikasi_berkas = @Berkas 
                                    WHERE [KD_CALON] = @Kd_Calon";
                   
                    var param = new
                    {
                        Kd_Calon = verifikasi.kd_calon,
                        Berkas = verifikasi.content
                    };

                    var data = conn.Execute(query, param);
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

        public bool VerifikasiKelengkapanBerkasCalonMhs(string id, int is_lengkap)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE MHS_PENDAFTAR SET
                                         is_lengkap_berkas = @is_lengkap 
                                    WHERE [KD_CALON] = @kd_calon";

                    var param = new
                    {
                        kd_calon = id,
                        is_lengkap = is_lengkap
                    };

                    var data = conn.Execute(query, param);
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

        public bool UbahDokumenCalonMhs(UploadDokumen ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = "";
                    if(ubah.id_calon_online > 0)
                    {
                        query = query + @"UPDATE tbl_pendaftar_online SET ";

                        if (ubah.id_ref_jenis_dokumen == 2 || ubah.id_ref_jenis_dokumen == 19)
                        {

                            query = query + @"foto_toefl = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 3)
                        {

                            query = query + @"foto_nilai = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 4)
                        {

                            query = query + @"foto_surat_keterangan = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 5)
                        {

                            query = query + @"foto_surat_keterangan_prestasi = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 6)
                        {

                            query = query + @"foto_slip_gaji_ortu = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 7)
                        {

                            query = query + @"foto_surat_rekomendasi = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 8)
                        {

                            query = query + @"foto_surat_keterangan_tidak_mampu = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 9)
                        {

                            query = query + @"foto_kartu_keluarga = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 10)
                        {

                            query = query + @"foto_ijasah = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 11)
                        {

                            query = query + @"foto_utbk = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 15)
                        {

                            query = query + @"foto = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 16)
                        {

                            query = query + @"foto_identitas = @data_pdf";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 18)
                        {

                            query = query + @"foto_kip = @data_pdf";
                        }


                        query = query + @" WHERE [id_calon_online] = @Id_calon_online";
                    }
                    else
                    {
                        query = query + @"INSERT INTO tbl_pendaftar_online";
                        if (ubah.id_ref_jenis_dokumen == 2 || ubah.id_ref_jenis_dokumen == 19)
                        {

                            query = query + @"(foto_toefl";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 3)
                        {

                            query = query + @"(foto_nilai";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 4)
                        {

                            query = query + @"(foto_surat_keterangan";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 5)
                        {

                            query = query + @"(foto_surat_keterangan_prestasi";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 6)
                        {

                            query = query + @"(foto_slip_gaji_ortu";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 7)
                        {

                            query = query + @"(foto_surat_rekomendasi";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 8)
                        {

                            query = query + @"(foto_surat_keterangan_tidak_mampu";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 9)
                        {

                            query = query + @"(foto_kartu_keluarga";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 10)
                        {

                            query = query + @"(foto_ijasah";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 11)
                        {

                            query = query + @"(foto_utbk";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 15)
                        {

                            query = query + @"(foto";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 16)
                        {

                            query = query + @"(foto_identitas";
                        }
                        else if (ubah.id_ref_jenis_dokumen == 18)
                        {

                            query = query + @"(foto_kip";
                        }

                        query = query + @", kd_jalur)";
                        query = query + @" VALUES(@data_pdf, @jalur) SELECT SCOPE_IDENTITY();";
                    }
                   

                    if(ubah.id_calon_online < 1)
                    {
                        var newItemId = Convert.ToInt32(conn.ExecuteScalar(query, new { data_pdf = ubah.pdf, Id_calon_online = ubah.id_calon_online, jalur = ubah.jalur }));
                        string queryPendaftar = @"UPDATE MHS_PENDAFTAR SET
                                                     ID_CALON_ONLINE = @Id_calon_online
                                                WHERE [KD_CALON] = @Kd_Calon";
                        conn.ExecuteScalar(queryPendaftar, new { Kd_Calon = ubah.kd_calon, Id_calon_online = newItemId });
                    }
                    else
                    {

                        var data = conn.Execute(query, new { data_pdf = ubah.pdf, Id_calon_online = ubah.id_calon_online, jalur = ubah.jalur });
                    }
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

        public bool UbahUTBKCalonMhs(List<UTBK> ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {

                    string query = @"UPDATE tbl_nilai_utbk SET
                                        nilai = @nilai
                                     WHERE id_nilai_utbk = @id";

                    for(int i=0; i < ubah.Count; i++)
                    {
                        var param = new
                        {
                            nilai = ubah[i].nilai,
                            id = ubah[i].id_nilai_utbk
                        };

                        var data = conn.Execute(query, param);
                    }
                    
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

        public bool UbahSMT5CalonMhs(List<SMT5> ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {

                    string query = @"UPDATE dt_prestasi_pendidikan SET
                                        matematika = @mm,
                                        bhs_inggris = @bing,
                                        bahasa = @bindo,
                                        rangking = @rank
                                     WHERE kd_calon = @kd_calon";

                    for (int i = 0; i < ubah.Count; i++)
                    {
                        var param = new
                        {
                            mm = ubah[i].mm,
                            bing = ubah[i].bing,
                            bindo = ubah[i].bindo,
                            rank = ubah[i].rank,
                            kd_calon = ubah[0].Kd_calon,
                        };

                        var data = conn.Execute(query, param);
                    }

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

        public bool UbahNUSCalonMhs(List<NUS> ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {

                    string query = @"UPDATE dt_prestasi_pendidikan SET
                                        matematika = @mm,
                                        bhs_inggris = @bing,
                                        bahasa = @bindo
                                     WHERE kd_calon = @kd_calon";

                    for (int i = 0; i < ubah.Count; i++)
                    {
                        var param = new
                        {
                            mm = ubah[i].mm,
                            bing = ubah[i].bing,
                            bindo = ubah[i].bindo,
                            kd_calon = ubah[0].Kd_calon,
                        };

                        var data = conn.Execute(query, param);
                    }

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

        public bool UbahRapotCalonMhs(List<Rapot> ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {

                    string query = @"UPDATE dt_prestasi_pendidikan SET
                                        matematika = @mm,
                                        bhs_inggris = @bing,
                                        bahasa = @bindo,
                                        kkm_matematika = @kkm_mm,
                                        kkm_inggris = @kkm_bing,
                                        kkm_indonesia = @kkm_bindo
                                     WHERE id_prestasi = @id";

                    for (int i = 0; i < ubah.Count; i++)
                    {
                        var param = new
                        {
                            mm = ubah[i].mm,
                            bing = ubah[i].bing,
                            bindo = ubah[i].bindo,
                            kkm_mm = ubah[i].kkm_mm,
                            kkm_bing = ubah[i].kkm_bing,
                            kkm_bindo = ubah[i].kkm_bindo,
                            id = ubah[i].id_prestasi,
                        };

                        var data = conn.Execute(query, param);
                    }

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
        
    }
}
