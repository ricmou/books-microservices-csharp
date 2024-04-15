using APIExemplar.Domain.Exemplars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIExemplar.Infraestructure.Exemplars
{
    internal class ExemplarEntityTypeConfiguration : IEntityTypeConfiguration<Exemplar>
    {
        public void Configure(EntityTypeBuilder<Exemplar> builder)
        {
            builder.ToTable("Exemplar", SchemaNames.APIExemplar);
            builder.HasKey(exemplar => exemplar.Id);
            builder.OwnsOne(exemplar => exemplar.Book, Book =>
            {
                Book.Property(property => property.Value)
                    .HasColumnName("BookId")
                    .IsRequired();
            }).Navigation(exemplar => exemplar.Book).IsRequired();
            builder.OwnsOne(exemplar => exemplar.BookState, BookState =>
            {
                BookState.Property(property => property.State)
                    .HasColumnName("BookState")
                    .IsRequired();
            }).Navigation(exemplar => exemplar.BookState).IsRequired();
            builder.OwnsOne(exemplar => exemplar.SellerId, SellerId =>
            {
                SellerId.Property(property => property.Value)
                    .HasColumnName("SellerId")
                    .IsRequired();
            }).Navigation(exemplar => exemplar.SellerId).IsRequired();
            builder.Property(author => author.DateOfAcquisition)
                .HasColumnName("DateOfAcquisition")
                .IsRequired();
        }
    }
}