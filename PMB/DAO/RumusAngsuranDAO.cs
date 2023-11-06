using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class RumusAngsuranDAO
    {
        public List<RmsAngsuran> DetailRumusAngsuran(int id_rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT ROW_NUMBER() OVER(ORDER BY (R.[ANGSURAN_KE])) AS NOMOR, R.[ID_DTL_RUMUS_ANGSURAN] AS ID_DETAIL, A.[ID_RUMUS], R.[ID_TAGIHAN],
                                         T.[nama_tagihan] AS jenis_tagihan,R.[ANGSURAN_KE], CAST(R.[PROSENTASE] as int) as JML_PERSENTASE, FORMAT(R.[TGL_BUKA],'dd MMMM yyyy','id-id') AS TGL_BUKA,
                                         FORMAT(R.[TGL_TUTUP],'dd MMMM yyyy','id-id') AS TGL_TUTUP, R.[KETERANGAN], 
                                         CASE
                                            WHEN R.[IS_JAMINAN] = 1 THEN 'Ya'
                                            WHEN R.[IS_JAMINAN] = 0 THEN 'Tidak'
										    ELSE 'Tidak'
										    END AS IS_JAMINAN
                                         FROM [dbo].[MST_ANGSURAN] A
									     JOIN [dbo].[DETAIL_RUMUS_ANGSURAN] R ON A.[ID_RUMUS] = R.[ID_RUMUS]
                                         LEFT OUTER JOIN [dbo].[REF_TAGIHAN] T ON R.[ID_TAGIHAN] = T.[id_tagihan]
                                     WHERE A.[ID_RUMUS] = @id_rumus
                                     GROUP BY R.[ID_DTL_RUMUS_ANGSURAN], A.[ID_RUMUS], R.[ANGSURAN_KE], R.[PROSENTASE], T.[nama_tagihan],
                                         R.[TGL_BUKA], R.[TGL_TUTUP], R.[KETERANGAN], R.[IS_JAMINAN], R.[ID_TAGIHAN]";

                    var data = conn.Query<RmsAngsuran>(query, new { id_rumus = id_rumus }).AsList();

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

        public dynamic DetailDataRumusAngsuran(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1)  R.[ID_DTL_RUMUS_ANGSURAN] AS ID_DETAIL, A.[ID_RUMUS], R.[ID_TAGIHAN],
                                         T.[nama_tagihan] AS jenis_tagihan,R.[ANGSURAN_KE], CAST(R.[PROSENTASE] as int) as JML_PERSENTASE, FORMAT(R.[TGL_BUKA],'M/d/yyyy','id-id') AS TGL_BUKA,
                                         FORMAT(R.[TGL_TUTUP],'M/d/yyyy','id-id') AS TGL_TUTUP, R.[KETERANGAN], 
                                         CASE
                                            WHEN R.[IS_JAMINAN] = 1 THEN 'Ya'
                                            WHEN R.[IS_JAMINAN] = 0 THEN 'Tidak'
										    ELSE 'Tidak'
										    END AS IS_JAMINAN
                                         FROM [dbo].[MST_ANGSURAN] A
									     JOIN [dbo].[DETAIL_RUMUS_ANGSURAN] R ON A.[ID_RUMUS] = R.[ID_RUMUS]
                                         LEFT OUTER JOIN [dbo].[REF_TAGIHAN] T ON R.[ID_TAGIHAN] = T.[id_tagihan]
                                     WHERE R.[ID_DTL_RUMUS_ANGSURAN] = @id";

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

        public RmsAngsuran DetailRumus(int id_detail)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) r.ID_DTL_RUMUS_ANGSURAN AS id_detail, r.ID_RUMUS, r.ANGSURAN_KE, r.PROSENTASE, a.JLH_ANGSURAN,
                                     r.ID_TAGIHAN, t.nama_tagihan AS jenis_tagihan, CONVERT(VARCHAR,r.[TGL_BUKA],23) AS TGL_BUKA,
									 CONVERT(VARCHAR,r.[TGL_TUTUP],23) AS TGL_TUTUP,r.KETERANGAN, r.IS_JAMINAN
                                     FROM [dbo].[DETAIL_RUMUS_ANGSURAN] r
                                     LEFT JOIN [dbo].[REF_TAGIHAN] t ON r.ID_TAGIHAN = t.id_tagihan
									 LEFT JOIN [dbo].[MST_ANGSURAN] a ON a.ID_RUMUS = r.ID_RUMUS
                                     WHERE [ID_DTL_RUMUS_ANGSURAN] = @id_detail";

                    var data = conn.QueryFirstOrDefault<RmsAngsuran>(query, new { id_detail = id_detail });

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

        public List<RmsAngsuran> GetRumusByID(int id_rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT r.ID_DTL_RUMUS_ANGSURAN AS id_detail, r.ID_RUMUS, r.ANGSURAN_KE, r.PROSENTASE, a.JLH_ANGSURAN,
                                         t.nama_tagihan AS jenis_tagihan, CONVERT(VARCHAR,r.[TGL_BUKA],23) AS TGL_BUKA,
									     CONVERT(VARCHAR,r.[TGL_TUTUP],23) AS TGL_TUTUP,r.KETERANGAN, r.IS_JAMINAN
                                         FROM [dbo].[DETAIL_RUMUS_ANGSURAN] r
                                         LEFT JOIN [dbo].[REF_TAGIHAN] t ON r.ID_TAGIHAN = t.id_tagihan
										 LEFT JOIN [dbo].[MST_ANGSURAN] a ON r.[ID_RUMUS] = a.[ID_RUMUS]
                                     WHERE r.[ID_RUMUS] = @id_rumus";

                    var data = conn.Query<RmsAngsuran>(query, new { id_rumus = id_rumus }).AsList();

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

        public int GetJmlAngsuran(int id_rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT JLH_ANGSURAN
                                     FROM [dbo].[MST_ANGSURAN]
                                     WHERE [ID_RUMUS] = @id_rumus";

                    var data = conn.QueryFirstOrDefault<int>(query, new { id_rumus = id_rumus });

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

        public bool SimpanRumus(StoreRmsAngsuran rumusAgs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryTambah = @"INSERT INTO [dbo].[DETAIL_RUMUS_ANGSURAN]
                                       ([ID_RUMUS]
                                        ,[ID_TAGIHAN]
                                        ,[ANGSURAN_KE]
                                        ,[PROSENTASE]
                                        ,[TGL_BUKA]
                                        ,[TGL_TUTUP]
                                        ,[IS_JAMINAN])
                                    VALUES
                                       (@id_rumus, @id_tagihan, @angsuran_ke, @prosentase,
                                        CONVERT(DATETIME,@tgl_buka), CONVERT(DATETIME,@tgl_tutup), @is_jaminan)";

                    string queryUbah = @"UPDATE [dbo].[DETAIL_RUMUS_ANGSURAN] SET
                                        [PROSENTASE] = @prosentase
                                        ,[ID_TAGIHAN] = @id_tagihan
                                        ,[TGL_BUKA] = CONVERT(DATETIME,@tgl_buka)
                                        ,[TGL_TUTUP] = CONVERT(DATETIME,@tgl_tutup)
                                        ,[IS_JAMINAN] = @is_jaminan
                                    WHERE [ID_DTL_RUMUS_ANGSURAN] = @id_detail";


                    var param = new
                    {
                        id_detail = rumusAgs.id_detail,
                        id_rumus = rumusAgs.id_rumus.ToString(),
                        id_tagihan = rumusAgs.id_tagihan,
                        angsuran_ke = rumusAgs.angsuran_ke,
                        prosentase = rumusAgs.jml_persentase.ToString(),
                        tgl_buka = rumusAgs.tgl_buka,
                        tgl_tutup = rumusAgs.tgl_tutup,
                        is_jaminan = rumusAgs.jaminan
                    };

                    if (rumusAgs.id_detail < 1)
                    {
                        var data = conn.Execute(queryTambah, param);
                    }
                    else
                    {
                        var data = conn.Execute(queryUbah, param);
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

        public List<int> CekTotalPresentasiRumus(string rumus, int tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT PROSENTASE FROM mission.dbo.DETAIL_RUMUS_ANGSURAN WHERE ID_RUMUS = @id_rumus and ID_TAGIHAN = @id_tagihan";


                    var param = new
                    {
                        id_rumus = rumus,
                        id_tagihan = tagihan
                    };

                    var data = conn.Query<int>(query, param).AsList();

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

        public bool UbahRmsAngsuran(RmsAngsuranView rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE [dbo].[DETAIL_RUMUS_ANGSURAN] SET
                                        [PROSENTASE] = @prosentase
                                        ,[ID_TAGIHAN] = @id_tagihan
                                        ,[TGL_BUKA] = CONVERT(DATETIME,@tgl_buka)
                                        ,[TGL_TUTUP] = CONVERT(DATETIME,@tgl_tutup)
                                        ,[KETERANGAN] = @keterangan
                                        ,[IS_JAMINAN] = @is_jaminan
                                    WHERE [ID_DTL_RUMUS_ANGSURAN] = @id_detail";

                    var data = conn.Execute(query, rumus.RmsAngsuran);

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

        public bool HapusRmsAngsuran(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[DETAIL_RUMUS_ANGSURAN] WHERE [ID_DTL_RUMUS_ANGSURAN] = @id_detail";

                    var data = conn.Execute(query, new { id_detail = id });

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

        public int GetJumlahPersentase(string id_rumus, int id_tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT SUM(CONVERT(INT,RMS.[PROSENTASE])) AS JML_PERSENTASE
                                         FROM [dbo].[MST_ANGSURAN] AGS
                                         JOIN [dbo].[DETAIL_RUMUS_ANGSURAN] RMS ON AGS.[ID_RUMUS] = RMS.[ID_RUMUS]
                                     WHERE RMS.[ID_RUMUS] = @id_rumus AND RMS.[ID_TAGIHAN] = @id_tagihan";

                    var param = new { id_rumus = id_rumus, id_tagihan = id_tagihan };
                    var data = conn.QueryFirstOrDefault<int>(query, param);

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

        public int GetPersentaseEdit(int id_detail, int id_tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [PROSENTASE]
                                     FROM [dbo].[DETAIL_RUMUS_ANGSURAN]
                                     WHERE [ID_DTL_RUMUS_ANGSURAN] = @id_detail AND [ID_TAGIHAN] = @id_tagihan";

                    var param = new { id_detail = id_detail, id_tagihan = id_tagihan };
                    var data = conn.QueryFirstOrDefault<int>(query, param);

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

        public string GetBatasAwal(string id_rumus, int angsuran_ke)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 [TGL_BUKA]
                                     FROM [dbo].[DETAIL_RUMUS_ANGSURAN]
                                     WHERE [ID_RUMUS] = @id_rumus AND [ANGSURAN_KE] = @angsuran_ke";

                    var param = new { id_rumus = id_rumus, angsuran_ke = angsuran_ke };
                    var data = conn.QueryFirstOrDefault<string>(query, param);

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

        public string GetBatasAkhir(string id_rumus, int angsuran_ke)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 [TGL_TUTUP]
                                     FROM [dbo].[DETAIL_RUMUS_ANGSURAN]
                                     WHERE [ID_RUMUS] = @id_rumus AND [ANGSURAN_KE] = @angsuran_ke";

                    var param = new { id_rumus = id_rumus, angsuran_ke = angsuran_ke };
                    var data = conn.QueryFirstOrDefault<string>(query, param);

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

        public string GetJaminan(string id_rumus, int angsuran_ke)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 [IS_JAMINAN]
                                     FROM [dbo].[DETAIL_RUMUS_ANGSURAN]
                                     WHERE [ID_RUMUS] = @id_rumus AND [ANGSURAN_KE] = @angsuran_ke";

                    var data = conn.QueryFirstOrDefault<string>(query, new { id_rumus = id_rumus, angsuran_ke = angsuran_ke });

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

        public RmsAngsuran GetJaminanTgl(string id_rumus, int angsuran_ke)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 [IS_JAMINAN], CONVERT(VARCHAR,[TGL_BUKA],23) AS TGL_BUKA, CONVERT(VARCHAR,[TGL_TUTUP],23) AS TGL_TUTUP
                                        FROM [dbo].[DETAIL_RUMUS_ANGSURAN]
                                     WHERE [ID_RUMUS] = @id_rumus AND [ANGSURAN_KE] = @angsuran_ke";

                    var data = conn.QueryFirstOrDefault<RmsAngsuran>(query, new { id_rumus = id_rumus, angsuran_ke = angsuran_ke });

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

        //Tabel Ref Tagihan -> Jenis tagihan
        public List<Tagihan> GetTagihan()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT id_tagihan, nama_tagihan
                                     FROM [dbo].[REF_TAGIHAN] Where is_aktif = '1'";

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

        public string GetJenisTagihan(int id_tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT LOWER([nama_tagihan]) AS [nama_tagihan]
                                     FROM [dbo].[REF_TAGIHAN]
                                     WHERE [id_tagihan] = @id_tagihan";

                    var param = new { id_tagihan = id_tagihan };
                    var data = conn.QueryFirstOrDefault<string>(query, param);

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
    }
}
