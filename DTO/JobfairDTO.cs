using System;
using System.Collections.Generic;

namespace vocafind_api.DTO
{
    public class JobfairDTO
    {
        public ulong Id { get; set; }
        public string NamaAcara { get; set; } = null!;
        public string? AcaraBkk { get; set; }
        public DateOnly TanggalAwalPendaftaranAcara { get; set; }
        public DateOnly TanggalAkhirPendaftaranAcara { get; set; }
        public DateOnly TanggalMulaiAcara { get; set; }
        public DateOnly TanggalSelesaiAcara { get; set; }

        public int MaxCapacity { get; set; }
        public int TotalLowongan { get; set; }
        public int TotalPerusahaan { get; set; }

        // Info admin vokasi
        public string NamaAdminVokasi { get; set; } = null!;

        // Flyer acara (ambil yang pertama jika ada)
        public string? FlyerUrl { get; set; }
    }

    public class JobfairDetailDTO
    {
        public ulong Id { get; set; }
        public string NamaAcara { get; set; } = null!;
        public string? AcaraBkk { get; set; }
        public string? AlamatAcara { get; set; }
        public string? Provinsi { get; set; }
        public string? Kabupaten { get; set; }
        public string? Lokasi { get; set; }
        public DateOnly TanggalAwalPendaftaranAcara { get; set; }
        public DateOnly TanggalAkhirPendaftaranAcara { get; set; }
        public DateOnly TanggalMulaiAcara { get; set; }
        public DateOnly TanggalSelesaiAcara { get; set; }
        public string? Deskripsi { get; set; }
        public string Status { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Perusahaan yang ikut
        public List<CompanyJobfairDTO> Perusahaan { get; set; } = new();

        // Flyer acara
        public List<FlyerAcaraDTO> FlyerAcara { get; set; } = new();

        // Lowongan yang tersedia di acara ini
        public List<LowonganAcaraDTO> LowonganAcara { get; set; } = new();
    }

    public class CompanyJobfairDTO
    {
        public string NamaPerusahaan { get; set; } = null!;
        public string? Logo { get; set; }

    }

    public class FlyerAcaraDTO
    {
        public string FlyerUrl { get; set; } = null!;
    }

    public class FlyerAcaraRequestDTO
    {
        public IFormFile FlyerImage { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    public class FlyerAcaraResponseDTO
    {
        public ulong Id { get; set; }
        public string FlyerUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }


    // DTO/JobfairDTO.cs - Update LowonganAcaraDTO
    public class LowonganAcaraDTO
    {
        public string LowonganId { get; set; } = null!;
        public string Posisi { get; set; } = null!;
        public string DeskripsiPekerjaan { get; set; } = null!;
        public string? MinimalLulusan { get; set; }
        public string Status { get; set; } = null!;
        public string Lokasi { get; set; } = null!;
        public string Gaji { get; set; } = null!;
        public string JenisPekerjaan { get; set; } = null!;
        public DateOnly TanggalPosting { get; set; }
        public DateOnly BatasLamaran { get; set; }
        public int BatasPelamar { get; set; }
        public int JumlahPelamar { get; set; }
        public string TingkatPengalaman { get; set; } = null!;
        public bool OpsiKerjaRemote { get; set; }
        public string KontrakDurasi { get; set; } = null!;
        public string PeluangKarir { get; set; } = null!;

        // Perusahaan info (gunakan Logo saja, bukan LogoPerusahaan)
        public string NamaPerusahaan { get; set; } = null!;
        public string? Logo { get; set; }
    }
}