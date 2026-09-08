using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions.ExtendedControls
{
    internal class ExtSelectionToggle : MonoBehaviour
    {
        internal bool Value { get; private set; }

        private Button ButtonOn;
        private Button ButtonOff;
        private string GroupName;
        private int IndexInGroup;

        private static readonly Dictionary<string, List<ExtSelectionToggle>> RadioGroups = new Dictionary<string, List<ExtSelectionToggle>>();

        private event Action<int> Event;

        private void OnDestroy() {
            Event = null;
            ButtonOn.onClick.RemoveAllListeners();
            ButtonOff.onClick.RemoveAllListeners();
            RadioGroups.Remove(GroupName);
        }

        internal void Setup(string groupName, Button buttonOn, Button buttonOff, string text, Action<int> action) {
            GroupName = groupName;
            ButtonOn = buttonOn;
            ButtonOff = buttonOff;
            ButtonOn.GetComponentInChildren<Text>().text = text;
            ButtonOff.GetComponentInChildren<Text>().text = text;
            Event += action;

            ButtonOff.onClick.AddListener(() => OnClick(true));

            if (!RadioGroups.ContainsKey(groupName)) {
                Value = true; // first toggle in group must be true
                RadioGroups.Add(groupName, new List<ExtSelectionToggle>());
            } else {
                Value = false; // all other toggles are set to false
            }

            SwitchButton(); // apply

            RadioGroups[groupName].Add(this);
            IndexInGroup = RadioGroups[groupName].Count - 1;
        }

        internal void ChangeValue(bool flag, bool updateGroupMembers, bool invoke) {
            if (Value == flag)
                return;

            Value = flag;
            SwitchButton();

            if (updateGroupMembers)
                UpdateGroupMembers();

            if (!invoke || Event == null)
                return;

            Event?.Invoke(IndexInGroup);
        }

        private void SwitchButton() {
            ButtonOff.gameObject.SetActive(!Value);
            ButtonOn.gameObject.SetActive(Value);
        }

        private void OnClick(bool flag) {
            Value = flag;
            SwitchButton();

            UpdateGroupMembers();

            Event?.Invoke(IndexInGroup);
        }

        private void UpdateGroupMembers() {
            if (RadioGroups.ContainsKey(GroupName) && RadioGroups[GroupName].Count > 1) {
                foreach (var toggle in RadioGroups[GroupName].Where(toggle => toggle != this))
                    toggle.ChangeValue(false, false, false);
            }
        }

        public void SetColor(ColorBlock onColor, ColorBlock offColor) {
            ButtonOn.colors = onColor;
            ButtonOff.colors = offColor;
        }
    }
}
