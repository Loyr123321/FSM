using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models.Mobile
{
    public class CancellationReasonMobile
    {
        public string? Id { get; set; }
        public string? ReasonName { get; set; } = string.Empty;
        public string? ReasonNote { get; set; } = string.Empty; //Пояснения к таблице отклонений

        public CancellationReasonMobile(CancellationReason cancellationReason)
        {
            this.Id = cancellationReason.Id;
            this.ReasonName = cancellationReason.ReasonName;
            this.ReasonNote = cancellationReason.ReasonNote;
        }
    }
}
