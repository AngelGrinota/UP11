using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TelegramBotApi.Models;

namespace TelegramBotApi.DataContext;

public partial class VoenkomContext : DbContext
{
    public VoenkomContext()
    {
    }

    public VoenkomContext(DbContextOptions<VoenkomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<InfoQuestion> InfoQuestions { get; set; }

    public virtual DbSet<LegalQuestion> LegalQuestions { get; set; }

    public virtual DbSet<MedicalQuestion> MedicalQuestions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserQuestionHistory> UserQuestionHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;userid=root;password=root;database=voenkom", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.19-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => new { e.ChatId, e.QuestionId, e.RequestDate })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("history");

            entity.HasIndex(e => e.QuestionId, "fk_history_question_idx");

            entity.HasIndex(e => e.ChatId, "fk_history_user_idx");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("request_date");

            entity.HasOne(d => d.Chat).WithMany(p => p.Histories)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_history_user");

            entity.HasOne(d => d.Question).WithMany(p => p.Histories)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_history_question");
        });

        modelBuilder.Entity<InfoQuestion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("info_questions");

            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("question_text");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("upload_date");
        });

        modelBuilder.Entity<LegalQuestion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("legal_questions");

            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("question_text");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("upload_date");
        });

        modelBuilder.Entity<MedicalQuestion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("medical_questions");

            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("question_text");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("upload_date");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.NotificationText)
                .HasColumnType("text")
                .HasColumnName("notification_text");
            entity.Property(e => e.PhotoNotification)
                .HasColumnType("mediumblob")
                .HasColumnName("photo_notification");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("send_time");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'ожидает'")
                .HasColumnType("enum('ожидает','отправлено')")
                .HasColumnName("status");

            entity.HasMany(d => d.Chats).WithMany(p => p.Notifications)
                .UsingEntity<Dictionary<string, object>>(
                    "Recipient",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_recipient_user"),
                    l => l.HasOne<Notification>().WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_recipient_notification"),
                    j =>
                    {
                        j.HasKey("NotificationId", "ChatId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("recipient");
                        j.HasIndex(new[] { "NotificationId" }, "fk_recipient_notification_idx");
                        j.HasIndex(new[] { "ChatId" }, "fk_recipient_user_idx");
                        j.IndexerProperty<int>("NotificationId").HasColumnName("notification_id");
                        j.IndexerProperty<long>("ChatId").HasColumnName("chat_id");
                    });
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PRIMARY");

            entity.ToTable("question");

            entity.HasIndex(e => e.FileName, "file_name_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CategoryId, "fk_question_category_idx");

            entity.HasIndex(e => e.PhotoName, "photo_name_UNIQUE").IsUnique();

            entity.HasIndex(e => e.QuestionText, "question_text_UNIQUE").IsUnique();

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.FileData)
                .HasColumnType("text")
                .HasColumnName("file_data");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.PhotoData)
                .HasColumnType("mediumblob")
                .HasColumnName("photo_data");
            entity.Property(e => e.PhotoName)
                .HasMaxLength(100)
                .HasColumnName("photo_name");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("question_text");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("upload_date");

            entity.HasOne(d => d.Category).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_question_category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.ChatId)
                .ValueGeneratedNever()
                .HasColumnName("chat_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LastActive)
                .HasColumnType("datetime")
                .HasColumnName("last_active");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserQuestionHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("user_question_history");

            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("question_text");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("request_date");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
