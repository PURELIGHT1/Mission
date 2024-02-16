using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class PendaftarDAO
    {
        public List<Provinsi> GetAllProvinsi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT KD_PROP, NAMA_PROP
                                    FROM REF_PROPINSI 
                                    WHERE ISCURRENT = 1
                                    ORDER BY NAMA_PROP ASC;";

                    var data = conn.Query<Provinsi>(query).AsList();

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

        public List<dynamic> GetProvinsi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT KD_PROP, NAMA_PROP, ISCURRENT
                                    FROM REF_PROPINSI 
                                    ORDER BY KD_PROP ASC;";

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

        public Provinsi GetDetailProvinsi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT TOP(1) KD_PROP, NAMA_PROP
                                    FROM REF_PROPINSI 
                                    WHERE ISCURRENT = 1 and KD_PROP = @kd_prop;";

                    var data = conn.QueryFirstOrDefault<Provinsi>(query, new { kd_prop = id });

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

        public List<Kota> GetAllKota(string prop)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT ID_KABUPATEN, NAMA_KABUPATEN
                                    FROM REF_KABUPATEN
                                    WHERE KD_PROP = @prop and ISCURRENT = 1
                                    ORDER BY NAMA_KABUPATEN ASC;";

                    var data = conn.Query<Kota>(query, new { prop = prop }).AsList();

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

        public List<dynamic> GetKota(string prop)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT ID_KABUPATEN, NAMA_KABUPATEN, ISCURRENT
                                    FROM REF_KABUPATEN
                                    WHERE KD_PROP = @prop";

                    var data = conn.Query<dynamic>(query, new { prop = prop }).AsList();

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
        public Kota GetDetailKota(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT ID_KABUPATEN, NAMA_KABUPATEN
                                    FROM REF_KABUPATEN
                                    WHERE ID_KABUPATEN = @id_kabupaten and ISCURRENT = 1;";

                    var data = conn.QueryFirstOrDefault<Kota>(query, new { id_kabupaten = id });

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

        public List<Kecamatan> GetAllKecamatan(int kota)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT DISTINCT id_kecamatan, nama_kecamatan
                                    FROM REF_KECAMATAN
                                    WHERE id_kab = @kota and ISCURRENT = 1
                                    ORDER BY nama_kecamatan ASC;";

                    var data = conn.Query<Kecamatan>(query, new { kota = kota }).AsList();

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

        public List<dynamic> GetAllSMA(int id_kab)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT ID_SMA, NAMA_SMA FROM REF_SMA 
                                    WHERE iscurrent = '1' ";
                    if (id_kab.Equals(0))
                    {
                        query = query + @"ORDER BY NAMA_SMA ASC";
                    }
                    else
                    {
                        query = query + @"and ID_KABUPATEN = @id_kab ORDER BY NAMA_SMA ASC";
                    }

                    var data = conn.Query<dynamic>(query, new { id_kab = id_kab }).AsList();

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

        public List<dynamic> GetAllProdi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT 
                                        ID_PRODI, 
                                        NM_PRODI, 
                                        CASE
                                            WHEN JENJANG = 's1' THEN 'S1'
                                            WHEN JENJANG = 's2' THEN 'S2'
                                            WHEN JENJANG = 's3' THEN 'S3'
										ELSE 'S4'
										END AS JENJANG
                                    FROM REF_PRODI WHERE JENJANG IS NOT NULL ORDER BY ID_PRODI ASC;";
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

        public string GetNamaProdi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        CASE WHEN NM_PRODI = '' THEN 'Tolak Mahasiswa'
	                                    ELSE 'menjadi prodi ' + UPPER(jenjang)+ ' - ' + NM_PRODI
	                                    END AS NM_PRODI
                                    FROM REF_PRODI WHERE ID_PRODI = @id";
                    var data = conn.QueryFirstOrDefault<string>(query, new { id = id });

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

        public List<dynamic> GetAllJalur()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT 
                                        kd_jalur, 
                                        nama_jalur
                                    FROM ref_jalur 
                                    WHERE kd_jalur != 0
                                    ORDER BY nama_jalur ASC;";
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

        public string GetJalurById(string jalur)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
                                        nama_jalur
                                    FROM ref_jalur 
                                    WHERE kd_jalur != 0 and kd_jalur = @jalur;";
                    var data = conn.QueryFirstOrDefault<string>(query, new { jalur = jalur });

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
