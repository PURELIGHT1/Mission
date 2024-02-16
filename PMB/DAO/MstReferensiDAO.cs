using Dapper;
using PMB.Models;
using System.Data.SqlClient;
using System.Web;

namespace PMB.DAO
{
    public class MstReferensiDAO
    {
        public List<MstReferensi> GetAllReferensi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT TOP(100) ID_SMA, NAMA_SMA, ALAMAT
                                    FROM REF_SMA
                                    WHERE iscurrent = 1
									ORDER BY ID_SMA ASC";

                    var data = conn.Query<MstReferensi>(query).AsList();

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

        public List<dynamic> GetFilterReferensi(string cari)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"SELECT ID_SMA, NAMA_SMA, ALAMAT
                                    FROM REF_SMA
                                    WHERE iscurrent = 1 ";
                    if (!String.IsNullOrEmpty(cari) && !String.Equals(cari,"semua"))
                    {
                        query = query + @"and CAST(ID_SMA as varchar) LIKE '%"
                                        + cari + "%' OR NAMA_SMA  LIKE '%"
                                        + cari + "%' OR ALAMAT  LIKE '%"
                                        + cari + "%'";
                    }
                    query = query + @"ORDER BY ID_SMA ASC";
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

        public bool SimpanReferensi(MstReferensiView simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"INSERT INTO REF_SMA
                                            ([NAMA_SMA] 
                                            ,[ALAMAT]
                                            ,[ID_KABUPATEN] 
                                            ,[ID_KECAMATAN] 
                                            ,[KODE_POS]
                                            ,[TELEPON] 
                                            ,[FAKSIMILI]
                                            ,[iscurrent])
                                    VALUES (@nama, @alamat, @kota, @kec, @pos, @hp, @faksimili, 1)";

                    var data = conn.Execute(query, new { 
                        nama = simpan.MstReferensi.NAMA_SMA, 
                        alamat = simpan.MstReferensi.ALAMAT, 
                        kota = simpan.MstReferensi.ID_KABUPATEN, 
                        kec = simpan.MstReferensi.ID_KECAMATAN, 
                        pos = simpan.MstReferensi.KODE_POS, 
                        hp = simpan.MstReferensi.TELEPON, 
                        faksimili = simpan.MstReferensi.FAKSIMILI });

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

        public bool HapusReferensi(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_SMA SET
                                        [iscurrent] = 0
                                    WHERE ID_SMA = @id;";

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

        public MstReferensi DetailMstReferensi(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {

                    string query = @"SELECT TOP (1) 
                                        ID_SMA, NAMA_SMA, ALAMAT, KODE_POS, TELEPON, FAKSIMILI,
                                        d.KD_PROP,
                                        d.NAMA_PROP as PROVINSI, 
                                        a.ID_KABUPATEN, 
                                        c.NAMA_KABUPATEN as KOTA, 
                                        a.ID_KECAMATAN, 
                                        b.nama_kecamatan as KECAMATAN
                                    FROM REF_SMA a
                                    LEFT OUTER JOIN REF_KECAMATAN b ON a.ID_KECAMATAN = b.id_kecamatan
                                    LEFT OUTER JOIN REF_KABUPATEN c ON b.id_kab = c.ID_KABUPATEN
                                    LEFT OUTER JOIN REF_PROPINSI d ON c.KD_PROP = d.KD_PROP
                                    WHERE ID_SMA = @id;";

                    var param = new { id = id };
                    var data = conn.QueryFirstOrDefault<MstReferensi>(query, param);

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

        public bool UbahReferensi(MstReferensiView ubah)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi2))
            {
                try
                {
                    string query = @"UPDATE REF_SMA SET
                                        [ALAMAT] = @alamat,
                                        [ID_KABUPATEN] = @kota,
                                        [ID_KECAMATAN] = @kec,
                                        [KODE_POS] = @pos,
                                        [TELEPON] = @hp,
                                        [FAKSIMILI] = @faksimili
                                    WHERE ID_SMA = @id;";

                    var data = conn.Execute(query, new {
                        id = ubah.MstReferensi.ID_SMA,
                        nama = ubah.MstReferensi.NAMA_SMA,
                        alamat = ubah.MstReferensi.ALAMAT,
                        kota = ubah.MstReferensi.ID_KABUPATEN,
                        kec = ubah.MstReferensi.ID_KECAMATAN,
                        pos = ubah.MstReferensi.KODE_POS,
                        hp = ubah.MstReferensi.TELEPON,
                        faksimili = ubah.MstReferensi.FAKSIMILI
                    });

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
