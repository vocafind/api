using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace vocafind_api.Models;

public partial class TalentcerdasContext : DbContext
{
    public TalentcerdasContext()
    {
    }

    public TalentcerdasContext(DbContextOptions<TalentcerdasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcaraInterviewSlot> AcaraInterviewSlots { get; set; }

    public virtual DbSet<AcaraJobfair> AcaraJobfairs { get; set; }

    public virtual DbSet<AcaraJobfairCompany> AcaraJobfairCompanies { get; set; }

    public virtual DbSet<AdditionalSkill> AdditionalSkills { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminCompany> AdminCompanies { get; set; }

    public virtual DbSet<AdminDisnaker> AdminDisnakers { get; set; }

    public virtual DbSet<AdminSecurity> AdminSecurities { get; set; }

    public virtual DbSet<AdminVokasi> AdminVokasis { get; set; }

    public virtual DbSet<AlumniVokasi> AlumniVokasis { get; set; }

    public virtual DbSet<ApplyAcara> ApplyAcaras { get; set; }

    public virtual DbSet<Award> Awards { get; set; }

    public virtual DbSet<Cache> Caches { get; set; }

    public virtual DbSet<CacheLock> CacheLocks { get; set; }

    public virtual DbSet<CareerInterest> CareerInterests { get; set; }

    public virtual DbSet<CareerPath> CareerPaths { get; set; }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyBranch> CompanyBranches { get; set; }

    public virtual DbSet<CompanyFacility> CompanyFacilities { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<EmployeeTestimonial> EmployeeTestimonials { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<FailedJob> FailedJobs { get; set; }

    public virtual DbSet<FlyerAcara> FlyerAcaras { get; set; }

    public virtual DbSet<Hobby> Hobbies { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobAdditionalFacility> JobAdditionalFacilities { get; set; }

    public virtual DbSet<JobAdditionalRequirement> JobAdditionalRequirements { get; set; }

    public virtual DbSet<JobApply> JobApplies { get; set; }

    public virtual DbSet<JobBatch> JobBatches { get; set; }

    public virtual DbSet<JobBenefit> JobBenefits { get; set; }

    public virtual DbSet<JobQualification> JobQualifications { get; set; }

    public virtual DbSet<JobVacancy> JobVacancies { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LowonganAcara> LowonganAcaras { get; set; }

    public virtual DbSet<LowonganPekerjaanAcara> LowonganPekerjaanAcaras { get; set; }

    public virtual DbSet<Migration> Migrations { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PersonalAccessToken> PersonalAccessTokens { get; set; }

    public virtual DbSet<Portofolio> Portofolios { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Social> Socials { get; set; }

    public virtual DbSet<SoftSkill> SoftSkills { get; set; }

    public virtual DbSet<Talent> Talents { get; set; }

    public virtual DbSet<TalentAcaraJobApplication> TalentAcaraJobApplications { get; set; }

    public virtual DbSet<TalentAcaraRegistration> TalentAcaraRegistrations { get; set; }

    public virtual DbSet<TalentInterviewAttendance> TalentInterviewAttendances { get; set; }

    public virtual DbSet<TalentReference> TalentReferences { get; set; }

    public virtual DbSet<Training> Trainings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkHistory> WorkHistories { get; set; }

    // ✅ TAMBAHKAN INI
    public DbSet<LoginAttempt> LoginAttempts { get; set; }
    public DbSet<BlockedIp> BlockedIps { get; set; }
    public DbSet<SavedJob> SavedJob { get; set; }
    public DbSet<AcaraQr> AcaraQr { get; set; }



    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
         => optionsBuilder.UseMySql("server=localhost;port=3306;database=talentcerdas;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AcaraInterviewSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("acara_interview_slots")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "acara_interview_slots_acara_jobfair_id_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Slot)
                .HasMaxLength(255)
                .HasColumnName("slot");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.AcaraInterviewSlots)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("acara_interview_slots_acara_jobfair_id_foreign");
        });

        modelBuilder.Entity<AcaraJobfair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("acara_jobfair")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminVokasiId, "acara_jobfair_adminvokasiid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraBkk)
                .HasMaxLength(255)
                .HasColumnName("acara_bkk");
            entity.Property(e => e.AdminVokasiId).HasColumnName("adminVokasiID");
            entity.Property(e => e.AlamatAcara)
                .HasMaxLength(255)
                .HasColumnName("alamat_acara");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentCapacity).HasColumnName("current_capacity");
            entity.Property(e => e.Deskripsi)
                .HasColumnType("text")
                .HasColumnName("deskripsi");
            entity.Property(e => e.Kabupaten)
                .HasMaxLength(255)
                .HasColumnName("kabupaten");
            entity.Property(e => e.Lokasi)
                .HasMaxLength(255)
                .HasColumnName("lokasi");
            entity.Property(e => e.MaxCapacity)
                .HasDefaultValueSql("'1000'")
                .HasColumnName("max_capacity");
            entity.Property(e => e.NamaAcara)
                .HasMaxLength(255)
                .HasColumnName("nama_acara");
            entity.Property(e => e.Provinsi)
                .HasMaxLength(255)
                .HasColumnName("provinsi");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'aktif'")
                .HasColumnName("status");
            entity.Property(e => e.TanggalAkhirPendaftaranAcara).HasColumnName("tanggal_akhir_pendaftaran_acara");
            entity.Property(e => e.TanggalAwalPendaftaranAcara).HasColumnName("tanggal_awal_pendaftaran_acara");
            entity.Property(e => e.TanggalMulaiAcara).HasColumnName("tanggal_mulai_acara");
            entity.Property(e => e.TanggalSelesaiAcara).HasColumnName("tanggal_selesai_acara");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AdminVokasi).WithMany(p => p.AcaraJobfairs)
                .HasForeignKey(d => d.AdminVokasiId)
                .HasConstraintName("acara_jobfair_adminvokasiid_foreign");
        });

        modelBuilder.Entity<AcaraJobfairCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("acara_jobfair_companies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.AcaraJobfairId, e.CompanyId }, "acara_jobfair_companies_acara_jobfair_id_companyid_unique").IsUnique();

            entity.HasIndex(e => e.CompanyId, "acara_jobfair_companies_companyid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.AcaraJobfairCompanies)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("acara_jobfair_companies_acara_jobfair_id_foreign");

            entity.HasOne(d => d.Company).WithMany(p => p.AcaraJobfairCompanies)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("acara_jobfair_companies_companyid_foreign");
        });

        modelBuilder.Entity<AdditionalSkill>(entity =>
        {
            entity.HasKey(e => e.AdditionalskillId).HasName("PRIMARY");

            entity
                .ToTable("additional_skills")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "additional_skills_talentid_foreign");

            entity.Property(e => e.AdditionalskillId).HasColumnName("additionalskillID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.NamaSkill)
                .HasMaxLength(255)
                .HasColumnName("nama_skill");
            entity.Property(e => e.Profisiensi)
                .HasMaxLength(255)
                .HasColumnName("profisiensi");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.AdditionalSkills)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("additional_skills_talentid_foreign");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PRIMARY");

            entity
                .ToTable("admins")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Username, "admins_username_unique").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.HakAkses)
                .HasMaxLength(255)
                .HasColumnName("hakAkses");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        modelBuilder.Entity<AdminCompany>(entity =>
        {
            entity.HasKey(e => e.AdminCompanyId).HasName("PRIMARY");

            entity
                .ToTable("admin_companies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "admin_companies_adminid_foreign");

            entity.HasIndex(e => e.CompanyId, "admin_companies_companyid_foreign");

            entity.Property(e => e.AdminCompanyId).HasColumnName("adminCompanyID");
            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminCompanies)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("admin_companies_adminid_foreign");

            entity.HasOne(d => d.Company).WithMany(p => p.AdminCompanies)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("admin_companies_companyid_foreign");
        });

