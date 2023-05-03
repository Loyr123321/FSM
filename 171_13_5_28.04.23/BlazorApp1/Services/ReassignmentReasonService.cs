using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    //Причины переназначения исполнителя
    public class ReassignmentReasonService : BaseService<ReassignmentReason>
    {
        public ReassignmentReasonService(string collectionName) : base(collectionName)
        {
        }
    }
}
