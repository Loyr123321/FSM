using BlazorApp1.Models;
using MongoDB.Driver;
using NPOI.SS.Formula.Functions;

namespace BlazorApp1.Services
{
    public class UserGridSettingsService : BaseService<UserGridSettings>
    {
        public UserGridSettingsService(string collectionName) : base(collectionName)
        {
            //Уникальность по 3м полям
            var keys = Builders<UserGridSettings>.IndexKeys.Combine(
                Builders<UserGridSettings>.IndexKeys.Ascending(x => x.UserId),
                Builders<UserGridSettings>.IndexKeys.Ascending(x => x.TemplateId),
                Builders<UserGridSettings>.IndexKeys.Ascending(x => x.MainUniqueness)
            );
            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserGridSettings>(keys, options);
            _table.Indexes.CreateOne(indexModel);
        }

        public List<UserGridSettings> GetSettings(string templateId, string userId)
        {
            return _table.Find(x => x.TemplateId == templateId && x.UserId == userId).ToList();
        }

        public UserGridSettings? GetMainSettings(string temlateId, string userId) 
        {
            return _table.Find(x => x.TemplateId == temlateId && x.UserId == userId && x.IsMain == true).FirstOrDefault();
        }

        //Убрать галочку Основной формат у всех записей По Шаблону и Юзеру
        public void SetMainSettingsFalse(string temlateId, string userId)
        {
            var settings = _table.Find(x => x.TemplateId == temlateId && x.UserId == userId && x.IsMain==true).FirstOrDefault();

            if (settings != null) 
            {
                settings.IsMain = false;
                base.SaveOrUpdate(settings);
            }
        }
    }
}
