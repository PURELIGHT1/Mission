using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class MstAngsuranDAO
    {
        public List<MstAngsuran> GetAllAngsuran()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT a.ID_RUMUS, a.JLH_ANGSURAN, a.THNAKADEMIK, a.KD_JALUR, b.NAMA_JALUR, a.PERIODE 
                                    FROM [dbo].[MST_ANGSURAN] a
                                    LEFT OUTER JOIN [dbo].[REF_JALUR] b ON a.KD_JALUR = b.KD_JALUR
									ORDER BY a.[THNAKADEMIK] DESC";

                    var data = conn.Query<MstAngsuran>(query).AsList();

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
        public MstAngsuran DetailAngsuran(string id_rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 a.ID_RUMUS, a.JLH_ANGSURAN, a.THNAKADEMIK, j.nama_jalur AS nama_jalur, a.PERIODE 
                                    FROM [dbo].[MST_ANGSURAN] a
                                    JOIN [dbo].[REF_JALUR] j ON a.KD_JALUR = j.kd_jalur
                                    WHERE ID_RUMUS = @id_rumus";

                    var data = conn.QueryFirstOrDefault<MstAngsuran>(query, new { id_rumus = id_rumus });

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

        public int JumlahAngsuran(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT jlh_angsuran FROM MST_ANGSURAN WHERE ID_RUMUS = @id";

                    var data = conn.QueryFirstOrDefault<int>(query, new { id = id });

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

        public bool SimpanDataAngsuran(StoreMstAngsuran ags)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryUbah = @"UPDATE [dbo].[MST_ANGSURAN] SET
                                       [JLH_ANGSURAN] = @jumlah
                                       ,[THNAKADEMIK] = @ta
                                       ,[KD_JALUR] = @kd_jalur
                                       ,[PERIODE] = @periode
                                    WHERE ID_RUMUS = @id_rumus";

                    string queryTambah = @"INSERT INTO [dbo].[MST_ANGSURAN]
                                       ([JLH_ANGSURAN]
                                       ,[THNAKADEMIK]
                                       ,[KD_JALUR]
                                       ,[PERIODE])
                                 VALUES
                                       (@jumlah, @ta, @kd_jalur, @periode)";

                    if(ags.id_rumus == 0)
                    {
                        var data = conn.Execute(queryTambah, ags);
                    }
                    else
                    {
                        var data = conn.Execute(queryUbah, ags);
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

        public bool SimpanAngsuran(MstAngsuranView ags)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"INSERT INTO [dbo].[MST_ANGSURAN]
                                       ([ID_RUMUS]
                                       ,[JLH_ANGSURAN]
                                       ,[THNAKADEMIK]
                                       ,[KD_JALUR]
                                       ,[PERIODE])
                                 VALUES
                                       (@id_rumus, @jlh_angsuran, @thnakademik, @kd_jalur, @periode)";

                    var data = conn.Execute(query, ags.MstAngsuran);

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

        public bool UbahAngsuran(MstAngsuranView ags)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE [dbo].[MST_ANGSURAN] SET
                                       [JLH_ANGSURAN] = @jlh_angsuran
                                       ,[PERIODE] = @periode
                                    WHERE ID_RUMUS = @id_rumus";

                    var data = conn.Execute(query, ags.MstAngsuran);

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
        public bool HapusAngsuran(int id_rumus)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[MST_ANGSURAN] WHERE ID_RUMUS = @id_rumus";

                    var data = conn.Execute(query, new { id_rumus = id_rumus });

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

        //Count data master angsuran
        //public int CountMstAngsuran()
        //{
        //    using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
        //    {
        //        try
        //        {
        //            string query = @"SELECT MAX(CONVERT(INT, [ID_RUMUS])) AS ID_RUMUS
        //                             FROM dbo.[MST_ANGSURAN]";

        //            var data = conn.QueryFirstOrDefault<int>(query);

        //            return data;
        //        }
        //        catch (Exception ex)
        //        {
        //            return 0;
        //        }
        //        finally
        //        {
        //            conn.Dispose();
        //        }
        //    }
        //}

        //Tabel Ref Jalur -> Jenis jalur penerimaan maba
        public List<Jalur> GetJalur()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT * FROM dbo.REF_JALUR ORDER BY nama_jalur ASC";

                    var data = conn.Query<Jalur>(query).AsList();

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

        // Checking jalur, tahun akademik, dan periode
        public MstAngsuran GetJalurTahunPeriode(string kd_jalur, string thnakademik, int periode)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT MST.[KD_JALUR], JALUR.[nama_jalur], MST.[THNAKADEMIK], MST.[PERIODE]
	                                    FROM [dbo].[MST_ANGSURAN] MST
	                                    JOIN [dbo].[REF_JALUR] JALUR ON JALUR.[kd_jalur] = MST.[KD_JALUR]
                                     WHERE (MST.[KD_JALUR] = @kd_jalur AND MST.[THNAKADEMIK] = @thnakademik AND MST.[PERIODE] = @periode)";

                    var data = conn.QueryFirstOrDefault<MstAngsuran>(query, new { kd_jalur = kd_jalur, thnakademik = thnakademik, periode = periode });

                    return data;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        // Checking jalur, tahun akademik, dan periode
        public string GetNamaJalur(string kd_jalur)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [nama_jalur]
	                                    FROM [dbo].[REF_JALUR]
                                     WHERE [KD_JALUR] = @kd_jalur";

                    var data = conn.QueryFirstOrDefault<string>(query, new { kd_jalur = kd_jalur });

                    return data;
                }
                catch
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
