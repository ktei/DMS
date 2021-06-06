// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using PingAI.DialogManagementService.Domain.Model;
// using PingAI.DialogManagementService.Infrastructure.Utils;
//
// namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
// {
//     public class QueryResponseConfiguration : IEntityTypeConfiguration<QueryResponse>
//     {
//         public void Configure(EntityTypeBuilder<QueryResponse> builder)
//         {
//             builder.ToTable("QueryResponses", "chatbot");
//             builder.ConfigureId();
//
//             builder.Property(o => o.QueryId)
//                 .HasColumnName("queryId");
//             builder.Property(o => o.ResponseId)
//                 .HasColumnName("responseId");
//             
//             builder.AttachTimestamps();
//         }
//     }
// }