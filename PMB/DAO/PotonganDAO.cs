using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class PotonganDAO
    {
        public List<Potongan> GetAllPotonganByRumus(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT a.id_dtl_potongan,
                                          a.id_rumus,
                                          a.id_prodi,
                                          a.kd_jalur,
                                          a.id_tagihan,
                                          a.nominal,
                                          a.keterangan,
	                                      c.nama_tagihan,
	                                      d.nama_jalur,
	                                      e.nm_prodi
                                      FROM [detail_rumus_potongan] a
                                      LEFT OUTER JOIN mst_angsuran b ON a.id_rumus = b.id_rumus
                                      LEFT OUTER JOIN ref_tagihan c ON a.id_tagihan = c.id_tagihan
                                      LEFT OUTER JOIN ref_jalur d ON a.kd_jalur = d.kd_jalur
                                      LEFT OUTER JOIN ref_prodi e ON a.id_prodi = e.id_prodi
                                      WHERE a.id_rumus = @id";

                    var data = conn.Query<Potongan>(query, new { id = id }).AsList();

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

        public Potongan GetDetailPotongan(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT a.id_dtl_potongan,
                                          a.id_rumus,
                                          a.id_prodi,
                                          a.kd_jalur,
                                          a.id_tagihan,
                                          a.nominal,
                                          a.keterangan,
	                                      c.nama_tagihan,
	                                      d.nama_jalur,
	                                      e.nm_prodi
                                      FROM [detail_rumus_potongan] a
                                      LEFT OUTER JOIN mst_angsuran b ON a.id_rumus = b.id_rumus
                                      LEFT OUTER JOIN ref_tagihan c ON a.id_tagihan = c.id_tagihan
                                      LEFT OUTER JOIN ref_jalur d ON a.kd_jalur = d.kd_jalur
                                      LEFT OUTER JOIN ref_prodi e ON a.id_prodi = e.id_prodi
                                      WHERE id_dtl_potongan = @id";

                    var data = conn.QueryFirstOrDefault<Potongan>(query, new { id = id });

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

        public bool TambahUbahDataRmsPotongan(Potongan potongan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string cariKdJalur = @"SELECT TOP(1) kd_jalur FROM mst_angsuran WHERE id_rumus = @id_rumus";

                    string kd_jalur = conn.QueryFirstOrDefault<string>(cariKdJalur, new { id_rumus = potongan.id_rumus });

                    string query = @"";
                    if(potongan.id_dtl_potongan < 1)
                    {
                        query = query + @"INSERT INTO detail_rumus_potongan 
                                            (id_rumus, id_prodi, kd_jalur, id_tagihan, nominal, keterangan, semester)
									  VALUES (@id_rumus, @id_prodi, @kd_jalur, @id_tagihan, @nominal, @keterangan, '1');";
                    }
                    else
                    {
                        query = query + @"UPDATE detail_rumus_potongan SET 
                                            id_prodi = @id_prodi,
                                            kd_jalur = @kd_jalur,
                                            id_tagihan = @id_tagihan,
                                            nominal = @nominal,
                                            keterangan = @keterangan
                                        WHERE id_dtl_potongan = @id";
                    }
                    var param = new
                    {
                        id = potongan.id_dtl_potongan,
                        id_rumus = potongan.id_rumus,
                        id_prodi = potongan.id_prodi,
                        kd_jalur = kd_jalur,
                        id_tagihan = potongan.id_tagihan,
                        nominal = potongan.nominal,
                        keterangan = potongan.keterangan

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

        public bool HapusDataRmsPotongan(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM detail_rumus_potongan
                                 WHERE id_dtl_potongan = @id";

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

        //potongan
        public List<dynamic> GetPotonganMandiri(string ta, string jalur)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT
                                        row_number() over (order by a.kd_calon) No,
	                                    id_potongan, 
	                                    a.kd_calon, 
	                                    jns_potongan, 
	                                    jlh_total, 
	                                    jenis, 
	                                    a.id_tagihan, 
	                                    nama_tagihan,
                                        c.kd_jalur,
                                        d.nama_jalur,
                                        c.thnakademik,
                                        e.biaya
                                    FROM Potongan a
                                    INNER JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan
                                    INNER JOIN mhs_pendaftar c ON a.kd_calon = c.kd_calon
                                    INNER JOIN ref_jalur d ON c.kd_jalur = d.kd_jalur
                                    LEFT OUTER JOIN tr_tarif e ON c.masuk = e.id_prodi AND c.th_masuk = e.thmasuk AND b.id_tagihan = e.id_tagihan
                                    WHERE c.thnakademik = @ta";
                    if (!jalur.Equals("All"))
                    {
                        query = query + @" AND c.kd_jalur = @jalur ";
                    }
                    query = query + @" ORDER BY a.kd_calon ASC";      
                    var data = conn.Query<dynamic>(query, new { jalur = jalur, ta = ta }).AsList();
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

        public dynamic GetPotonganMandiriById(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1)
	                                    id_potongan, 
	                                    a.kd_calon, 
	                                    jns_potongan, 
	                                    jlh_total, 
	                                    jenis, 
	                                    a.id_tagihan, 
	                                    nama_tagihan,
                                        c.kd_jalur,
                                        d.nama_jalur,
                                        c.thnakademik
                                    FROM Potongan a
                                    INNER JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan
                                    INNER JOIN mhs_pendaftar c ON a.kd_calon = c.kd_calon
                                    INNER JOIN ref_jalur d ON c.kd_jalur = d.kd_jalur
                                    WHERE a.id_potongan = @id";
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

        
        public List<dynamic> GetTagihan(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT c.id_tagihan, d.nama_tagihan, e.biaya
                                    FROM mhs_pendaftar a
                                    LEFT OUTER JOIN mst_angsuran b ON a.kd_jalur = b.kd_jalur AND a.thnakademik = b.thnakademik
                                    LEFT OUTER JOIN detail_rumus_angsuran c ON b.id_rumus = c.id_rumus
                                    LEFT OUTER JOIN ref_tagihan d ON c.id_tagihan = d.id_tagihan
                                    LEFT OUTER JOIN tr_tarif e ON a.masuk = e.id_prodi AND a.th_masuk = e.thmasuk AND d.id_tagihan = e.id_tagihan
                                    WHERE a.kd_calon = @kd_calon";

                    var data = conn.Query<dynamic>(query, new { kd_calon = id }).AsList();

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

        public bool CekKodeCalon(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT COUNT(*) FROM mhs_pendaftar
                                    WHERE kd_calon = @id";

                    int data = conn.QueryFirstOrDefault<int>(query, new { id = id});
                    if(data < 1)
                    {
                        return false;
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

        public bool SaveDataPotongan(PotonganMhs mhs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {

                    string queryTambah = @"INSERT INTO potongan(kd_calon, jns_potongan, jlh_total, jenis, id_tagihan)
                                  SELECT TOP(1) @kd_calon, @keterangan, @nominal, b.nama_tagihan, @id_tagihan
                                  FROM potongan a
                                  INNER JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan
                                  WHERE kd_calon = @kd_calon";

                    string queryUbah = @"UPDATE potongan
                                      SET jns_potongan = @keterangan,
                                      jlh_total = @nominal,
                                      jenis = b.nama_tagihan,
                                      id_tagihan = @id_tagihan
                                      FROM ref_tagihan b 
                                      WHERE id_potongan = @id_potongan and b.id_tagihan = @id_tagihan";

                    if(mhs.id_potongan < 1)
                    {
                        var data = conn.Execute(queryTambah, mhs);
                        if (data == 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var data = conn.Execute(queryUbah, mhs);
                        if (data == 1)
                        {
                            return true;
                        }
                    }
                    
                    return false;
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

        public bool HapusPotongan(int id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM potongan WHERE id_potongan = @id";

                    var data = conn.Execute(query, new { id = id });
                    if (data == 1)
                    {
                        return true;
                    }
                    return false;
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
        
        //public List<dynamic> GetAllExcelPotonganCalon(string ta, string jalur)
        //{
        //    using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
        //    {
        //        try
        //        {
        //            string query = @"DELETE FROM potongan WHERE id_potongan = @id";

        //            var data = conn.Query<dynamic>(query, new { ta = ta, jalur = jalur }).AsList();

        //            return false;
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
    }
}
