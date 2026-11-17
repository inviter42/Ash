using System;
using System.IO;
using UnityEngine;

namespace Ash.GlobalUtils
{
    internal static class FileUtils
    {
        internal static byte[] LoadFile(string filePath) {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!File.Exists(filePath))
                throw new Exception($"File {filePath} does not exist.");

            return File.ReadAllBytes(filePath);
        }

        internal static Texture2D LoadTextureFromFile(string filePath, TextureFormat format, bool mipmap = false) {
            var bytes = LoadFile(filePath);
            // ReSharper disable once InvertIf
            if (bytes == null)
                throw new Exception("Unable to load texture - file does not exist.");

            var texture = new Texture2D(2, 2, format, mipmap);
            texture.LoadImage(bytes);

            return texture;
        }
    }
}
