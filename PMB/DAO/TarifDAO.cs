using Dapper;
using PMB.Models;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PMB.DAO
{
    public class TarifDAO
    {
        public List<string> GetAllTagihan()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT [nama_tagihan]
                                    FROM [Mission].[dbo].[ref_tagihan]
                                    WHERE is_aktif = '1'";

                    var data = conn.Query<string>(query).AsList();

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

        public List<dynamic> GetAllTarifTagihan(List<string> tagihan) 
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string listTagihan = "";
                    string query = @"select * from (
	                                    SELECT 
		                                    a.id_prodi, 
		                                    b.nm_prodi,
                                            CASE
												WHEN b.jenjang = 's1' THEN 'S1'
												WHEN b.jenjang = 's2' THEN 'S2'
												WHEN b.jenjang = 's3' THEN 'S3'
												ELSE null
										    END AS jenjang,
		                                    a.thmasuk, 
		                                    c.nama_tagihan,
		                                    CASE
											    WHEN (select nama_tagihan from ref_tagihan where id_tagihan = a.id_tagihan) LIKE '%SPP Variabel%' THEN a.biaya * a.jumlah_pengali
											    ELSE biaya
											END AS biaya,
                                            a.iscurrent
	                                    FROM tr_tarif a 
	                                    INNER JOIN ref_prodi b ON a.id_prodi = b.id_prodi  
	                                    INNER JOIN ref_tagihan c ON a.id_tagihan = c.id_tagihan 
	                                    --AND a.id_prodi = '06' 
	                                    --AND a.thmasuk = 2022
                                    ) x
                                    pivot (
	                                    sum(biaya) for nama_tagihan in (";

                    for (int i = 0; i < tagihan.Count; i++)
                    {
                        if (i > 0)
                        {
                            listTagihan += ",";
                        }
                        listTagihan += "[" + tagihan[i] + "]";
                    }

                    query = query + listTagihan;
                    query = query + @")
                                    )p";
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
        public dynamic GetDetailTarifTagihan(string id, int thn, List<string> tagihan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string listTagihan = "";
                    string query = @"select top(1) * from (
	                                    SELECT 
		                                    a.id_prodi, 
		                                    b.nm_prodi, 
		                                    a.thmasuk, 
		                                    c.nama_tagihan, 
		                                    a.biaya, 
											(SELECT MAX(a.jumlah_pengali) FROM tr_tarif a Where a.id_prodi = @prodi and a.thmasuk = @thn) as SKS
	                                    FROM tr_tarif a 
	                                    INNER JOIN ref_prodi b ON a.id_prodi = b.id_prodi  
	                                    INNER JOIN ref_tagihan c ON a.id_tagihan = c.id_tagihan 
                                        WHERE a.id_prodi = @prodi and a.thmasuk = @thn
                                    ) x
                                    pivot (
	                                    sum(biaya) for nama_tagihan in (";

                    for (int i = 0; i < tagihan.Count; i++)
                    {
                        if (i > 0)
                        {
                            listTagihan += ",";
                        }
                        listTagihan += "[" + tagihan[i] + "]";
                    }

                    query = query + listTagihan;
                    query = query + @")
                                    )p";
                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { prodi = id, thn = thn });

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

        public bool SimpanTarifTagihan(Tarif tarif)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string cekProdiDanTahun = @"SELECT id_tarif, id_tagihan FROM tr_tarif WHERE id_prodi = @prodi AND thmasuk = @tahun order by id_tagihan asc";

                    List<TarifTagihan> listTarif = conn.Query<TarifTagihan>(cekProdiDanTahun, new { prodi = tarif.prodi, tarif.tahun }).AsList();

                    string queryTambah = @"INSERT INTO tr_tarif
                                            ([thmasuk], [semester], [id_prodi], [id_tagihan], [biaya], [jumlah_pengali], [iscurrent])
                                        VALUES(@tahun, @semester, @prodi, @tagihan, @biaya, @pengali, 1);";


                    string queryUbah = @"UPDATE tr_tarif SET 
                                            biaya = @biaya,
                                            jumlah_pengali = @pengali,
                                            iscurrent = 1
                                        WHERE id_tarif = @id_tarif";

                    string queryCekTagihan = @"SELECT nama_tagihan FROM ref_tagihan WHERE id_tagihan = @id";

                    int pengali = 1;
                    if (listTarif.Count() == 0)
                    {
                        string tambah = queryTambah;

                        for (int i = 0; i < tarif.biaya.Count(); i++)
                        {
                            string nm_tagihan = conn.QueryFirstOrDefault<string>(queryCekTagihan, new { id = tarif.tagihan[i] });

                            if (Regex.IsMatch(nm_tagihan, @"SPP Variabel"))
                            {
                                pengali = tarif.pengali;
                            }

                            var param = new
                            {
                                tahun = tarif.tahun,
                                semester = 1,
                                prodi = tarif.prodi,
                                tagihan = tarif.tagihan[i],
                                biaya = tarif.biaya[i],
                                pengali = pengali

                            };
                            var data = conn.Execute(tambah, param);
                            pengali = 1;
                        }
                    }
                    else
                    {
                        string ubah = queryUbah;

                        int id_tarif = 0;
                        int biaya = 0;
                        for(int i = 0; i < listTarif.Count(); i++)
                        {
                            for(int j = 0; j < tarif.tagihan.Count(); j++)
                            {
                                if (listTarif[i].id_tagihan.Equals(tarif.tagihan[j]))
                                {
                                    string nm_tagihan = conn.QueryFirstOrDefault<string>(queryCekTagihan, new { id = tarif.tagihan[i] });

                                    if (Regex.IsMatch(nm_tagihan, @"SPP Variabel"))
                                    {
                                        pengali = tarif.pengali;
                                    }

                                    string tagihan = tarif.tagihan[j];
                                    id_tarif = listTarif[i].id_tarif;
                                    biaya = tarif.biaya[j];
                                    var data = conn.Execute(ubah, new { id_tarif = id_tarif, biaya = biaya, pengali = pengali });

                                    if (data == 1)
                                    {
                                        pengali = 1;
                                        tarif.tagihan.RemoveAt(j);
                                        tarif.biaya.RemoveAt(j);
                                    }
                                    break;
                                }
                                continue;
                            }
                        }

                        if(tarif.biaya.Count() != 0)
                        {
                            string tambah = queryTambah;
                            for (int i = 0; i < tarif.biaya.Count(); i++)
                            {
                                string nm_tagihan = conn.QueryFirstOrDefault<string>(queryCekTagihan, new { id = tarif.tagihan[i] });

                                if (Regex.IsMatch(nm_tagihan, @"SPP Variabel"))
                                {
                                    pengali = tarif.pengali;
                                }

                                var param = new
                                {
                                    tahun = tarif.tahun,
                                    semester = 1,
                                    prodi = tarif.prodi,
                                    tagihan = tarif.tagihan[i],
                                    biaya = tarif.biaya[i],
                                    pengali = pengali

                                };
                                var data = conn.Execute(tambah, param);
                                pengali = 1;
                            }

                        }
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

        public bool UbahTarifTagihan(Tarif tarif)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string cekProdiDanTahun = @"SELECT id_tarif, id_tagihan FROM tr_tarif WHERE id_prodi = @prodi AND thmasuk = @tahun order by id_tagihan asc";

                    List<TarifTagihan> listTarif = conn.Query<TarifTagihan>(cekProdiDanTahun, new { prodi = tarif.prodi, tarif.tahun }).AsList();

                    string queryUbah = @"UPDATE tr_tarif SET 
                                            biaya = @biaya,
                                            jumlah_pengali = case when nama_tagihan like '%variabel%' then @pengali else 1 end,
                                            iscurrent = 1
                                        from dbo.tr_tarif inner join dbo.ref_tagihan on tr_tarif.id_tagihan = ref_tagihan.id_tagihan
                                        WHERE id_tarif = @id_tarif";

                    string ubah = queryUbah;

                    int id_tarif = 0;
                    int biaya = 0;
                    int pengali = tarif.pengali;
                    for (int i = 0; i < listTarif.Count(); i++)
                    {
                        for (int j = 0; j < tarif.tagihan.Count(); j++)
                        {
                            if (listTarif[i].id_tagihan.Equals(tarif.tagihan[j]))
                            {
                                string tagihan = tarif.tagihan[j];
                                id_tarif = listTarif[i].id_tarif;
                                biaya = tarif.biaya[j];
                                var data = conn.Execute(ubah, new { id_tarif = id_tarif, biaya = biaya, pengali = pengali });

                                if (data == 1)
                                {
                                    tarif.tagihan.RemoveAt(j);
                                    tarif.biaya.RemoveAt(j);
                                }
                                break;
                            }
                            continue;
                        }
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

        public bool NonAktifTarifTagihan(string prodi, int tahun)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE tr_tarif SET iscurrent = 0 WHERE id_prodi = @prodi AND thmasuk = @tahun";

                    var data = conn.Execute(query, new { prodi = prodi, tahun = tahun });
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

        public bool AktifTarifTagihan(string prodi, int tahun)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE tr_tarif SET iscurrent = 1 WHERE id_prodi = @prodi AND thmasuk = @tahun";

                    var data = conn.Execute(query, new { prodi = prodi, tahun = tahun });
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

        public List<ListTagihan> GetAllRefTagihan(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryJenjang = @"SELECT TOP(1) jenjang FROM ref_prodi WHERE id_prodi = @id_prodi;";

                    string jenjang = conn.QueryFirstOrDefault<string>(queryJenjang, new { id_prodi = id });

                    string query = @"SELECT id_tagihan, nama_tagihan FROM ref_tagihan WHERE is_aktif = '1' AND (jenjang = @jenjang OR jenjang IS NULL)";

                    var data = conn.Query<ListTagihan>(query, new { jenjang = jenjang }).AsList();
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
