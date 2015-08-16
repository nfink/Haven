using SQLite;
using System.IO;
using System;

namespace Haven
{
    public class Image : IEntity, IDeletable, ICloneable<Image>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public string Filepath { get; set; }

        public string Filename { get; set; }

        public void SaveImage(Stream stream)
        {
            string rootDir = System.Configuration.ConfigurationManager.AppSettings["rootDir"];
            string folder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"];
            this.Filepath = "/" + folder + "/" + Guid.NewGuid().ToString();

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
            this.Repository.Remove<Image>(this);
        }

        public Image Clone()
        {
            var image = new Image() { Filename = this.Filename };
            this.Repository.Add<Image>(image);

            if (this.Filepath != null)
            {
                string rootDir = System.Configuration.ConfigurationManager.AppSettings["rootDir"];
                using (var fileStream = File.Open(Path.Combine(rootDir, this.FixedFilepath()), FileMode.Open))
                {
                    image.SaveImage(fileStream);
                    this.Repository.Add<Image>(image);
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
