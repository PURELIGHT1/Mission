using Dapper;
using Newtonsoft.Json.Linq;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class ProdiDAO
    {
        public List<dynamic> GetPilProdiMhs(string ta, string jalur, string prodi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT 
                                        row_number() over (order by a.kd_calon) No,
	                                    kd_calon, 
	                                    pilihan_1
	                                    pilihan_1, 
	                                    pilihan_2, 
	                                    pilihan_3, 
	                                    c.nm_prodi as pil_1, 
	                                    d.nm_prodi as pil_2, 
	                                    e.nm_prodi as pil_3, 
	                                    pilihan_2, 
	                                    pilihan_3, 
                                        CASE 
		                                    WHEN masuk in(00) THEN ''
                                            WHEN masuk is null THEN ''
		                                    ELSE masuk
	                                    END masuk,
	                                    b.nama_jalur
                                    FROM mhs_pendaftar a
                                    INNER JOIN ref_jalur b ON a.kd_jalur = b.kd_jalur
                                    LEFT OUTER JOIN ref_prodi c ON a.pilihan_1 = c.id_prodi
                                    LEFT OUTER JOIN ref_prodi d ON a.pilihan_2 = d.id_prodi
                                    LEFT OUTER JOIN ref_prodi e ON a.pilihan_3 = e.id_prodi
                                    WHERE a.thnakademik = @ta AND (masuk is null OR masuk in (00, ''))";
                    if (!jalur.Equals("All"))
                    {
                        query = query + @" AND a.kd_jalur = @jalur ";
                    }
                    if (!prodi.Equals("All"))
                    {
                        query = query + @" AND (pilihan_1 LIKE @prodi OR pilihan_2 LIKE @prodi OR pilihan_3 LIKE @prodi) ";
                    }
                    var data = conn.Query<dynamic>(query, new { jalur = jalur, ta = ta, prodi = prodi }).AsList();
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
        
        public bool UbahProdiDiterima(ProdiDiterima prodi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE mhs_pendaftar
                                    SET masuk = @id_prodi
                                    WHERE kd_calon in @listKdCalon";
                    var data = conn.Execute(query, new { id_prodi = prodi.id_prodi, listKdCalon = prodi.kode_calon });
                    if(data > 0)
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

        public bool TambahProdiDiterima(List<ProdiDiterimaExcel> excel)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE mhs_pendaftar
                                    SET masuk = (SELECT TOP 1 id_prodi FROM ref_prodi WHERE UPPER(nm_prodi) LIKE '%' + UPPER(@id_prodi) + '%')
                                    WHERE kd_calon = @kd_calon";
                    int countEksekusi = 0;
                    foreach(ProdiDiterimaExcel item in excel)
                    {
                        var data = conn.Execute(query, new { id_prodi = item.masuk, kd_calon = item.kd_calon });
                        countEksekusi += data;
                    }
                    if(countEksekusi == excel.Count())
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

        public ProdiDiterimaExcel CekDataExcelDatabase(string kd_calon, string prodi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT 
                                        kd_calon,  
                                        c.nm_prodi as pil_1, 
                                        d.nm_prodi as pil_2, 
                                        e.nm_prodi as pil_3,
                                        (
                                            SELECT TOP 1 nm_prodi 
                                            FROM ref_prodi 
                                            WHERE UPPER(nm_prodi) LIKE '%' + UPPER(@prodi) + '%'
                                        ) AS masuk

                                    FROM mhs_pendaftar a
                                    LEFT OUTER JOIN ref_prodi c ON a.pilihan_1 = c.id_prodi
                                    LEFT OUTER JOIN ref_prodi d ON a.pilihan_2 = d.id_prodi
                                    LEFT OUTER JOIN ref_prodi e ON a.pilihan_3 = e.id_prodi
                                    WHERE a.kd_calon = @kd_calon AND 
                                    (
                                        UPPER(c.nm_prodi) LIKE '%' + UPPER(@prodi) + '%' OR 
                                        UPPER(d.nm_prodi) LIKE '%' + UPPER(@prodi) + '%' OR 
                                        UPPER(e.nm_prodi) LIKE '%' + UPPER(@prodi) + '%'
                                    ) AND (masuk is null OR masuk in (00, ''))";

                    var data = conn.QueryFirstOrDefault<ProdiDiterimaExcel>(query, new { kd_calon = kd_calon, prodi = prodi });
                    
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
