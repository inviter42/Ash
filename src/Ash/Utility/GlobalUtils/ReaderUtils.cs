using System;
using System.IO;

namespace Ash.Utility.GlobalUtils
{
    internal static class ReaderUtils
    {
        internal static bool SeekByteSequence(BinaryReader reader, uint sequence) {
            var initialPosition = reader.BaseStream.Position;

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            try {
                uint rollingData = 0;

                while (true) {
                    var b = reader.ReadByte();
                    rollingData = (rollingData >> 8) | ((uint)b << 24);
                    if (rollingData == sequence)
                        return true;
                }
            }
            catch (EndOfStreamException) {
                Ash.Logger.LogDebug($"Unable to find magic bytes while reading the stream");
                reader.BaseStream.Position = initialPosition;
            }
            catch (Exception e) {
                Ash.Logger.LogError($"Unexpected exception has occured while trying to read magic bytes: {e}");
                reader.BaseStream.Position = initialPosition;
            }

            return false;
        }
    }
}
