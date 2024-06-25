using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

public partial class CmsContext : DbContext
{
    public CmsContext()
    {
    }

    public CmsContext(DbContextOptions<CmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<ComponentLocation> ComponentLocations { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<CultureCode> CultureCodes { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<EmailTemplateItem> EmailTemplateItems { get; set; }

    public virtual DbSet<Faqdetail> Faqdetails { get; set; }

    public virtual DbSet<Faqheader> Faqheaders { get; set; }

    public virtual DbSet<GaleryCategory> GaleryCategories { get; set; }

    public virtual DbSet<GaleryDetail> GaleryDetails { get; set; }

    public virtual DbSet<GaleryHeader> GaleryHeaders { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Layout> Layouts { get; set; }

    public virtual DbSet<LayoutRole> LayoutRoles { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<LinkType> LinkTypes { get; set; }

    public virtual DbSet<LiveChat> LiveChats { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<MeasureUnit> MeasureUnits { get; set; }

    public virtual DbSet<MediaEx> MediaExes { get; set; }

    public virtual DbSet<MediaExCategory> MediaExCategories { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuType> MenuTypes { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    public virtual DbSet<MunicipalityLocation> MunicipalityLocations { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<PageMedium> PageMedia { get; set; }

    public virtual DbSet<Personel> Personels { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<PostMedium> PostMedia { get; set; }

    public virtual DbSet<PostTag> PostTags { get; set; }

    public virtual DbSet<PostsInCategory> PostsInCategories { get; set; }

    public virtual DbSet<PostsInTag> PostsInTags { get; set; }

    public virtual DbSet<ResourceTranslationString> ResourceTranslationStrings { get; set; }

    public virtual DbSet<ResourceTranslationType> ResourceTranslationTypes { get; set; }

    public virtual DbSet<Sequence> Sequences { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Slide> Slides { get; set; }

    public virtual DbSet<SocialNetwork> SocialNetworks { get; set; }

    public virtual DbSet<SysMenu> SysMenus { get; set; }

    public virtual DbSet<SysMenuRole> SysMenuRoles { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<ThemeConfig> ThemeConfigs { get; set; }

    public virtual DbSet<UserAudit> UserAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NameEn)
                .HasMaxLength(128)
                .HasDefaultValueSql("(N'')")
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(128)
                .HasDefaultValueSql("(N'')")
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(128)
                .HasDefaultValueSql("(N'')")
                .HasColumnName("Name_SR");
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(256);
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");
            entity.Property(e => e.ChangePassword)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");
            entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Firstname).HasDefaultValueSql("(N'')");
            entity.Property(e => e.Lastname).HasDefaultValueSql("(N'')");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PasswordExpires).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.PersonalNumber).HasMaxLength(10);
            entity.Property(e => e.ProfileImage).HasMaxLength(512);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.WorkPosition).HasMaxLength(500);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AspNetUserRoles_AspNetRoles"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AspNetUserRoles_AspNetUsers"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760AD604C984E");
                        j.ToTable("AspNetUserRoles");
                        j.IndexerProperty<string>("UserId").HasMaxLength(256);
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.LoginProvider).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ComponentLocation>(entity =>
        {
            entity.ToTable("ComponentLocation");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.TitleEn)
                .HasMaxLength(250)
                .HasColumnName("Title_EN");
            entity.Property(e => e.TitleSq)
                .HasMaxLength(250)
                .HasColumnName("Title_SQ");
            entity.Property(e => e.TitleSr)
                .HasMaxLength(250)
                .HasColumnName("Title_SR");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Contact");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fax)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.Property(e => e.MapLocation).HasMaxLength(250);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber2)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Layout).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contact_Layout");

            entity.HasOne(d => d.Media).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Contact_Media");

            entity.HasOne(d => d.Gender).WithMany(p => p.Contacts)
                .HasForeignKey(d => new { d.GenderId, d.LanguageId })
                .HasConstraintName("FK_Contact_Gender");

            entity.HasOne(d => d.Page).WithMany(p => p.Contacts)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_Contact_Page");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.EmailBcc)
                .HasMaxLength(200)
                .HasColumnName("EmailBCC");
            entity.Property(e => e.EmailCc)
                .HasMaxLength(200)
                .HasColumnName("EmailCC");
            entity.Property(e => e.EmailFrom).HasMaxLength(200);
            entity.Property(e => e.EmailTo).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Subject).HasMaxLength(200);
        });

        modelBuilder.Entity<CultureCode>(entity =>
        {
            entity.ToTable("CultureCode");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CultureCode1)
                .HasMaxLength(20)
                .HasColumnName("CultureCode");
            entity.Property(e => e.CultureCodeCountry).HasMaxLength(100);
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("EmailTemplate");

            entity.Property(e => e.Content).HasMaxLength(1500);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Subject).HasMaxLength(250);
        });

        modelBuilder.Entity<EmailTemplateItem>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId }).HasName("PK_EmailTemplateItem_1");

            entity.ToTable("EmailTemplateItem");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.EmailTemplate).WithMany(p => p.EmailTemplateItems)
                .HasForeignKey(d => new { d.EmailTemplateId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailTemplateItem_EmailTemplate");
        });

        modelBuilder.Entity<Faqdetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("FAQDetails");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.OrderNo).HasDefaultValueSql("((0))");
            entity.Property(e => e.Question).HasMaxLength(500);

            entity.HasOne(d => d.Faqheader).WithMany(p => p.Faqdetails)
                .HasForeignKey(d => new { d.HeaderId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FAQDetails_FAQHeader");
        });

        modelBuilder.Entity<Faqheader>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("FAQHeader");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Layout).WithMany(p => p.Faqheaders)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FAQHeader_Layout");

            entity.HasOne(d => d.Page).WithMany(p => p.Faqheaders)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_FAQHeader_Page");
        });

        modelBuilder.Entity<GaleryCategory>(entity =>
        {
            entity.ToTable("GaleryCategory");

            entity.Property(e => e.NameEn)
                .HasMaxLength(50)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(50)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(50)
                .HasColumnName("Name_SR");
        });

        modelBuilder.Entity<GaleryDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            entity.HasOne(d => d.Media).WithMany(p => p.GaleryDetails)
                .HasForeignKey(d => d.MediaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GaleryDetails_Media");

            entity.HasOne(d => d.GaleryHeader).WithMany(p => p.GaleryDetails)
                .HasForeignKey(d => new { d.HeaderId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GaleryDetails_GaleryHeader");
        });

        modelBuilder.Entity<GaleryHeader>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId }).HasName("PK_Galery");

            entity.ToTable("GaleryHeader");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.ShfaqNeHome)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Category).WithMany(p => p.GaleryHeaders)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GaleryHeader_GaleryCategory");

            entity.HasOne(d => d.Layout).WithMany(p => p.GaleryHeaders)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GaleryHeader_Layout");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Gender");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Language");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CultureCodeId).HasColumnName("CultureCodeID");
            entity.Property(e => e.NameEn)
                .HasMaxLength(20)
                .HasColumnName("Name_En");
            entity.Property(e => e.NameSq)
                .HasMaxLength(20)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(20)
                .HasColumnName("Name_SR");

            entity.HasOne(d => d.CultureCode).WithMany(p => p.Languages)
                .HasForeignKey(d => d.CultureCodeId)
                .HasConstraintName("FK_Language_CultureCode");
        });

        modelBuilder.Entity<Layout>(entity =>
        {
            entity.ToTable("Layout");

            entity.Property(e => e.Active).HasDefaultValueSql("((1))");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.NameEn)
                .HasMaxLength(50)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(50)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(50)
                .HasColumnName("Name_SR");
            entity.Property(e => e.Path).HasMaxLength(50);
        });

        modelBuilder.Entity<LayoutRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LayoutRo__C8F9964BC3744215");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Layout).WithMany(p => p.LayoutRoles)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LayoutRoles_Layout");
        });

        modelBuilder.Entity<Link>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId }).HasName("PK_Links");

            entity.ToTable("Link");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.LinkName).HasMaxLength(100);
            entity.Property(e => e.LinkTarget)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Url).HasMaxLength(200);

            entity.HasOne(d => d.Layout).WithMany(p => p.Links)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Links_Layout");

            entity.HasOne(d => d.Page).WithMany(p => p.Links)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_Links_Page");

            entity.HasOne(d => d.LinkType).WithMany(p => p.Links)
                .HasForeignKey(d => new { d.TypeId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Links_LinkTypes");
        });

        modelBuilder.Entity<LinkType>(entity =>
        {
            entity.HasKey(e => new { e.LinkTypeId, e.LanguageId }).HasName("PK_LinkTypes");

            entity.ToTable("LinkType");

            entity.Property(e => e.LinkuTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ComponentLocation).WithMany(p => p.LinkTypes)
                .HasForeignKey(d => d.ComponentLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LinkType_LinkType");
        });

        modelBuilder.Entity<LiveChat>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId }).HasName("PK_CMS_LiveChat");

            entity.ToTable("LiveChat");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.OtherSource).HasMaxLength(50);
            entity.Property(e => e.OtherSourceName).HasMaxLength(200);

            entity.HasOne(d => d.Language).WithMany(p => p.LiveChats)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LiveChat_Language");

            entity.HasOne(d => d.Page).WithMany(p => p.LiveChats)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_LiveChat_Page");

            entity.HasOne(d => d.LiveChatNavigation).WithMany(p => p.InverseLiveChatNavigation)
                .HasForeignKey(d => new { d.ParentId, d.LanguageId })
                .HasConstraintName("FK_LiveChat_LiveChat");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Action).HasMaxLength(128);
            entity.Property(e => e.Controller).HasMaxLength(128);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DescriptionTitle).HasMaxLength(256);
            entity.Property(e => e.Hostname)
                .HasMaxLength(128)
                .HasColumnName("HOSTNAME");
            entity.Property(e => e.HttpMethod).HasMaxLength(64);
            entity.Property(e => e.InsertedDate).HasColumnType("datetime");
            entity.Property(e => e.Ip)
                .HasMaxLength(64)
                .HasColumnName("IP");
            entity.Property(e => e.Url).HasMaxLength(2048);
            entity.Property(e => e.UserId).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<MeasureUnit>(entity =>
        {
            entity.ToTable("MeasureUnit");

            entity.Property(e => e.NameEn)
                .HasMaxLength(50)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(50)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(50)
                .HasColumnName("Name_SR");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MediaEx>(entity =>
        {
            entity.HasKey(e => e.MediaEx1).HasName("PK_MediaEX");

            entity.ToTable("MediaEx");

            entity.Property(e => e.MediaEx1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MediaEx");
            entity.Property(e => e.MediaExPath)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.MediaExCategory).WithMany(p => p.MediaExes)
                .HasForeignKey(d => d.MediaExCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaEX_MediaExCategory");
        });

        modelBuilder.Entity<MediaExCategory>(entity =>
        {
            entity.ToTable("MediaExCategory");

            entity.Property(e => e.NameEn)
                .HasMaxLength(150)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(150)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(150)
                .HasColumnName("Name_SR");
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.FileEx)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FileNameMedium).HasMaxLength(250);
            entity.Property(e => e.FileNameSmall).HasMaxLength(250);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.OtherSourceLink).HasMaxLength(500);

            entity.HasOne(d => d.FileExNavigation).WithMany(p => p.Media)
                .HasForeignKey(d => d.FileEx)
                .HasConstraintName("FK_Media_MediaEX");

            entity.HasOne(d => d.MediaExCategory).WithMany(p => p.Media)
                .HasForeignKey(d => d.MediaExCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_MediaExCategory");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Menu");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.IsClickable)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Level).HasDefaultValueSql("((1))");
            entity.Property(e => e.MenuParentId).HasColumnName("MenuParentID");
            entity.Property(e => e.MenuTypeId).HasColumnName("MenuTypeID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.OtherSourceName).HasMaxLength(200);
            entity.Property(e => e.OtherSourceUrl).HasMaxLength(100);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.PageIdredirect).HasColumnName("PageIDRedirect");
            entity.Property(e => e.PageParentId).HasColumnName("PageParentID");
            entity.Property(e => e.Target)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MenuNavigation).WithMany(p => p.InverseMenuNavigation)
                .HasForeignKey(d => new { d.MenuParentId, d.LanguageId })
                .HasConstraintName("FK_Menu_Menu");

            entity.HasOne(d => d.MenuType).WithMany(p => p.Menus)
                .HasForeignKey(d => new { d.MenuTypeId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu_MenuType");

            entity.HasOne(d => d.Page).WithMany(p => p.MenuPages)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_Menu_Page");

            entity.HasOne(d => d.PageNavigation).WithMany(p => p.MenuPageNavigations)
                .HasForeignKey(d => new { d.PageIdredirect, d.LanguageId })
                .HasConstraintName("FK_Menu_PageRedirect");

            entity.HasOne(d => d.Page1).WithMany(p => p.MenuPage1s)
                .HasForeignKey(d => new { d.PageParentId, d.LanguageId })
                .HasConstraintName("FK_Menu_PageParent");
        });

        modelBuilder.Entity<MenuType>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId }).HasName("PK_WebMenuLocation");

            entity.ToTable("MenuType");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Layout).WithMany(p => p.MenuTypes)
                .HasForeignKey(d => d.LayoutId)
                .HasConstraintName("FK_MenuType_MenuType");
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.ToTable("Municipality");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.NameEn)
                .HasMaxLength(50)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(50)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(50)
                .HasColumnName("Name_SR");

            entity.HasOne(d => d.Media).WithMany(p => p.Municipalities)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Municipality_Media");
        });

        modelBuilder.Entity<MunicipalityLocation>(entity =>
        {
            entity.ToTable("MunicipalityLocation");

            entity.Property(e => e.Area).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Latitude)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MapLocationUrl)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.NameEn)
                .HasMaxLength(250)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(250)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(250)
                .HasColumnName("Name_SR");

            entity.HasOne(d => d.MeasureUnit).WithMany(p => p.MunicipalityLocations)
                .HasForeignKey(d => d.MeasureUnitId)
                .HasConstraintName("FK_MunicipalityLocation_MeasureUnit");

            entity.HasOne(d => d.Media).WithMany(p => p.MunicipalityLocations)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_MunicipalityLocation_Media");

            entity.HasOne(d => d.Municipality).WithMany(p => p.MunicipalityLocations)
                .HasForeignKey(d => d.MunicipalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MunicipalityLocation_Municipality");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Page");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.LayoutId).HasColumnName("LayoutID");
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.PageAdress).HasMaxLength(200);
            entity.Property(e => e.PageName).HasMaxLength(250);
            entity.Property(e => e.PageParentId).HasColumnName("PageParentID");
            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

            entity.HasOne(d => d.Language).WithMany(p => p.Pages)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Page_Language");

            entity.HasOne(d => d.Layout).WithMany(p => p.Pages)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Page_Layout");

            entity.HasOne(d => d.Media).WithMany(p => p.Pages)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Page_Media");

            entity.HasOne(d => d.Template).WithMany(p => p.Pages)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Page_Template");

            entity.HasOne(d => d.PageNavigation).WithMany(p => p.InversePageNavigation)
                .HasForeignKey(d => new { d.PageParentId, d.LanguageId })
                .HasConstraintName("FK_Page_Page");
        });

        modelBuilder.Entity<PageMedium>(entity =>
        {
            entity.HasKey(e => new { e.PageId, e.LanguageId, e.MediaId });

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.DocumentParentId)
                .HasDefaultValueSql("((0))")
                .HasColumnName("DocumentParentID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(50);
            entity.Property(e => e.LinkName).HasMaxLength(100);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Media).WithMany(p => p.PageMedia)
                .HasForeignKey(d => d.MediaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageMedia_Media");

            entity.HasOne(d => d.Page).WithMany(p => p.PageMedia)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageMedia_PageMedia");
        });

        modelBuilder.Entity<Personel>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Personel");

            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.BirthPlace).HasMaxLength(100);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OrderNo).HasDefaultValueSql("((0))");
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(200);
            entity.Property(e => e.Qualification).HasMaxLength(200);

            entity.HasOne(d => d.Layout).WithMany(p => p.Personels)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Personel_Layout");

            entity.HasOne(d => d.Media).WithMany(p => p.Personels)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Personel_Media");

            entity.HasOne(d => d.Page).WithMany(p => p.Personels)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_Personel_Page");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Post");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(500);

            entity.HasOne(d => d.Media).WithMany(p => p.Posts)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Post_Media");
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("PostCategory");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Extra).HasColumnType("text");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Layout).WithMany(p => p.PostCategories)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostCategory_Layout");

            entity.HasOne(d => d.Page).WithMany(p => p.PostCategories)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_PostCategory_Page");
        });

        modelBuilder.Entity<PostMedium>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.LanguageId, e.MediaId }).HasName("PK_PostMedia_1");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);

            entity.HasOne(d => d.Media).WithMany(p => p.PostMedia)
                .HasForeignKey(d => d.MediaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostMedia_Media");

            entity.HasOne(d => d.Post).WithMany(p => p.PostMedia)
                .HasForeignKey(d => new { d.PostId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostMedia_Post");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        modelBuilder.Entity<PostsInCategory>(entity =>
        {
            entity.HasKey(e => new { e.PostCategoryId, e.PostId, e.LanguageId }).HasName("PK_PostsInCategory_1");

            entity.ToTable("PostsInCategory");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);

            entity.HasOne(d => d.PostCategory).WithMany(p => p.PostsInCategories)
                .HasForeignKey(d => new { d.PostCategoryId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostsInCategory_PostCategory");

            entity.HasOne(d => d.Post).WithMany(p => p.PostsInCategories)
                .HasForeignKey(d => new { d.PostId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostsInCategory_Post");
        });

        modelBuilder.Entity<PostsInTag>(entity =>
        {
            entity.HasKey(e => new { e.PostTagId, e.LanguageId, e.PostId }).HasName("PK_PostInTag");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);

            entity.HasOne(d => d.Post).WithMany(p => p.PostsInTags)
                .HasForeignKey(d => new { d.PostId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostInTag_Post");

            entity.HasOne(d => d.PostTag).WithMany(p => p.PostsInTags)
                .HasForeignKey(d => new { d.PostTagId, d.LanguageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostInTag_PostTags");
        });

        modelBuilder.Entity<ResourceTranslationString>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Value).HasMaxLength(1000);

            entity.HasOne(d => d.Language).WithMany(p => p.ResourceTranslationStrings)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceTranslationStrings_Language");

            entity.HasOne(d => d.Type).WithMany(p => p.ResourceTranslationStrings)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceTranslationStrings_ResourceTranslationType");
        });

        modelBuilder.Entity<ResourceTranslationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ResurceTranslationType");

            entity.ToTable("ResourceTranslationType");

            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Sequence>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__Sekuenca__3362627DD0CAC167");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Incr).HasDefaultValueSql("((1))");
            entity.Property(e => e.Seed).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Label)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Value).HasMaxLength(150);
        });

        modelBuilder.Entity<Slide>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.ToTable("Slide");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Deleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.LayoutId).HasColumnName("LayoutID");
            entity.Property(e => e.Link).HasMaxLength(200);
            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Layout).WithMany(p => p.Slides)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slide_Layout");

            entity.HasOne(d => d.Media).WithMany(p => p.Slides)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("FK_Slide_Media");

            entity.HasOne(d => d.Page).WithMany(p => p.Slides)
                .HasForeignKey(d => new { d.PageId, d.LanguageId })
                .HasConstraintName("FK_Slide_Page");
        });

        modelBuilder.Entity<SocialNetwork>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.LanguageId });

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Html).HasMaxLength(250);
            entity.Property(e => e.ImgPath).HasMaxLength(250);
            entity.Property(e => e.Link).HasMaxLength(250);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.ComponentLocation).WithMany(p => p.SocialNetworks)
                .HasForeignKey(d => d.ComponentLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialNetworks_ComponentLocation");

            entity.HasOne(d => d.Layout).WithMany(p => p.SocialNetworks)
                .HasForeignKey(d => d.LayoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialNetworks_Layout");
        });

        modelBuilder.Entity<SysMenu>(entity =>
        {
            entity.ToTable("SysMenu");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.NameEn)
                .HasMaxLength(250)
                .HasColumnName("Name_EN");
            entity.Property(e => e.NameSq)
                .HasMaxLength(250)
                .HasColumnName("Name_SQ");
            entity.Property(e => e.NameSr)
                .HasMaxLength(250)
                .HasColumnName("Name_SR");
            entity.Property(e => e.Path).HasMaxLength(50);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_SysMenu_SysMenu");
        });

        modelBuilder.Entity<SysMenuRole>(entity =>
        {
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.SysMenuRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SysMenuRoles_AspNetRoles");

            entity.HasOne(d => d.SysMenu).WithMany(p => p.SysMenuRoles)
                .HasForeignKey(d => d.SysMenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SysMenuRoles_SysMenu");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.ToTable("Template");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TemplateName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TemplateUrl).HasMaxLength(100);
            entity.Property(e => e.TemplateUrlWithId).HasColumnName("TemplateUrlWithID");
        });

        modelBuilder.Entity<ThemeConfig>(entity =>
        {
            entity.ToTable("ThemeConfig");

            entity.Property(e => e.LayoutType).HasMaxLength(100);
            entity.Property(e => e.Mode).HasMaxLength(100);
            entity.Property(e => e.NavMode).HasMaxLength(100);
            entity.Property(e => e.ThemeColor).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.ThemeConfigs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ThemeConfig_ThemeConfig");
        });

        modelBuilder.Entity<UserAudit>(entity =>
        {
            entity.ToTable("UserAudit");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DescriptionEn)
                .HasMaxLength(100)
                .HasColumnName("Description_EN");
            entity.Property(e => e.DescriptionSq)
                .HasMaxLength(100)
                .HasColumnName("Description_SQ");
            entity.Property(e => e.DescriptionSr)
                .HasMaxLength(100)
                .HasColumnName("Description_SR");
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
