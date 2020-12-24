using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Settings;

namespace PlixP.Services
{
    public static class CatcherKeys
    {
        public static string MovieList { get; } = "Movies";
    }
    public class CatcherService
    {
        public void Set(object item, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = item.GetType().Name;
            }

            var json = JsonConvert.SerializeObject(item,Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(key, json);
        }

        public T Get<T>(string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = typeof(T).Name;
            var json = CrossSettings.Current.GetValueOrDefault(key, string.Empty);
            if (string.IsNullOrEmpty(json))
                return Activator.CreateInstance<T>();
            return JsonConvert.DeserializeObject<T>(json);

        }

        public void Delete<T>(string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = typeof(T).Name;
            CrossSettings.Current.Remove(key);
        }
    }
}
