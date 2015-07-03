using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace Haven
{
    public class Image
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Filepath { get; set; }

        public string Filename { get; set; }

        public void SaveImage(string rootPath, string folder, Stream stream)
        {
            this.Filepath = "/" + folder + "/" + this.Id.ToString();

            using (var fileStream = File.Create(Path.Combine(rootPath, this.FixedFilepath())))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        public void DeleteImage(string rootPath)
        {
            File.Delete(Path.Combine(rootPath, this.FixedFilepath()));
        }

        private string FixedFilepath()
        {
            return this.Filepath.TrimStart('/');
        }
    }
}
