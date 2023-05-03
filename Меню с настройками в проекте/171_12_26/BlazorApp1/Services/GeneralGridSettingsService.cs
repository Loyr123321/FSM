using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class GeneralGridSettingsService : BaseService<GeneralGridSettings>
    {
        public GeneralGridSettingsService(string collectionName) : base(collectionName)
        {
            //Уникальность по 2м полям
            var keys = Builders<GeneralGridSettings>.IndexKeys.Combine(
                Builders<GeneralGridSettings>.IndexKeys.Ascending(x => x.TemplateId),
                Builders<GeneralGridSettings>.IndexKeys.Ascending(x => x.DefaultUniqueness)
            );
            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<GeneralGridSettings>(keys, options);
            _table.Indexes.CreateOne(indexModel);
        }

        public List<GeneralGridSettings> GetSettings(string templateId)
        {
            //return _table.Find(x => x.TemplateId == templateId).ToList();

            var allSettings = _table.Find(x => x.TemplateId == templateId).ToList();

            var defaultSetting = allSettings.FirstOrDefault(x=>x.IsDefault == true);
            if (defaultSetting!=null)
            {
                allSettings.Remove(defaultSetting);
                allSettings.Insert(0, defaultSetting);
            }
            
            return allSettings;
        }

        public void DeleteDefaultSettings(string templateId)
        {
            var defaultSettings = _table.Find(x => x.TemplateId == templateId && x.IsDefault == true).FirstOrDefault();
            if (defaultSettings != null && !string.IsNullOrEmpty(defaultSettings.Id))
            {
                base.Delete(defaultSettings.Id);
            }
        }

    }
}
