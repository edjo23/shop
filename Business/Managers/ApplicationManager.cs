using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class ApplicationManager
    {
        public const string ImageFileExtension = ".png";

        public IEnumerable<Denomination> GetDenominations()
        {
            // TODO - Move to DB.

            var result = new List<Denomination>();
            result.Add(new Denomination { Description = "$100", Value = 100.0m });
            result.Add(new Denomination { Description = "$50", Value = 50.0m });
            result.Add(new Denomination { Description = "$20", Value = 20.0m });
            result.Add(new Denomination { Description = "$10", Value = 10.0m });
            result.Add(new Denomination { Description = "$5", Value = 5.0m });
            result.Add(new Denomination { Description = "$2", Value = 2.0m });
            result.Add(new Denomination { Description = "$1", Value = 1.0m });
            result.Add(new Denomination { Description = "50c", Value = 0.50m });
            result.Add(new Denomination { Description = "20c", Value = 0.20m });
            result.Add(new Denomination { Description = "10c", Value = 0.10m });
            result.Add(new Denomination { Description = "5c", Value = 0.05m });

            return result;
        }

        public IEnumerable<string> GetImageList()
        {
            var directory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Images"));

            return directory.Exists ? directory.GetFiles("*" + ImageFileExtension).Select(o => o.Name.Substring(0, o.Name.Length - ImageFileExtension.Length)).ToList() : new List<string>();
        }

        public byte[] GetImage(string code)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            var file = new FileInfo(Path.Combine(folder, code + ".png"));

            return file.Exists ? File.ReadAllBytes(file.FullName) : new byte[0];
        }

        public string[] GetText(string code)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Text");
            var file = new FileInfo(Path.Combine(folder, code + ".txt"));

            return file.Exists ? File.ReadAllLines(file.FullName) : new string[0];
        }
    }
}