        modelBuilder.Entity<AdminDisnaker>(entity =>
        {
            entity.HasKey(e => e.AdminDisnakerId).HasName("PRIMARY");

            entity
                .ToTable("admin_disnakers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "admin_disnakers_adminid_foreign");

            entity.HasIndex(e => e.Nik, "admin_disnakers_nik_unique").IsUnique();

            entity.Property(e => e.AdminDisnakerId).HasColumnName("adminDisnakerID");
            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Nik).HasColumnName("nik");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminDisnakers)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("admin_disnakers_adminid_foreign");
        });

        modelBuilder.Entity<AdminSecurity>(entity =>
        {
            entity.HasKey(e => e.AdminSecurityId).HasName("PRIMARY");

            entity
                .ToTable("admin_securities")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "admin_securities_acara_jobfair_id_foreign");

            entity.HasIndex(e => new { e.AdminVokasiId, e.AcaraJobfairId }, "admin_securities_adminvokasiid_acara_jobfair_id_index");

            entity.HasIndex(e => e.IsActive, "admin_securities_is_active_index");

            entity.HasIndex(e => e.Nim, "admin_securities_nim_unique").IsUnique();

            entity.Property(e => e.AdminSecurityId).HasColumnName("adminSecurityID");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.AdminVokasiId).HasColumnName("adminVokasiID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_login_at");
            entity.Property(e => e.NamaLengkap)
                .HasMaxLength(255)
                .HasColumnName("nama_lengkap");
            entity.Property(e => e.Nim)
                .HasMaxLength(20)
                .HasColumnName("nim");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.AdminSecurities)
                .HasForeignKey(d => d.AcaraJobfairId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_securities_acara_jobfair_id_foreign");

            entity.HasOne(d => d.AdminVokasi).WithMany(p => p.AdminSecurities)
                .HasForeignKey(d => d.AdminVokasiId)
                .HasConstraintName("admin_securities_adminvokasiid_foreign");
        });

        modelBuilder.Entity<AdminVokasi>(entity =>
        {
            entity.HasKey(e => e.AdminVokasiId).HasName("PRIMARY");

            entity
                .ToTable("admin_vokasi")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "admin_vokasi_adminid_foreign");

            entity.Property(e => e.AdminVokasiId).HasColumnName("adminVokasiID");
            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Pt)
                .HasMaxLength(255)
                .HasColumnName("pt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminVokasis)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("admin_vokasi_adminid_foreign");
        });

        modelBuilder.Entity<AlumniVokasi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("alumni_vokasi")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminVokasiId, "alumni_vokasi_adminvokasiid_foreign");

            entity.HasIndex(e => e.TalentId, "alumni_vokasi_talentid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminVokasiId).HasColumnName("adminVokasiID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AdminVokasi).WithMany(p => p.AlumniVokasis)
                .HasForeignKey(d => d.AdminVokasiId)
                .HasConstraintName("alumni_vokasi_adminvokasiid_foreign");

            entity.HasOne(d => d.Talent).WithMany(p => p.AlumniVokasis)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("alumni_vokasi_talentid_foreign");
        });

        modelBuilder.Entity<ApplyAcara>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("apply_acara")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "apply_acara_acara_jobfair_id_foreign");

            entity.HasIndex(e => e.ApplyId, "apply_acara_applyid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.ApplyId).HasColumnName("applyID");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.ApplyAcaras)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("apply_acara_acara_jobfair_id_foreign");

            entity.HasOne(d => d.Apply).WithMany(p => p.ApplyAcaras)
                .HasForeignKey(d => d.ApplyId)
                .HasConstraintName("apply_acara_applyid_foreign");
        });

        modelBuilder.Entity<Award>(entity =>
        {
            entity.HasKey(e => e.AwardId).HasName("PRIMARY");

            entity
                .ToTable("awards")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "awards_talentid_foreign");

            entity.Property(e => e.AwardId).HasColumnName("awardID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.NamaPenghargaan)
                .HasMaxLength(255)
                .HasColumnName("nama_penghargaan");
            entity.Property(e => e.PemberiPenghargaan)
                .HasMaxLength(255)
                .HasColumnName("pemberi_penghargaan");
            entity.Property(e => e.Sertifikat)
                .HasMaxLength(255)
                .HasColumnName("sertifikat");
            entity.Property(e => e.Tahun).HasColumnName("tahun");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TingkatPenghargaan)
                .HasMaxLength(255)
                .HasColumnName("tingkat_penghargaan");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Awards)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("awards_talentid_foreign");
        });

        modelBuilder.Entity<Cache>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PRIMARY");

            entity
                .ToTable("cache")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Expiration).HasColumnName("expiration");
            entity.Property(e => e.Value)
                .HasColumnType("mediumtext")
                .HasColumnName("value");
        });

        modelBuilder.Entity<CacheLock>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PRIMARY");

            entity
                .ToTable("cache_locks")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Expiration).HasColumnName("expiration");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
        });

        modelBuilder.Entity<CareerInterest>(entity =>
        {
            entity.HasKey(e => e.CareerinterestId).HasName("PRIMARY");

            entity
                .ToTable("career_interests")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "career_interests_talentid_foreign");

            entity.Property(e => e.CareerinterestId).HasColumnName("careerinterestID");
            entity.Property(e => e.Alasan)
                .HasMaxLength(255)
                .HasColumnName("alasan");
            entity.Property(e => e.BidangKetertarikan)
                .HasMaxLength(255)
                .HasColumnName("bidang_ketertarikan");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TingkatKetertarikan)
                .HasMaxLength(255)
                .HasColumnName("tingkat_ketertarikan");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.CareerInterests)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("career_interests_talentid_foreign");
        });

        modelBuilder.Entity<CareerPath>(entity =>
        {
            entity.HasKey(e => e.CareerpathId).HasName("PRIMARY");

            entity
                .ToTable("career_paths")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "career_paths_companyid_foreign");

            entity.Property(e => e.CareerpathId).HasColumnName("careerpathID");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasColumnType("text")
                .HasColumnName("deskripsi");
            entity.Property(e => e.JalurKarir)
                .HasMaxLength(255)
                .HasColumnName("jalur_karir");
            entity.Property(e => e.Posisi)
                .HasMaxLength(255)
                .HasColumnName("posisi");
            entity.Property(e => e.Tingkatan).HasColumnName("tingkatan");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.CareerPaths)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("career_paths_companyid_foreign");
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PRIMARY");

            entity
                .ToTable("certifications")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "certifications_talentid_foreign");

            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LembagaSertifikasi)
                .HasMaxLength(255)
                .HasColumnName("lembaga_sertifikasi");
            entity.Property(e => e.NamaSertifikasi)
                .HasMaxLength(255)
                .HasColumnName("nama_sertifikasi");
            entity.Property(e => e.NomorSertifikat)
                .HasMaxLength(255)
                .HasColumnName("nomor_sertifikat");
            entity.Property(e => e.Sertifikat)
                .HasMaxLength(255)
                .HasColumnName("sertifikat");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TanggalHabisMasa).HasColumnName("tanggal_habis_masa");
            entity.Property(e => e.TanggalTerbit).HasColumnName("tanggal_terbit");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("certifications_talentid_foreign");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PRIMARY");

            entity
                .ToTable("companies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.Alamat)
                .HasMaxLength(255)
                .HasColumnName("alamat");
            entity.Property(e => e.BidangUsaha)
                .HasMaxLength(255)
                .HasColumnName("bidangUsaha");
            entity.Property(e => e.BudayaPerusahaan)
                .HasColumnType("text")
                .HasColumnName("budayaPerusahaan");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeskripsiPerusahaan)
                .HasColumnType("text")
                .HasColumnName("deskripsiPerusahaan");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.JumlahKaryawan).HasColumnName("jumlahKaryawan");
            entity.Property(e => e.JumlahProyekBerjalan).HasColumnName("jumlahProyekBerjalan");
            entity.Property(e => e.KebijakanKerja)
                .HasColumnType("text")
                .HasColumnName("kebijakanKerja");
            entity.Property(e => e.Kota)
                .HasMaxLength(255)
                .HasColumnName("kota");
            entity.Property(e => e.Logo)
                .HasMaxLength(255)
                .HasColumnName("logo");
            entity.Property(e => e.NamaPerusahaan)
                .HasMaxLength(255)
                .HasColumnName("namaPerusahaan");
            entity.Property(e => e.Nib)
                .HasMaxLength(255)
                .HasColumnName("nib");
            entity.Property(e => e.NomorTelepon)
                .HasMaxLength(255)
                .HasColumnName("nomorTelepon");
            entity.Property(e => e.Npwp)
                .HasMaxLength(255)
                .HasColumnName("npwp");
            entity.Property(e => e.Provinsi)
                .HasMaxLength(255)
                .HasColumnName("provinsi");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<CompanyBranch>(entity =>
        {
            entity.HasKey(e => e.CompanybranchId).HasName("PRIMARY");

            entity
                .ToTable("company_branches")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "company_branches_companyid_foreign");

            entity.Property(e => e.CompanybranchId).HasColumnName("companybranchID");
            entity.Property(e => e.Alamat)
                .HasMaxLength(255)
                .HasColumnName("alamat");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.NamaCabang)
                .HasMaxLength(255)
                .HasColumnName("namaCabang");
            entity.Property(e => e.NomorTelepon)
                .HasMaxLength(255)
                .HasColumnName("nomorTelepon");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyBranches)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("company_branches_companyid_foreign");
        });

        modelBuilder.Entity<CompanyFacility>(entity =>
        {
            entity.HasKey(e => e.CompanyfacilityId).HasName("PRIMARY");

            entity
                .ToTable("company_facilities")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "company_facilities_companyid_foreign");

            entity.Property(e => e.CompanyfacilityId).HasColumnName("companyfacilityID");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.GaleriFoto)
                .HasMaxLength(255)
                .HasColumnName("galeri_foto");
            entity.Property(e => e.NamaFasilitas)
                .HasMaxLength(255)
                .HasColumnName("nama_fasilitas");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyFacilities)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("company_facilities_companyid_foreign");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PRIMARY");

            entity
                .ToTable("educations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "educations_talentid_foreign");

            entity.Property(e => e.EducationId).HasColumnName("educationID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Gelar)
                .HasMaxLength(255)
                .HasColumnName("gelar");
            entity.Property(e => e.Institusi)
                .HasMaxLength(255)
                .HasColumnName("institusi");
            entity.Property(e => e.Jenjang)
                .HasColumnType("enum('SD','SMP','SMA','SMK','D1','D2','D3','D4','S1','S2','S3')")
                .HasColumnName("jenjang");
            entity.Property(e => e.Jurusan)
                .HasMaxLength(255)
                .HasColumnName("jurusan");
            entity.Property(e => e.NilaiAkhir)
                .HasPrecision(5, 2)
                .HasColumnName("nilai_akhir");
            entity.Property(e => e.TahunLulus).HasColumnName("tahun_lulus");
            entity.Property(e => e.TahunMasuk).HasColumnName("tahun_masuk");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Educations)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("educations_talentid_foreign");
        });

        modelBuilder.Entity<EmployeeTestimonial>(entity =>
        {
            entity.HasKey(e => e.EmployeetestimonialsId).HasName("PRIMARY");

            entity
                .ToTable("employee_testimonials")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "employee_testimonials_companyid_foreign");

            entity.Property(e => e.EmployeetestimonialsId).HasColumnName("employeetestimonialsID");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .HasColumnName("employeeName");
            entity.Property(e => e.PositionTitle)
                .HasMaxLength(255)
                .HasColumnName("positionTitle");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Testimoni)
                .HasColumnType("text")
                .HasColumnName("testimoni");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.EmployeeTestimonials)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("employee_testimonials_companyid_foreign");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PRIMARY");

            entity
                .ToTable("experiences")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "experiences_talentid_foreign");

            entity.Property(e => e.ExperienceId).HasColumnName("experienceID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Pengalaman)
                .HasMaxLength(255)
                .HasColumnName("pengalaman");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("experiences_talentid_foreign");
        });

        modelBuilder.Entity<FailedJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("failed_jobs")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Uuid, "failed_jobs_uuid_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Connection)
                .HasColumnType("text")
                .HasColumnName("connection");
            entity.Property(e => e.Exception).HasColumnName("exception");
            entity.Property(e => e.FailedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("failed_at");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.Queue)
                .HasColumnType("text")
                .HasColumnName("queue");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<FlyerAcara>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("flyer_acara")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "flyer_acara_acara_jobfair_id_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.FlyerUrl)
                .HasMaxLength(255)
                .HasColumnName("flyer_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.FlyerAcaras)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("flyer_acara_acara_jobfair_id_foreign");
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.HasKey(e => e.HobbyId).HasName("PRIMARY");

            entity
                .ToTable("hobbies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "hobbies_talentid_foreign");

            entity.Property(e => e.HobbyId).HasColumnName("hobbyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.NamaHobi)
                .HasMaxLength(255)
                .HasColumnName("nama_hobi");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Hobbies)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("hobbies_talentid_foreign");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("jobs")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Queue, "jobs_queue_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attempts).HasColumnName("attempts");
            entity.Property(e => e.AvailableAt).HasColumnName("available_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.Queue).HasColumnName("queue");
            entity.Property(e => e.ReservedAt).HasColumnName("reserved_at");
        });

        modelBuilder.Entity<JobAdditionalFacility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PRIMARY");

            entity
                .ToTable("job_additional_facilities")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LowonganId, "job_additional_facilities_lowonganid_foreign");

            entity.Property(e => e.FacilityId).HasColumnName("facilityID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.FasilitasTambahan)
                .HasColumnType("text")
                .HasColumnName("fasilitas_tambahan");
            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lowongan).WithMany(p => p.JobAdditionalFacilities)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("job_additional_facilities_lowonganid_foreign");
        });

        modelBuilder.Entity<JobAdditionalRequirement>(entity =>
        {
            entity.HasKey(e => e.RequirementId).HasName("PRIMARY");

            entity
                .ToTable("job_additional_requirements")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LowonganId, "job_additional_requirements_lowonganid_foreign");

            entity.Property(e => e.RequirementId).HasColumnName("requirementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");
            entity.Property(e => e.PersyaratanTambahan)
                .HasColumnType("text")
                .HasColumnName("persyaratan_tambahan");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lowongan).WithMany(p => p.JobAdditionalRequirements)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("job_additional_requirements_lowonganid_foreign");
        });

        modelBuilder.Entity<JobApply>(entity =>
        {
            entity.HasKey(e => e.ApplyId).HasName("PRIMARY");

            entity
                .ToTable("job_apply")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ApplicationCode, "job_apply_application_code_unique").IsUnique();

            entity.HasIndex(e => e.InterviewSlot, "job_apply_interview_slot_foreign");

            entity.Property(e => e.ApplyId)
                .HasMaxLength(191)
                .HasColumnName("applyID");
            entity.Property(e => e.ApplicationCode).HasColumnName("application_code");
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("applied_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.InterviewSlot).HasColumnName("interview_slot");
            entity.Property(e => e.LowonganId)
                .HasMaxLength(255)
                .HasColumnName("lowonganID");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TalentId)
                .HasMaxLength(255)
                .HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.InterviewSlotNavigation).WithMany(p => p.JobApplies)
                .HasForeignKey(d => d.InterviewSlot)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("job_apply_interview_slot_foreign");
        });

        modelBuilder.Entity<JobBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("job_batches")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FailedJobIds).HasColumnName("failed_job_ids");
            entity.Property(e => e.FailedJobs).HasColumnName("failed_jobs");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Options)
                .HasColumnType("mediumtext")
                .HasColumnName("options");
            entity.Property(e => e.PendingJobs).HasColumnName("pending_jobs");
            entity.Property(e => e.TotalJobs).HasColumnName("total_jobs");
        });

        modelBuilder.Entity<JobBenefit>(entity =>
        {
            entity.HasKey(e => e.BenefitId).HasName("PRIMARY");

            entity
                .ToTable("job_benefits")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LowonganId, "job_benefits_lowonganid_foreign");

            entity.Property(e => e.BenefitId).HasColumnName("benefitID");
            entity.Property(e => e.Benefit)
                .HasMaxLength(255)
                .HasColumnName("benefit");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lowongan).WithMany(p => p.JobBenefits)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("job_benefits_lowonganid_foreign");
        });

        modelBuilder.Entity<JobQualification>(entity =>
        {
            entity.HasKey(e => e.QualificationId).HasName("PRIMARY");

            entity
                .ToTable("job_qualifications")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LowonganId, "job_qualifications_lowonganid_foreign");

            entity.Property(e => e.QualificationId).HasColumnName("qualificationID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Kualifikasi)
                .HasMaxLength(255)
                .HasColumnName("kualifikasi");
            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Lowongan).WithMany(p => p.JobQualifications)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("job_qualifications_lowonganid_foreign");
        });

        modelBuilder.Entity<JobVacancy>(entity =>
        {
            entity.HasKey(e => e.LowonganId).HasName("PRIMARY");

            entity
                .ToTable("job_vacancies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "job_vacancies_companyid_foreign");

            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");
            entity.Property(e => e.BatasLamaran).HasColumnName("batasLamaran");
            entity.Property(e => e.BatasPelamar).HasColumnName("batasPelamar");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeskripsiPekerjaan)
                .HasColumnType("text")
                .HasColumnName("deskripsiPekerjaan");
            entity.Property(e => e.Gaji)
                .HasMaxLength(255)
                .HasColumnName("gaji");
            entity.Property(e => e.JenisPekerjaan)
                .HasMaxLength(255)
                .HasColumnName("jenisPekerjaan");
            entity.Property(e => e.JumlahPelamar).HasColumnName("jumlahPelamar");
            entity.Property(e => e.KontrakDurasi)
                .HasMaxLength(255)
                .HasColumnName("kontrakDurasi");
            entity.Property(e => e.Lokasi)
                .HasMaxLength(255)
                .HasColumnName("lokasi");
            entity.Property(e => e.MinimalLulusan)
                .HasColumnType("enum('SMA','SMK','D1','D2','D3','D4','S1','S2','S3')")
                .HasColumnName("minimalLulusan");
            entity.Property(e => e.OpsiKerjaRemote).HasColumnName("opsiKerjaRemote");
            entity.Property(e => e.PeluangKarir)
                .HasMaxLength(255)
                .HasColumnName("peluangKarir");
            entity.Property(e => e.Posisi)
                .HasMaxLength(255)
                .HasColumnName("posisi");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TanggalPosting).HasColumnName("tanggalPosting");
            entity.Property(e => e.TingkatPengalaman)
                .HasMaxLength(255)
                .HasColumnName("tingkatPengalaman");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.JobVacancies)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("job_vacancies_companyid_foreign");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PRIMARY");

            entity
                .ToTable("languages")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "languages_talentid_foreign");

            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.NamaBahasa)
                .HasMaxLength(255)
                .HasColumnName("nama_bahasa");
            entity.Property(e => e.Profisiensi)
                .HasMaxLength(255)
                .HasColumnName("profisiensi");
            entity.Property(e => e.Sertifikat)
                .HasMaxLength(255)
                .HasColumnName("sertifikat");
            entity.Property(e => e.Skor)
                .HasMaxLength(255)
                .HasColumnName("skor");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Languages)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("languages_talentid_foreign");
        });

        modelBuilder.Entity<LowonganAcara>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("lowongan_acara")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "lowongan_acara_acara_jobfair_id_foreign");

            entity.HasIndex(e => e.LowonganId, "lowongan_acara_lowonganid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.LowonganId).HasColumnName("lowonganID");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.LowonganAcaras)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("lowongan_acara_acara_jobfair_id_foreign");

            entity.HasOne(d => d.Lowongan).WithMany(p => p.LowonganAcaras)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("lowongan_acara_lowonganid_foreign");
        });

        modelBuilder.Entity<LowonganPekerjaanAcara>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("lowongan_pekerjaan_acara")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "lowongan_pekerjaan_acara_acara_jobfair_id_foreign");

            entity.HasIndex(e => e.CompanyId, "lowongan_pekerjaan_acara_companyid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.BatasLamaran).HasColumnName("batas_lamaran");
            entity.Property(e => e.BatasPelamar).HasColumnName("batas_pelamar");
            entity.Property(e => e.CompanyId).HasColumnName("companyID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeskripsiPekerjaan)
                .HasColumnType("text")
                .HasColumnName("deskripsi_pekerjaan");
            entity.Property(e => e.Gaji)
                .HasMaxLength(100)
                .HasColumnName("gaji");
            entity.Property(e => e.JenisPekerjaan)
                .HasMaxLength(255)
                .HasColumnName("jenis_pekerjaan");
            entity.Property(e => e.JumlahPelamar).HasColumnName("jumlah_pelamar");
            entity.Property(e => e.KontrakDurasi)
                .HasMaxLength(255)
                .HasColumnName("kontrak_durasi");
            entity.Property(e => e.Lokasi)
                .HasMaxLength(255)
                .HasColumnName("lokasi");
            entity.Property(e => e.MinimalLulusan)
                .HasColumnType("enum('SMA','SMK','D1','D2','D3','D4','S1','S2','S3')")
                .HasColumnName("minimal_lulusan");
            entity.Property(e => e.OpsiKerjaRemote).HasColumnName("opsi_kerja_remote");
            entity.Property(e => e.PeluangKarir)
                .HasMaxLength(255)
                .HasColumnName("peluang_karir");
            entity.Property(e => e.Posisi)
                .HasMaxLength(255)
                .HasColumnName("posisi");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TanggalPosting).HasColumnName("tanggal_posting");
            entity.Property(e => e.TingkatPengalaman)
                .HasMaxLength(255)
                .HasColumnName("tingkat_pengalaman");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.LowonganPekerjaanAcaras)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("lowongan_pekerjaan_acara_acara_jobfair_id_foreign");

            entity.HasOne(d => d.Company).WithMany(p => p.LowonganPekerjaanAcaras)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("lowongan_pekerjaan_acara_companyid_foreign");
        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("migrations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Batch).HasColumnName("batch");
            entity.Property(e => e.Migration1)
                .HasMaxLength(255)
                .HasColumnName("migration");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PRIMARY");

            entity
                .ToTable("password_reset_tokens")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
        });

        modelBuilder.Entity<PersonalAccessToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("personal_access_tokens")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Token, "personal_access_tokens_token_unique").IsUnique();

            entity.HasIndex(e => new { e.TokenableType, e.TokenableId }, "personal_access_tokens_tokenable_type_tokenable_id_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Abilities)
                .HasColumnType("text")
                .HasColumnName("abilities");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp")
                .HasColumnName("expires_at");
            entity.Property(e => e.LastUsedAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_used_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Token)
                .HasMaxLength(64)
                .HasColumnName("token");
            entity.Property(e => e.TokenableId).HasColumnName("tokenable_id");
            entity.Property(e => e.TokenableType).HasColumnName("tokenable_type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Portofolio>(entity =>
        {
            entity.HasKey(e => e.PortfolioId).HasName("PRIMARY");

            entity
                .ToTable("portofolios")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "portofolios_talentid_foreign");

            entity.Property(e => e.PortfolioId).HasColumnName("portfolioID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.GaleriPortofolio)
                .HasMaxLength(255)
                .HasColumnName("galeri_portofolio");
            entity.Property(e => e.Judul)
                .HasMaxLength(255)
                .HasColumnName("judul");
            entity.Property(e => e.LinkPorotofolio)
                .HasMaxLength(255)
                .HasColumnName("link_porotofolio");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Portofolios)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("portofolios_talentid_foreign");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PRIMARY");

            entity
                .ToTable("projects")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "projects_talentid_foreign");

            entity.Property(e => e.ProjectId).HasColumnName("projectID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Klien)
                .HasMaxLength(255)
                .HasColumnName("klien");
            entity.Property(e => e.NamaProyek)
                .HasMaxLength(255)
                .HasColumnName("nama_proyek");
            entity.Property(e => e.PenggunaanTeknologi)
                .HasMaxLength(255)
                .HasColumnName("penggunaan_teknologi");
            entity.Property(e => e.PeranTim)
                .HasMaxLength(255)
                .HasColumnName("peran_tim");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TanggalMulai).HasColumnName("tanggal_mulai");
            entity.Property(e => e.TanggalSelesai).HasColumnName("tanggal_selesai");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Projects)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("projects_talentid_foreign");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("sessions")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LastActivity, "sessions_last_activity_index");

            entity.HasIndex(e => e.UserId, "sessions_user_id_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastActivity).HasColumnName("last_activity");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.UserAgent)
                .HasColumnType("text")
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Social>(entity =>
        {
            entity.HasKey(e => e.SocialId).HasName("PRIMARY");

            entity
                .ToTable("socials")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "socials_talentid_foreign");

            entity.Property(e => e.SocialId).HasColumnName("socialID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Platform)
                .HasMaxLength(255)
                .HasColumnName("platform");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Talent).WithMany(p => p.Socials)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("socials_talentid_foreign");
        });

        modelBuilder.Entity<SoftSkill>(entity =>
        {
            entity.HasKey(e => e.SoftskillsId).HasName("PRIMARY");

            entity
                .ToTable("soft_skills")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "soft_skills_talentid_foreign");

            entity.Property(e => e.SoftskillsId).HasColumnName("softskillsID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.NamaSkill)
                .HasMaxLength(255)
                .HasColumnName("nama_skill");
            entity.Property(e => e.Profisiensi)
                .HasMaxLength(255)
                .HasColumnName("profisiensi");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.SoftSkills)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("soft_skills_talentid_foreign");
        });

        modelBuilder.Entity<Talent>(entity =>
        {
            entity.HasKey(e => e.TalentId).HasName("PRIMARY");

            entity
                .ToTable("talents")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "talents_email_unique").IsUnique();

            entity.HasIndex(e => e.Nik, "talents_nik_unique").IsUnique();

            entity.HasIndex(e => e.NomorTelepon, "talents_nomortelepon_unique").IsUnique();

            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.Alamat)
                .HasMaxLength(255)
                .HasColumnName("alamat");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FotoProfil)
                .HasMaxLength(255)
                .HasColumnName("fotoProfil");
            entity.Property(e => e.JenisKelamin)
                .HasColumnType("enum('Laki-Laki','Perempuan')")
                .HasColumnName("jenisKelamin");
            entity.Property(e => e.KabupatenKota)
                .HasMaxLength(255)
                .HasColumnName("kabupaten_kota");
            entity.Property(e => e.KabupatenKotaId).HasColumnName("kabupaten_kota_id");
            entity.Property(e => e.LastVerificationRequestAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_verification_request_at");
            entity.Property(e => e.LokasiKerjaDiinginkan)
                .HasMaxLength(255)
                .HasColumnName("lokasiKerjaDiinginkan");
            entity.Property(e => e.Nama)
                .HasMaxLength(255)
                .HasColumnName("nama");
            entity.Property(e => e.Nik)
                .HasMaxLength(16)
                .HasColumnName("nik");
            entity.Property(e => e.NomorTelepon).HasColumnName("nomorTelepon");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PreferensiGaji).HasColumnName("preferensiGaji");
            entity.Property(e => e.PreferensiJamKerjaMulai)
                .HasColumnType("time")
                .HasColumnName("preferensiJamKerjaMulai");
            entity.Property(e => e.PreferensiJamKerjaSelesai)
                .HasColumnType("time")
                .HasColumnName("preferensiJamKerjaSelesai");
            entity.Property(e => e.PreferensiPerjalananDinas)
                .HasMaxLength(255)
                .HasColumnName("preferensiPerjalananDinas");
            entity.Property(e => e.Provinsi)
                .HasMaxLength(255)
                .HasColumnName("provinsi");
            entity.Property(e => e.ProvinsiId).HasColumnName("provinsi_id");
            entity.Property(e => e.RememberToken)
                .HasMaxLength(100)
                .HasColumnName("remember_token");
            entity.Property(e => e.ResetPasswordRequest).HasColumnName("resetPasswordRequest");
            entity.Property(e => e.StatusAkun)
                .HasMaxLength(255)
                .HasDefaultValueSql("'Belum Terverifikasi'")
                .HasColumnName("statusAkun");
            entity.Property(e => e.StatusPekerjaanSaatIni)
                .HasMaxLength(255)
                .HasColumnName("statusPekerjaanSaatIni");
            entity.Property(e => e.StatusVerifikasi)
                .HasMaxLength(255)
                .HasDefaultValueSql("'0'")
                .HasColumnName("statusVerifikasi");
            entity.Property(e => e.TentangSaya)
                .HasColumnType("text")
                .HasColumnName("tentangSaya");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Usia).HasColumnName("usia");
            entity.Property(e => e.VerificationToken)
                .HasMaxLength(255)
                .HasColumnName("verificationToken");
        });

        modelBuilder.Entity<TalentAcaraJobApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("talent_acara_job_applications")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "talent_acara_job_applications_acara_jobfair_id_foreign");

            entity.HasIndex(e => e.ApplicationCode, "talent_acara_job_applications_application_code_unique").IsUnique();

            entity.HasIndex(e => e.LowonganAcaraId, "talent_acara_job_applications_lowongan_acara_id_foreign");

            entity.HasIndex(e => new { e.TalentId, e.LowonganAcaraId }, "talent_acara_job_applications_talentid_lowongan_acara_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.ApplicationCode).HasColumnName("application_code");
            entity.Property(e => e.AppliedAt)
                .HasColumnType("timestamp")
                .HasColumnName("applied_at");
            entity.Property(e => e.CoverLetter)
                .HasColumnType("text")
                .HasColumnName("cover_letter");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.InterviewSlot)
                .HasMaxLength(255)
                .HasColumnName("interview_slot");
            entity.Property(e => e.LowonganAcaraId).HasColumnName("lowongan_acara_id");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','reviewed','interview','accepted','rejected')")
                .HasColumnName("status");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.TalentAcaraJobApplications)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("talent_acara_job_applications_acara_jobfair_id_foreign");

            entity.HasOne(d => d.LowonganAcara).WithMany(p => p.TalentAcaraJobApplications)
                .HasForeignKey(d => d.LowonganAcaraId)
                .HasConstraintName("talent_acara_job_applications_lowongan_acara_id_foreign");

            entity.HasOne(d => d.Talent).WithMany(p => p.TalentAcaraJobApplications)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("talent_acara_job_applications_talentid_foreign");
        });

        modelBuilder.Entity<TalentAcaraRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("talent_acara_registrations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AcaraJobfairId, "talent_acara_registrations_acara_jobfair_id_foreign");

            entity.HasIndex(e => e.RegistrationCode, "talent_acara_registrations_registration_code_unique").IsUnique();

            entity.HasIndex(e => new { e.TalentId, e.AcaraJobfairId }, "talent_acara_registrations_talentid_acara_jobfair_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcaraJobfairId).HasColumnName("acara_jobfair_id");
            entity.Property(e => e.AttendedAt)
                .HasColumnType("timestamp")
                .HasColumnName("attended_at");
            entity.Property(e => e.CheckedInAt)
                .HasColumnType("timestamp")
                .HasColumnName("checked_in_at");
            entity.Property(e => e.CheckedOutAt)
                .HasColumnType("timestamp")
                .HasColumnName("checked_out_at");
            entity.Property(e => e.CheckinStatus)
                .HasDefaultValueSql("'waiting'")
                .HasColumnType("enum('waiting','inside','checked_out','cancelled')")
                .HasColumnName("checkin_status");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.QrCodePath)
                .HasMaxLength(255)
                .HasColumnName("qr_code_path");
            entity.Property(e => e.RegisteredAt)
                .HasColumnType("timestamp")
                .HasColumnName("registered_at");
            entity.Property(e => e.RegistrationCode).HasColumnName("registration_code");
            entity.Property(e => e.ScanLog)
                .HasColumnType("text")
                .HasColumnName("scan_log");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'registered'")
                .HasColumnType("enum('registered','attended','cancelled')")
                .HasColumnName("status");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AcaraJobfair).WithMany(p => p.TalentAcaraRegistrations)
                .HasForeignKey(d => d.AcaraJobfairId)
                .HasConstraintName("talent_acara_registrations_acara_jobfair_id_foreign");

            entity.HasOne(d => d.Talent).WithMany(p => p.TalentAcaraRegistrations)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("talent_acara_registrations_talentid_foreign");
        });

        modelBuilder.Entity<TalentInterviewAttendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("talent_interview_attendance")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ApplyId, "talent_interview_attendance_applyid_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplyId).HasColumnName("applyID");
            entity.Property(e => e.AttendedAt)
                .HasColumnType("datetime")
                .HasColumnName("attended_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Apply).WithMany(p => p.TalentInterviewAttendances)
                .HasForeignKey(d => d.ApplyId)
                .HasConstraintName("talent_interview_attendance_applyid_foreign");
        });

        modelBuilder.Entity<TalentReference>(entity =>
        {
            entity.HasKey(e => e.ReferenceId).HasName("PRIMARY");

            entity
                .ToTable("talent_references")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "talent_references_talentid_foreign");

            entity.Property(e => e.ReferenceId).HasColumnName("referenceID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Nama)
                .HasMaxLength(255)
                .HasColumnName("nama");
            entity.Property(e => e.Perusahaan)
                .HasMaxLength(255)
                .HasColumnName("perusahaan");
            entity.Property(e => e.Posisi)
                .HasMaxLength(255)
                .HasColumnName("posisi");
            entity.Property(e => e.Relasi)
                .HasMaxLength(255)
                .HasColumnName("relasi");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.Telepon)
                .HasMaxLength(255)
                .HasColumnName("telepon");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.TalentReferences)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("talent_references_talentid_foreign");
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.HasKey(e => e.TrainingId).HasName("PRIMARY");

            entity
                .ToTable("trainings")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "trainings_talentid_foreign");

            entity.Property(e => e.TrainingId).HasColumnName("trainingID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.LinkSertifikat)
                .HasMaxLength(255)
                .HasColumnName("link_sertifikat");
            entity.Property(e => e.NamaPelatihan)
                .HasMaxLength(255)
                .HasColumnName("nama_pelatihan");
            entity.Property(e => e.Penyelenggara)
                .HasMaxLength(255)
                .HasColumnName("penyelenggara");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TanggalMulai).HasColumnName("tanggal_mulai");
            entity.Property(e => e.TanggalSelesai).HasColumnName("tanggal_selesai");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.Training)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("trainings_talentid_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "users_email_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("timestamp")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RememberToken)
                .HasMaxLength(100)
                .HasColumnName("remember_token");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<WorkHistory>(entity =>
        {
            entity.HasKey(e => e.WorkhistoryId).HasName("PRIMARY");

            entity
                .ToTable("work_histories")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TalentId, "work_histories_talentid_foreign");

            entity.Property(e => e.WorkhistoryId).HasColumnName("workhistoryID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deskripsi)
                .HasMaxLength(255)
                .HasColumnName("deskripsi");
            entity.Property(e => e.Perusahaan)
                .HasMaxLength(255)
                .HasColumnName("perusahaan");
            entity.Property(e => e.Posisi)
                .HasMaxLength(255)
                .HasColumnName("posisi");
            entity.Property(e => e.TalentId).HasColumnName("talentID");
            entity.Property(e => e.TanggalMulai)
                .HasMaxLength(255)
                .HasColumnName("tanggal_mulai");
            entity.Property(e => e.TanggalSelesai)
                .HasMaxLength(255)
                .HasColumnName("tanggal_selesai");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Talent).WithMany(p => p.WorkHistories)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("work_histories_talentid_foreign");
        });

        OnModelCreatingPartial(modelBuilder);


        // ✅ TAMBAHKAN INI
        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.ToTable("login_attempts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
            entity.Property(e => e.AttemptTime).HasColumnName("attempt_time");
            entity.Property(e => e.IsSuccess).HasColumnName("is_success");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        });

        modelBuilder.Entity<BlockedIp>(entity =>
        {
            entity.ToTable("blocked_ips");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
            entity.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(255);
            entity.Property(e => e.BlockedAt).HasColumnName("blocked_at");
            entity.Property(e => e.BlockedUntil).HasColumnName("blocked_until");
        });


        modelBuilder.Entity<SavedJob>(entity =>
        {
            entity.HasKey(e => e.saved_job_ID).HasName("PRIMARY");

            entity
                .ToTable("saved_job")
                .UseCollation("utf8mb4_unicode_ci");

            // Indexes untuk foreign keys
            entity.HasIndex(e => e.TalentId, "saved_job_talentid_foreign");
            entity.HasIndex(e => e.LowonganId, "saved_job_lowonganid_foreign");

            // ✅ PERBAIKI MAPPING KOLOM - SESUAIKAN DENGAN DATABASE
            entity.Property(e => e.saved_job_ID)
                .HasColumnName("saved_job_ID"); // ← Gunakan saved_job_ID (bukan 'id')

            entity.Property(e => e.TalentId)
                .HasColumnName("talentID"); // ← talentID (bukan talent_id)

            entity.Property(e => e.LowonganId)
                .HasColumnName("lowonganID"); // ← lowonganID (bukan lowongan_id)

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            // Relationships
            entity.HasOne(d => d.Talent)
                .WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.TalentId)
                .HasConstraintName("saved_job_talentid_foreign");

            entity.HasOne(d => d.Lowongan)
                .WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.LowonganId)
                .HasConstraintName("saved_job_lowonganid_foreign");
        });

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
