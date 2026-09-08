using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using PhExtendedSaveFiles.Utils;
using Character;
using HarmonyLib;
using MessagePack;
using UnityEngine;

namespace PhExtendedSaveFiles
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ExtendedSaveFiles : BaseUnityPlugin
    {
        public const string PluginName = "PhExtendedSaveFiles";
        public const string GUID = "inviter42.phextendedsavefiles";
        public const string Version = "1.0.0";

        public static event Action SaveFileBeingWritten;
        public static event Action SaveFileBeingLoaded;

        internal new static ManualLogSource Logger;

        private const uint AppendixMagicBytes = 0x415348;
        private const uint AppendixEndMagicBytes = 0x454E44;
        private const uint AppendixVersion = 0;

        private static readonly Dictionary<string, PluginData> Payloads = new Dictionary<string, PluginData>();
        private static readonly Dictionary<string, PluginData> RetrievedData = new Dictionary<string, PluginData>();

        private void Awake() {
            Logger = base.Logger;
            Hooks.Patch();
        }

        /// <summary>
        /// Set the data to be recorded into a save file
        /// </summary>
        /// <param name="id">Unique payload identifier</param>
        /// <param name="payload">Data to be recorded</param>
        public static void SetPayload(string id, PluginData payload) {
            try {
                Payloads.Add(id, payload);
            }
            catch (Exception e) {
                Logger.LogError($"Exception has occured while setting payload: {e}");
            }
        }

        /// <summary>
        /// Get PluginData for a specified ID
        /// </summary>
        /// <param name="id">Unique data identifier</param>
        /// <returns>Instance of a PluginData class, containing the data, null if data doesn't exist</returns>
        public static PluginData GetDataById(string id) {
            return RetrievedData.TryGetValue(id, out var data) ? data : null;
        }

        private static void WriteSection(BinaryWriter writer, string sectionName, PluginData data) {
            var bytes = SerializationUtils.Serialize(data);

            writer.Write(sectionName);
            writer.Write(bytes.Length);
            Logger.LogDebug($"Writing section '{sectionName}' ({bytes.Length} bytes)");
            writer.Write(bytes);
        }

        private static KeyValuePair<string, PluginData> ReadSection(BinaryReader reader) {
            try {
                var closingBytes = reader.ReadUInt32();
                if (closingBytes == AppendixEndMagicBytes) {
                    Logger.LogDebug($"Reached closing bytes");
                    return default;
                }

                reader.BaseStream.Position -= sizeof(uint);

                var sectionName = reader.ReadString();
                var length = reader.ReadInt32();

                // Logger.LogDebug($"Reading section '{sectionName}' ({length} bytes)");
                if (sectionName.Length == 0 || length == 0) {
                    Logger.LogDebug($"Reached end of file");
                    return default;
                }

                var data = SerializationUtils.Deserialize<PluginData>(reader.ReadBytes(length));

                Logger.LogDebug($"Section '{sectionName}' deserialized successfully");

                return new KeyValuePair<string, PluginData>(sectionName, data);
            }
            catch (EndOfStreamException) {
                Logger.LogDebug($"Last section has been read");
                return default;
            }
            catch (Exception e) {
                Logger.LogWarning($"Unexpected exception has occured while trying to read magic bytes: {e}");
                return default;
            }
        }

        private static void WriteAllPayloads(BinaryWriter writer) {
            writer.Write(AppendixMagicBytes);
            writer.Write(AppendixVersion);

            if (Payloads.Count == 0) {
                Logger.LogDebug($"No payloads to write");
                return;
            }

            foreach (var kvp in Payloads) {
                Logger.LogDebug($"Writing payload '{kvp.Key}'");
                WriteSection(writer, kvp.Key, kvp.Value);
            }

            writer.Write(AppendixEndMagicBytes);

            Payloads.Clear();

            Logger.LogDebug($"All payloads have been written");
        }

        private static void FillRetrievedDataDictionary(BinaryReader reader) {
            RetrievedData.Clear(); // clear previous data

            try {
                uint data;
                do {
                    data = reader.ReadUInt32();
                } while (data != AppendixMagicBytes);
            }
            catch (EndOfStreamException) {
                Logger.LogDebug($"Unable to find magic bytes while reading the stream");
                return;
            }
            catch (Exception e) {
                Logger.LogError($"Unexpected exception has occured while trying to read magic bytes: {e}");
                return;
            }

            var version = reader.ReadUInt32();
            Logger.LogDebug($"Version: {version}");

            do {
                var kv = ReadSection(reader);
                if (kv.Key == null)
                    break;
                RetrievedData.Add(kv.Key, kv.Value);
            } while (true);
        }

        [MessagePackObject]
        public class PluginData
        {
            [Key(0)] public int Version;

            [Key(1)] public Dictionary<string, object> Data = new Dictionary<string, object>();
        }

        private class Hooks
        {
            internal static void Patch() {
                Harmony.CreateAndPatchAll(typeof(Hooks), "inviter42.phextsavefiles");
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(GamePlayData), nameof(GamePlayData.Save), typeof(string), typeof(string))]
            // ReSharper disable once InconsistentNaming
            internal static bool SavePrefix(GamePlayData __instance, string path, string comment) {
                FileStream output;
                try {
                    output = File.Create(path);
                }
                catch (Exception ex) {
                    MonoBehaviour.print($"ファイルが開けません:{path} {(object)ex}");
                    return false;
                }

                var writer = new BinaryWriter(output);
                __instance.header.version = 5;
                __instance.header.comment = comment;
                __instance.header.SetNowTime();
                __instance.header.Save(writer);

                writer.Write((int)__instance.progress);

                __instance.custom_hero.Save(writer);
                __instance.custom_kouichi.Save(writer);
                __instance.custom_h_maleMobA.Save(writer);
                __instance.custom_h_maleMobB.Save(writer);
                __instance.custom_h_maleMobC.Save(writer);

                writer.Write(4);

                __instance.custom_ritsuko.Save(writer);
                __instance.custom_akiko.Save(writer);
                __instance.custom_yukiko.Save(writer);
                __instance.custom_mariko.Save(writer);

                foreach (var personality in __instance.personality)
                    personality.Save(writer);

                writer.Write((int)__instance.lastSelectMale);
                writer.Write((int)__instance.lastSelectFemale);
                writer.Write((int)__instance.lastSelectVisitor);
                writer.Write(__instance.lastSelectMap);
                writer.Write(__instance.lastSelectTimeZone);
                writer.Write(__instance.unlockWeaknessRecovery);
                writer.Write(__instance.unlockShowHitArea);
                writer.Write(__instance.unlockFastXtc);
                writer.Write(__instance.readAllFreeMessage);
                writer.Write(__instance.readAllFreeWithAdd);

                SaveFileBeingWritten?.Invoke();

                WriteAllPayloads(writer);

                output.Close();

                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(GamePlayData), nameof(GamePlayData.Load), typeof(string))]
            // ReSharper disable once InconsistentNaming
            internal static bool LoadPrefix(GamePlayData __instance, string path) {
                FileStream input;
                try {
                    input = File.OpenRead(path);
                }
                catch {
                    return false;
                }

                var reader = new BinaryReader(input);

                if (!__instance.header.Load(reader)) {
                    input.Close();
                }
                else {
                    __instance.Clear();

                    __instance.progress = (GamePlayData.PROGRESS)reader.ReadInt32();
                    __instance.custom_hero.Load(reader);
                    __instance.custom_kouichi.Load(reader);
                    __instance.custom_h_maleMobA.Load(reader);
                    __instance.custom_h_maleMobB.Load(reader);
                    __instance.custom_h_maleMobC.Load(reader);

                    var num = reader.ReadInt32();

                    __instance.custom_ritsuko.Load(reader);
                    __instance.custom_akiko.Load(reader);
                    __instance.custom_yukiko.Load(reader);

                    if (__instance.header.version >= 4)
                        __instance.custom_mariko.Load(reader);
                    if (num >= 4) { }

                    for (var index = 0; index < num; ++index)
                        __instance.personality[index].Load(reader, __instance.header.version);

                    __instance.lastSelectMale = (MALE_ID)reader.ReadInt32();
                    __instance.lastSelectFemale = (HEROINE)reader.ReadInt32();
                    __instance.lastSelectVisitor = (VISITOR)reader.ReadInt32();
                    __instance.lastSelectMap = reader.ReadInt32();
                    __instance.lastSelectTimeZone = reader.ReadInt32();

                    if (__instance.header.version >= 2) {
                        __instance.unlockWeaknessRecovery = reader.ReadBoolean();
                        __instance.unlockShowHitArea = reader.ReadBoolean();
                        __instance.unlockFastXtc = reader.ReadBoolean();
                        __instance.readAllFreeMessage = reader.ReadBoolean();
                    }

                    if (__instance.header.version >= 5)
                        __instance.readAllFreeWithAdd = reader.ReadBoolean();

                    FillRetrievedDataDictionary(reader);

                    SaveFileBeingLoaded?.Invoke();

                    input.Close();
                }

                return false;
            }
        }
    }
}
