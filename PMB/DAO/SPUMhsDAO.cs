using Dapper;
using PMB.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PMB.DAO
{
    public class SPUMhsDAO
    {
        //Tabel SPU
        public List<SPUMhs2> GetAllSPU()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT
		                                A.kd_calon,
		                                B.nm_calon,
		                                B.masuk,
		                                CONCAT(CASE WHEN c.jenjang = 's1' THEN 'S1'
													WHEN c.jenjang = 's2' THEN 'S2'
													WHEN c.jenjang = 's3' THEN 'S3'
												ELSE null 
												END, ' - ',c.nm_prodi) prodi_diterima,
		                                B.kd_jalur,
		                                D.nama_jalur,
		                                B.periode
                                  FROM spu A
                                  JOIN mhs_pendaftar B ON A.kd_calon = B.kd_calon
                                  JOIN ref_prodi C ON B.masuk = C.id_prodi
                                  JOIN ref_jalur D ON B.kd_jalur = D.kd_jalur
                                  ORDER BY kd_calon DESC";
                    
                    var data = conn.Query<SPUMhs2>(query).AsList();

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

        public dynamic GetDetailSPU(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1) 
									    a.kd_calon,
									    b.kd_jalur,
									    c.nama_jalur,
									    b.masuk,
									    CONCAT(CASE WHEN d.jenjang = 's1' THEN 'S1'
										    WHEN d.jenjang = 's2' THEN 'S2'
										    WHEN d.jenjang = 's3' THEN 'S3'
										    ELSE null 
									    END, ' - ', d.nm_prodi) as prodi_diterima,
										CONVERT(varchar, a.tgl_cetak, 20) as tgl_cetak,
									    a.spu
								      FROM spu a
								      JOIN mhs_pendaftar b ON a.kd_calon = b.kd_calon
								      JOIN ref_jalur c ON b.kd_jalur = c.kd_jalur
								      JOIN ref_prodi d ON b.masuk = d.id_prodi
								      WHERE a.kd_calon = @kd_calon";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { kd_calon = id });
                    return data;
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

        public bool SimpanSPU(SPUMhsView simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"INSERT INTO [dbo].[SPU]
	                                    ([KD_CALON]
	                                    ,[SPU]
	                                    ,[USERNAME]
	                                    ,[TANGGAL]
                                        ,[TGL_CETAK]
	                                    ,[IS_MATRIKULASI])
                                    VALUES
                                        (@kd_calon, @spu, @username, GETDATE(), NULL, @is_matrikulasi)";

                    var data = conn.Execute(query, simpan.SPUCalonMhs);

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

        public bool SimpanDataSPU(StoreSPUMhs simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryTambah = @"INSERT INTO [dbo].[SPU]
	                                    ([KD_CALON]
	                                    ,[SPU]
	                                    ,[USERNAME]
	                                    ,[TANGGAL]
                                        ,[TGL_CETAK]
	                                    ,[IS_MATRIKULASI])
                                    VALUES
                                        (@kd_calon, @spu, @username, GETDATE(), @tgl_cetak, NULL)";

                    string queryUbah = @"UPDATE [dbo].[SPU] SET 
                                            SPU = @spu,
                                            USERNAME = @username,
                                            TANGGAL = GETDATE(),
                                            TGL_CETAK = @tgl_cetak
                                        WHERE KD_CALON = @kd_calon";

                    string queryTambah2 = @"INSERT INTO [dbo].[angsuran_mhs]
	                                    ([kd_calon]
	                                    ,[angsuranke]
	                                    ,[jmluang]
                                        ,[tgl_buka]
                                        ,[batas_waktu]
                                        ,[ket_angsuran]
                                        ,[status]
                                        ,[is_jaminan]
                                        ,[jumlah]
                                        ,[potongan]
                                        ,[sks])
                                    VALUES
                                        (@kode_calon, @angsuran_ke, @jmlUang, @tgl_buka, @batas_waktu, @ket_ags, 1, @is_jaminan, @jumlah, @ptg, @sks)";

                    string queryUbah2 = @"UPDATE [dbo].[angsuran_mhs] SET
                                            angsuranke = @angsuran_ke,
                                            jmluang = @jmlUang,
                                            tgl_buka = @tgl_buka,
                                            batas_waktu = @batas_waktu,
                                            ket_angsuran = @ket_ags,
                                            status = 1,
                                            is_jaminan = @is_jaminan,
                                            jumlah = @jumlah,
                                            potongan = @ptg,
                                            sks = @sks,
                                        WHERE kd_calon = @kode_calon";

                    string queryTagihan = @"SELECT TOP(1) biaya FROM tr_tarif WHERE id_prodi = @id_prodi and id_tagihan = @id_tagihan and thmasuk = @thn_masuk and iscurrent = '1'";

                    string querySKS = @"SELECT TOP(1) jumlah_pengali FROM tr_tarif WHERE id_prodi = @id_prodi and id_tagihan = @id_tagihan and thmasuk = @thn_masuk and iscurrent = '1'";

                    if (simpan.kode_calon_akhir.Equals("ubah"))
                    {
                        string query = queryUbah;
                        var param = new
                        {
                            kd_calon = simpan.kode_calon_awal,
                            spu = simpan.spu,
                            username = simpan.username,
                            tgl_cetak = simpan.tgl_cetak
                        };

                        var data = conn.Execute(query, param);

                        string query2 = @"SELECT kd_calon, angsuranke, ket_angsuran, potongan
                                        FROM angsuran_mhs WHERE ket_angsuran like '%SPU%' and kd_calon = @kd_calon_mhs and status = '1'";

                        var dataAgsMhs2 = conn.Query<AngsuranMhs2>(query2, new { kd_calon_mhs = simpan.kode_calon_awal }).AsList();
                        int total = simpan.spu;
                        if (dataAgsMhs2.Count() > 0)
                        {
                            for (int i = 0; i < dataAgsMhs2.Count(); i++)
                            {
                                total = total - dataAgsMhs2[i].potongan;
                                string persenHilang = new string(dataAgsMhs2[i].ket_angsuran.Where(char.IsDigit).ToArray());
                                int nilaiPersen;
                                if (int.TryParse(persenHilang, out nilaiPersen))
                                {
                                    total = (total * nilaiPersen) / 100;
                                }
                                string ubahAngsuranTidakAktif = @"UPDATE angsuran_mhs SET
                                                                    jmluang = @total_uang,
                                                                    jumlah = @jumlah
                                                                WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                var paramTidakAktif = new
                                {
                                    total_uang = total,
                                    jumlah = simpan.spu,
                                    kd_calon_mhs = simpan.kode_calon_awal,
                                    angsuranke = dataAgsMhs2[i].angsuranke,
                                    ket_angsuran = dataAgsMhs2[i].ket_angsuran,
                                };
                                var ubahAngsuranMhs = conn.Execute(ubahAngsuranTidakAktif, paramTidakAktif);
                                total = simpan.spu;
                                continue;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        int SPU = 0;
                        string kd_calon_mhs = "";
                        string thnmasuk = "";
                        int id_rumus = 0;
                        int id_tagihan_mhs = 0;

                        string GetKdCalon = @"SELECT kd_calon FROM mhs_pendaftar WHERE masuk = @id_prodi and kd_calon BETWEEN @kd_awal AND @kd_akhir ORDER BY kd_calon ASC";

                        var KdCalon = conn.Query<string>(GetKdCalon, new { id_prodi = simpan.id_prodi, kd_awal = simpan.kode_calon_awal, kd_akhir = simpan.kode_calon_akhir }).AsList();

                        if (KdCalon.Count() > 0)
                        {
                            for (int i = 0; i < KdCalon.Count(); i++)
                            {
                                kd_calon_mhs = KdCalon[i];

                                string GetTA = @"SELECT TOP(1) thnakademik FROM mhs_pendaftar WHERE kd_calon = @kd_calon_mhs"; 

                                var TA = conn.QueryFirstOrDefault<string>(GetTA, new { kd_calon_mhs = kd_calon_mhs });

                                if (!String.IsNullOrEmpty(TA))
                                {
                                    string GetRmsAgs = @"SELECT TOP(1) id_rumus FROM mst_angsuran WHERE kd_jalur = @kd_jalur and thnakademik = @ta";

                                    var IdRumus = conn.QueryFirstOrDefault<int>(GetRmsAgs, new { kd_jalur = simpan.kd_jalur, ta = TA });

                                    if (IdRumus > 0)
                                    {
                                        id_rumus = IdRumus;
                                        string GetIdTagihan = @"SELECT TOP(1) id_tagihan FROM detail_rumus_angsuran WHERE id_rumus = @id_rumus and id_tagihan BETWEEN 13 and 16";

                                        var IdTagihan = conn.QueryFirstOrDefault<int>(GetIdTagihan, new { id_rumus = IdRumus });

                                        if (IdTagihan > 0)
                                        {
                                            id_tagihan_mhs = IdTagihan;
                                            string GetSPU = queryTagihan;

                                            string[] parts = TA.Split(new char[] { '/' });
                                            thnmasuk = parts[0];
                                            var data_SPU = conn.QueryFirstOrDefault<int>(GetSPU, new { id_prodi = simpan.id_prodi, id_tagihan = IdTagihan, thn_masuk = int.Parse(thnmasuk) });

                                            SPU = data_SPU;
                                        }
                                    }
                                }

                                string GetAllAgs = @"SELECT 
                                                        a.id_tagihan,
                                                        b.nama_tagihan,
                                                        angsuran_ke, 
                                                        CAST(prosentase as int) presentase, 
                                                        tgl_buka, 
                                                        tgl_tutup, 
                                                        is_jaminan 
                                                    FROM detail_rumus_angsuran a
                                                     JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan
                                                    WHERE id_rumus = @id_rumus
                                                    ORDER BY id_tagihan asc";

                                var dataAgs = conn.Query<DetailRmsAgs>(GetAllAgs, new { id_rumus = id_rumus }).AsList();
                               

                                string queryAngsuranMhs2 = @"SELECT kd_calon, angsuranke, ket_angsuran, potongan FROM angsuran_mhs WHERE kd_calon = @kd_calon_mhs and status = '1'";

                                var dataAgsMhs2 = conn.Query<AngsuranMhs2>(queryAngsuranMhs2, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                if(dataAgsMhs2.Count() > 0)
                                {
                                    for(int l = 0; l < dataAgsMhs2.Count(); l++)
                                    {
                                        //                  string ubahAngsuranTidakAktif = @"UPDATE angsuran_mhs SET 
                                        //                                                      status = 0,
                                        //ket_angsuran = @ket_angsuran_ubah
                                        //                                                  WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                        string ubahAngsuranTidakAktif = @"DELETE FROM angsuran_mhs
                                                                        WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                        var paramTidakAktif = new
                                        {
                                            kd_calon_mhs = kd_calon_mhs,
                                            angsuranke = dataAgsMhs2[l].angsuranke,
                                            ket_angsuran = dataAgsMhs2[l].ket_angsuran,
                                        };
                                        var ubahAngsuranMhs = conn.Execute(ubahAngsuranTidakAktif, paramTidakAktif);
                                        continue;
                                    }
                                }

                                string queryAngsuranMhs = @"SELECT * FROM angsuran_mhs WHERE kd_calon = @kd_calon_mhs and status = '1'";


                                var dataAgsMhs = conn.Query<dynamic>(queryAngsuranMhs, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                if (dataAgsMhs.Count() < 1)
                                {
                                    if (dataAgs.Count > 0)
                                    {
                                        for (int j = 0; j < dataAgs.Count(); j++)
                                        {
                                            int total = 0;
                                            int sks = 0;
                                            string GetBiaya = queryTagihan;

                                            string[] parts = TA.Split(new char[] { '/' });
                                            thnmasuk = parts[0];
                                            if (dataAgs[j].id_tagihan == 10|| dataAgs[j].id_tagihan == 3)
                                            {
                                                string GetSks = querySKS;
                                                var data_Sks = conn.QueryFirstOrDefault<int>(GetSks, new { id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan, thn_masuk = int.Parse(thnmasuk) });
                                                sks = data_Sks;

                                            }
                                            var data_Biaya = conn.QueryFirstOrDefault<int>(GetBiaya, new { id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan, thn_masuk = int.Parse(thnmasuk) });

                                            total = data_Biaya;
                                            if (dataAgs[j].id_tagihan == 3 || dataAgs[j].id_tagihan == 10)
                                            {                                          
                                                total = total * sks;
                                            }
                                            string GetAllPtg = @"SELECT TOP(1) nominal 
                                                                FROM detail_rumus_potongan 
                                                                WHERE id_rumus = @id_rumus and id_prodi = @id_prodi and id_tagihan = @id_tagihan";
                                            var data_Ptg = conn.QueryFirstOrDefault<int>(GetAllPtg, new { id_rumus = id_rumus, id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan });
                                            total = total - data_Ptg;
                                            if (dataAgs[j].presentase == 100)
                                            {
                                                total = total * 1;
                                            }
                                            else
                                            {
                                                total = (total * dataAgs[j].presentase)/ 100;
                                            }
                                            var param2 = new
                                            {
                                                kode_calon = kd_calon_mhs,
                                                angsuran_ke = dataAgs[j].angsuran_ke,
                                                jmlUang = total,
                                                tgl_buka = dataAgs[j].tgl_buka,
                                                batas_waktu = dataAgs[j].tgl_tutup,
                                                ket_ags = dataAgs[j].presentase + "% " + dataAgs[j].nama_tagihan,
                                                is_jaminan = dataAgs[j].is_jaminan,
                                                jumlah = data_Biaya,
                                                ptg = data_Ptg,
                                                sks = sks,
                                            };

                                            var data2 = conn.Execute(queryTambah2, param2);
                                            continue;
                                        }
                                    }
                                }
                                
                                string queryCekCalon = @"SELECT * FROM SPU WHERE kd_calon = @kd_calon_mhs";

                                var CalonMhs = conn.Query<dynamic>(queryCekCalon, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                string query = @"";
                                if (CalonMhs.Count > 0)
                                {
                                    query = query + queryUbah;
                                }
                                else
                                {
                                    query = query + queryTambah;
                                }

                                var param = new
                                {
                                    kd_calon = kd_calon_mhs,
                                    spu = SPU,
                                    username = simpan.username,
                                    tgl_cetak = simpan.tgl_cetak
                                };

                                var data = conn.Execute(query, param);
                            }

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

        public SPUMhs DetailSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) S.[KD_CALON]
                                        ,P.[NM_CALON] AS nama_calon
                                        ,P.[THNAKADEMIK]
                                        ,P.[MASUK]
                                        ,P.[periode]
                                        ,S.[SPU]
                                        ,S.[USERNAME]
                                        ,CONVERT(VARCHAR,S.[TANGGAL],23) AS TANGGAL
                                        ,CONVERT(VARCHAR,S.[TGL_CETAK],23) AS TGL_CETAK
                                        ,S.[IS_MATRIKULASI]
                                        FROM [dbo].[SPU] S
                                        JOIN [dbo].[MHS_PENDAFTAR] P ON P.[KD_CALON] = S.[KD_CALON]
                                     WHERE S.[KD_CALON] = @kd_calon";

                    var param = new { kd_calon = kd_calon };
                    var data = conn.QueryFirstOrDefault<SPUMhs>(query, param);

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

        public bool UbahSPU(SPUMhsView spuMhs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE [Mission].[dbo].[SPU] SET
                                        [SPU] = @spu
	                                    ,[USERNAME] = @username
	                                    ,[IS_MATRIKULASI] = @is_matrikulasi
                                     WHERE [KD_CALON] = @kd_calon";

                    var data = conn.Execute(query, spuMhs.SPUCalonMhs);

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

        public bool HapusSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[SPU] WHERE KD_CALON = @kd_calon";

                    var data = conn.Execute(query, new { kd_calon = kd_calon });

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

        // Tabel Pendaftar
        public List<Pendaftar> GetPendaftar()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (5000) MHS.[KD_CALON], MHS.[NM_CALON]
                                         FROM [dbo].[MHS_PENDAFTAR] MHS
                                         WHERE MHS.[KD_CALON] NOT IN
                                            (SELECT SPU.[KD_CALON] 
                                             FROM [dbo].[SPU] SPU)
                                         ORDER BY CONVERT(INT,MHS.[KD_CALON]) DESC";

                    //WHERE [KD_CALON] = '22334455'";
                    var data = conn.Query<Pendaftar>(query).AsList();

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

        public SPUMhs GetSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_prodi VARCHAR(10), @thnakademik VARCHAR(25)
                                     SET @id_prodi = (SELECT [MASUK] FROM [dbo].[MHS_PENDAFTAR] WHERE [KD_CALON] = @kd_calon)
                                     SET @thnakademik = (SELECT [THNAKADEMIK] FROM [dbo].[MHS_PENDAFTAR] WHERE [KD_CALON] = @kd_calon)

                                     SELECT S.[NOMINAL] AS SPU FROM [dbo].[TBL_SKPU] S
                                     LEFT JOIN [dbo].[MHS_PENDAFTAR] M
                                     ON S.[THNAKADEMIK] = M.[THNAKADEMIK]
                                     WHERE S.[ID_TAGIHAN] = 1 AND M.[MASUK] = @id_prodi AND S.[THNAKADEMIK] = @thnakademik";

                    var data = conn.QueryFirstOrDefault<SPUMhs>(query, new { kd_calon = kd_calon });

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
