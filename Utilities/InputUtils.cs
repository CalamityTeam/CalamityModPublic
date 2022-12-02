using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        internal static readonly FieldInfo KeybindName = typeof(ModKeybind).GetField("uniqueName", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool HasValidBinding(this ModKeybind key)
        {
            // Copypasted stuff from vanilla. This should not be as complicated as it is.
            static string genInput(List<string> list)
            {
                if (list.Count == 0)
                    return "";

                string text = list[0];
                for (int j = 1; j < list.Count; j++)
                    text = text + "/" + list[j];
                return text;
            }

            List<string> list = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[(string)KeybindName.GetValue(key)];
            string text = genInput(list);
            return !string.IsNullOrEmpty(text);
        }
    }
}
