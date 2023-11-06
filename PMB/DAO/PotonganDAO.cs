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
    }
}
