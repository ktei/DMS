namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class InsertRequest
    {
        public string QueryId { get; set; }
        public int DisplayOrder{ get; set; }

        public InsertRequest(string queryId, int displayOrder)
        {
            QueryId = queryId;
            DisplayOrder = displayOrder;
        }

        public InsertRequest()
        {
            
        }
    }
}