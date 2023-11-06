using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class SuratDiterimaDAO
    {
        public List<SuratDiterima> GetListRegis()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1000) [no_surat],
		                                    CASE
			                                    WHEN A.[kd_jalur] IS NULL THEN '-'
			                                    ELSE A.[kd_jalur]
		                                    END AS KD_JALUR,
		                                    CASE
			                                    WHEN B.[nama_jalur] IS NULL THEN '-'
			                                    ELSE B.[nama_jalur]
		                                    END AS NAMA_JALUR
		                                    ,[periode],
		                                    CASE
			                                    WHEN A.[angsuran1] IS NULL THEN '-'
			                                    WHEN A.[angsuran1] = '' THEN '-'
			                                    ELSE A.[angsuran1]
		                                    END AS ANGSURAN1,
											CASE
			                                    WHEN A.[registrasi1] IS NULL THEN '-'
			                                    WHEN A.[registrasi1] = '' THEN '-'
			                                    ELSE A.[registrasi1]
		                                    END AS PERIODE_REGIS,
		                                    CASE
			                                    WHEN A.[inisiasi1] IS NULL THEN '-'
			                                    WHEN A.[inisiasi1] = '' THEN '-'
			                                    ELSE A.[inisiasi1]
		                                    END AS PERIODE_INISIASI,
											CASE
			                                    WHEN A.[tgl_inisiasi1] IS NULL THEN '-'
			                                    WHEN A.[tgl_inisiasi1] = '' THEN '-'
			                                    ELSE A.[tgl_inisiasi1]
		                                    END AS TGL_INISIASI,
											CASE
			                                    WHEN A.[perkulliahan] IS NULL THEN '-'
			                                    WHEN A.[perkulliahan] = '' THEN '-'
			                                    ELSE A.[perkulliahan]
		                                    END AS PERKULIAHAN,
											CASE
			                                    WHEN A.[tgl_kelompok_1] IS NULL THEN '-'
			                                    WHEN A.[tgl_kelompok_1] = '' THEN '-'
			                                    ELSE A.[tgl_kelompok_1]
		                                    END AS TGL_REGIS,
											CASE
			                                    WHEN A.[surat_pembatalan] IS NULL THEN '-'
			                                    WHEN A.[surat_pembatalan] = '' THEN '-'
			                                    ELSE A.[surat_pembatalan]
		                                    END AS SURAT_PEMBATALAN,
                                            CASE
			                                    WHEN A.[batas_permohonan] IS NULL THEN '-'
			                                    WHEN A.[batas_permohonan] = '' THEN '-'
			                                    ELSE A.[batas_permohonan]
		                                    END AS BATAS_PERMOHONAN
                                      FROM [Mission].[dbo].[suratDiterima] A
                                      LEFT JOIN [dbo].[REF_JALUR] B ON A.[kd_jalur] = B.[kd_jalur]";

                    var data = conn.Query<SuratDiterima>(query).AsList();

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

        public SuratDiterima CekData(string Kd_Jalur, string Periode)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [kd_jalur], [periode]
                                          FROM [Mission].[dbo].[suratDiterima]
									 WHERE [kd_jalur] = @Kd_Jalur AND [periode] = @Periode";

                    var param = new { Kd_Jalur = Kd_Jalur, Periode = Periode };
                    var data = conn.QueryFirstOrDefault<SuratDiterima>(query, param);

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

            public List<Jalur> GetJalur()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT * FROM dbo.REF_JALUR";

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

        public bool Insert(SuratDiterima simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string q1 = "CONCAT((@Tgl_Regis),(' melalui web sikaa.uajy.ac.id'))";

                    if (simpan.Tgl_Regis == "-")
                    {
                        q1 = "@Tgl_Regis";
                    }
                    string query = @"INSERT INTO [dbo].[suratDiterima]
		                                ([no_surat],
		                                 [kd_jalur],
		                                 [periode],
		                                 [perkulliahan],
                                         [tgl_kelompok_1])
	                                 VALUES (@No_Surat, @Kd_Jalur, @Periode, @Perkuliahan, " +q1+")";
                    
                    var data = conn.Execute(query, simpan);

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

        public SuratDiterima DetailSurat(string Kd_Jalur, string Periode)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) [no_surat],
		                                    CASE
			                                    WHEN A.[kd_jalur] IS NULL THEN '-'
			                                    ELSE A.[kd_jalur]
		                                    END AS KD_JALUR,
		                                    CASE
			                                    WHEN B.[nama_jalur] IS NULL THEN '-'
			                                    ELSE B.[nama_jalur]
		                                    END AS NAMA_JALUR,
                                            [periode],
		                                    CASE
			                                    WHEN A.[angsuran1] IS NULL THEN '-'
			                                    WHEN A.[angsuran1] = '' THEN '-'
			                                    ELSE A.[angsuran1]
		                                    END AS ANGSURAN1,
											CASE
			                                    WHEN A.[registrasi1] IS NULL THEN '-'
			                                    WHEN A.[registrasi1] = '' THEN '-'
			                                    ELSE A.[registrasi1]
		                                    END AS PERIODE_REGIS,
		                                    CASE
			                                    WHEN A.[inisiasi1] IS NULL THEN '-'
			                                    WHEN A.[inisiasi1] = '' THEN '-'
			                                    ELSE A.[inisiasi1]
		                                    END AS PERIODE_INISIASI,
											CASE
			                                    WHEN A.[tgl_inisiasi1] IS NULL THEN '-'
			                                    WHEN A.[tgl_inisiasi1] = '' THEN '-'
			                                    ELSE A.[tgl_inisiasi1]
		                                    END AS TGL_INISIASI,
											CASE
			                                    WHEN A.[perkulliahan] IS NULL THEN '-'
			                                    WHEN A.[perkulliahan] = '' THEN '-'
			                                    ELSE A.[perkulliahan]
		                                    END AS PERKULIAHAN,
											CASE
			                                    WHEN A.[tgl_kelompok_1] IS NULL THEN '-'
			                                    WHEN A.[tgl_kelompok_1] = '' THEN '-'
			                                    ELSE A.[tgl_kelompok_1]
		                                    END AS TGL_REGIS,
											CASE
			                                    WHEN A.[surat_pembatalan] IS NULL THEN '-'
			                                    WHEN A.[surat_pembatalan] = '' THEN '-'
			                                    ELSE A.[surat_pembatalan]
		                                    END AS SURAT_PEMBATALAN,
                                            CASE
			                                    WHEN A.[batas_permohonan] IS NULL THEN '-'
			                                    WHEN A.[batas_permohonan] = '' THEN '-'
			                                    ELSE A.[batas_permohonan]
		                                    END AS BATAS_PERMOHONAN
                                      FROM [Mission].[dbo].[suratDiterima] A
                                      LEFT JOIN [dbo].[REF_JALUR] B ON A.[kd_jalur] = B.[kd_jalur]
									  WHERE A.[kd_jalur] = @Kd_Jalur AND A.[periode] = @Periode";

                    var param = new { Kd_Jalur = Kd_Jalur, Periode = Periode };
                    var data = conn.QueryFirstOrDefault<SuratDiterima>(query, param);

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

        public bool Update(SuratDiterima ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string q1 = ", [tgl_kelompok_1] = CONCAT(@Tgl_Regis, ' melalui web sikaa.uajy.ac.id') ";

                    string cek = "melalui web sikaa.uajy.ac.id";
                    bool contain = ubah.Tgl_Regis.Contains(cek);

                    if (ubah.Tgl_Regis == "-")
                    {
                        q1 = ", [tgl_kelompok_1] = @Tgl_Regis ";
                    }
                    else if (contain)
                    {
                        int index = ubah.Tgl_Regis.IndexOf(cek);
                        if (index >= 0)
                        {
                            q1 = ", [tgl_kelompok_1] = @Tgl_Regis ";
                        }
                    }
                    else
                    {
                        q1 = ", [tgl_kelompok_1] = CONCAT(@Tgl_Regis, ' melalui web sikaa.uajy.ac.id') ";
                    }
                    
                    string query = @"UPDATE [dbo].[suratDiterima] SET
                                       [no_surat] = @No_Surat
                                       ,[perkulliahan] = @Perkuliahan
                                       ,[registrasi1] = @Periode_Regis"+q1+
                                    "WHERE [kd_jalur] = @Kd_Jalur AND [periode] = @Periode";

                    var data = conn.Execute(query, ubah);

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

        public bool Delete(string Kd_Jalur, string Periode)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[suratDiterima] WHERE [kd_jalur] = @Kd_Jalur AND [periode] = @Periode";

                    var data = conn.Execute(query, new { Kd_Jalur = Kd_Jalur, Periode = Periode });

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
