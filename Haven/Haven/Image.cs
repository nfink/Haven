using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace Haven
{
    public class Image : IDeletable, ICloneable<Image>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Filepath { get; set; }

        public string Filename { get; set; }

        public void SaveImage(Stream stream)
        {
            string rootDir = System.Configuration.ConfigurationManager.AppSettings["rootDir"];
            string folder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"];
            this.Filepath = "/" + folder + "/" + this.Id.ToString();

            using (var fileStream = File.Create(Path.Combine(rootDir, this.FixedFilepath())))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        public void Delete()
        {
            string rootDir = System.Configuration.ConfigurationManager.AppSettings["rootDir"];
            File.Delete(Path.Combine(rootDir, this.FixedFilepath()));
            Persistence.Connection.Delete(this);
        }

        public Image Clone()
        {
            var image = new Image() { Filename = this.Filename };
            Persistence.Connection.Insert(image);

            if (this.Filepath != null)
            {
                string rootDir = System.Configuration.ConfigurationManager.AppSettings["rootDir"];
                using (var fileStream = File.Open(Path.Combine(rootDir, this.FixedFilepath()), FileMode.Open))
                {
                    image.SaveImage(fileStream);
                    Persistence.Connection.Update(image);
                }
            }

            return image;
        }

        private string FixedFilepath()
        {
            return this.Filepath.TrimStart('/');
        }
    }
}
