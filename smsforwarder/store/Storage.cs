using Newtonsoft.Json;
using System;
using System.IO;

namespace smsforwarder.store { 
    public class Storage<T> : IStorage<T> {

        #region vars
        T t;
        string path;
        #endregion

        public Storage(string folder, string filename, T t) {                        
            string storePath = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(storePath))
                Directory.CreateDirectory(storePath);    
            this.path = Path.Combine(storePath, filename);
            this.t = t;
        }

        #region public
        public T load() {

            if (!File.Exists(path)) {
                save(t);
            }

             string rd = File.ReadAllText(path);

             var p = JsonConvert.DeserializeObject<T>(rd);

             return p;
            

        }

        public void save(T p) {

            var json = JsonConvert.SerializeObject(p, Formatting.Indented);
            try {

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllText(path, json);

            }
            catch (Exception ex) {
                throw new Exception("Не удалось сохранить файл JSON");
            }

        }
        #endregion
    }
}
