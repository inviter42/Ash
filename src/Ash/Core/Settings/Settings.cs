using Ash.GlobalUtils;
using Newtonsoft.Json;

namespace Ash.Core.Settings
{
    public class PersistentSettings
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Setting<bool> ShouldMuteBackgroundFemale = new Setting<bool>(false);
    }

    public class Setting<T> {
        [JsonProperty("Value")]
        // ReSharper disable once InconsistentNaming
        internal T _value;

        // ignore, so IO.Save() isn't triggered during object construction
        [JsonIgnore]
        public T Value {
            get => _value;
            set {
                _value = value;
                IO.Save(Ash.Settings, IO.SettingsFileName);
            }
        }
        public Setting(T defaultValue) => _value = defaultValue;
    }
}
