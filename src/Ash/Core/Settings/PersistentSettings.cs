using Ash.GlobalUtils;
using Newtonsoft.Json;

namespace Ash.Core.Settings
{
    internal class PersistentSettings
    {
        [JsonProperty]
        internal Setting<bool> ShouldMuteBackgroundFemale = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> DisableFemaleAutoEjaculation = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> SkippingSpurtStateEnabled = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> DisableFemaleHVoiceBark = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> DisableFemaleInactionBark = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> DisableFemaleVoiceBarkAtSceneStart = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> DisableFemaleVoiceBarkAtSceneEnd = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> InterruptVoiceClipImmediatelyUponGagChange = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> FixIncorrectShowMouthLiquidState = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> IsImmersiveUiEnabled = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> ConfineCursorToWindowEnabled = new Setting<bool>(false);

        [JsonProperty]
        internal Setting<bool> ThumbnailBackgroundRemovalEnabled = new Setting<bool>(false);
    }

    internal class Setting<T> {
        [JsonProperty("Value")]
        // ReSharper disable once InconsistentNaming
        internal T _value;

        // ignore to prevent IO.Save() trigger during object construction
        [JsonIgnore]
        public T Value {
            get => _value;
            set {
                _value = value;
                IO.Save(Ash.PersistentSettings, IO.SettingsFileName);
            }
        }

        public Setting(T defaultValue) => _value = defaultValue;
    }
}
