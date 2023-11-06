using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class WilayahDAO
    {
        public bool TambahDataPropinsi(StoreProvinsi tambah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"";

                    string queryID = @"SELECT TOP(1) 
                                            CAST(KD_PROP as int) id
                                      FROM REF_PROPINSI
                                      ORDER BY CAST(KD_PROP as int) DESC";

                    int KDPROP = conn.QueryFirstOrDefault<int>(queryID);

                    var param = new 
                    {
                        id = string.Empty,
                        nama = string.Empty,
                        status = 1
                    };
                    if (!String.IsNullOrEmpty(tambah.kd_prop))
                    {
                        query = query + @"UPDATE REF_PROPINSI SET  
                                            NAMA_PROP = @nama, 
                                            ISCURRENT = @status
                                        WHERE KD_PROP = @id;";
                        param = new
                        {
                            id = tambah.kd_prop,
                            nama = tambah.nama_prop,
                            status = tambah.status,
                        };
                    }
                    else
                    {
                        query = query + @"INSERT INTO REF_PROPINSI  
                                        (KD_PROP, NAMA_PROP, ISCURRENT)
                                    VALUES (@id, @nama, @status);";

                        param = new
                        {
                            id = (KDPROP + 1).ToString(),
                            nama = tambah.nama_prop,
                            status = tambah.status,
                        };
                    }


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

        public bool TambahDataKabupaten(StoreKota tambah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"";

                    string queryID = @"SELECT TOP(1) 
                                            ID_KABUPATEN
                                      FROM REF_KABUPATEN
                                      ORDER BY ID_KABUPATEN DESC";

                    int id_kab = conn.QueryFirstOrDefault<int>(queryID);

                    var param = new
                    {
                        kd_prop = string.Empty,
                        id = 0,
                        nama = string.Empty,
                        status = 1
                    };
                    if (tambah.ID_KABUPATEN > 0)
                    {
                        query = query + @"UPDATE REF_KABUPATEN SET  
                                            NAMA_KABUPATEN = @nama
                                        WHERE ID_KABUPATEN = @id;";
                        param = new
                        {
                            kd_prop = string.Empty,
                            id = tambah.ID_KABUPATEN,
                            nama = tambah.NAMA_KABUPATEN,
                            status = 1,
                        };
                    }
                    else
                    {
                        query = query + @"INSERT INTO REF_KABUPATEN  
                                        (KD_PROP, NAMA_KABUPATEN, ISCURRENT)
                                    VALUES (@kd_prop, @nama, @status);";

                        param = new
                        {
                            kd_prop = tambah.KD_PROP,
                            id = id_kab+1,
                            nama = tambah.NAMA_KABUPATEN,
                            status = 1,
                        };
                    }


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
        
        public bool NonAktifDataPropinsi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_PROPINSI SET
                                            ISCURRENT = 0
                                    WHERE KD_PROP = @id;";

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

        public bool AktifDataPropinsi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_PROPINSI SET
                                            ISCURRENT = 1
                                    WHERE KD_PROP = @id;";

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

        public bool NonAktifDataKab(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_KABUPATEN SET
                                            ISCURRENT = 0
                                    WHERE ID_KABUPATEN = @id;";

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

        public bool AktifDataKab(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_KABUPATEN SET
                                            ISCURRENT = 1
                                    WHERE ID_KABUPATEN = @id;";

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
        public bool HapusDataPropinsi(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string cekDataTerkait = @"";

                    string query = @"DELETE FROM REF_PROPINSI
                                    WHERE KD_PROP = @id;";

                    var data = conn.Execute(query);

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
