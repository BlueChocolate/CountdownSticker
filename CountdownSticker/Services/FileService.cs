using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.Services
{
    public interface IFileService
    {
        public string? ReadFileText(string path);
        void WriteTextToFile(string path, string text);
        public Task<string> ReadFileTextAsync(string path);
        public Task WriteTextToFileAsync(string path, string text);
    }

    public class FileService : IFileService
    {
        public string? ReadFileText(string path)
        {
            if (File.Exists(path))
            {
                using var reader = File.OpenText(path);
                var text = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
            return null;
        }

        public void WriteTextToFile(string path, string text)
        {
            using var writer = File.CreateText(path); // 打开或创建
            writer.Write(text);
        }

        public async Task<string> ReadFileTextAsync(string path)
        {
            if (File.Exists(path))
            {
                using var reader = File.OpenText(path);
                return await reader.ReadToEndAsync();
            }
            else
            {
                await WriteTextToFileAsync(path, string.Empty);
                return string.Empty;
            }
        }

        public async Task WriteTextToFileAsync(string path, string text)
        {
            using var writer = File.CreateText(path);
            await writer.WriteAsync(text);
        }
    }
}
