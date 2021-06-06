// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using PingAI.DialogManagementService.Domain.Model;
// using PingAI.DialogManagementService.Infrastructure.Utils;
//
// namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
// {
//     public class QueryIntentConfiguration :  IEntityTypeConfiguration<QueryIntent>
//     {
//         public void Configure(EntityTypeBuilder<QueryIntent> builder)
//         {
//             builder.ToTable("QueryIntents", "chatbot");
//
//             builder.ConfigureId();
//             
//             builder.Property(o => o.QueryId)
//                 .HasColumnName("queryId");
//             builder.Property(o => o.IntentId)
//                 .HasColumnName("intentId");
//
//             builder.AttachTimestamps();
//         }
//     }
// }