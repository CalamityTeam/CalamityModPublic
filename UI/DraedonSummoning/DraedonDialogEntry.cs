using System;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.UI.DraedonSummoning
{
    public class DraedonDialogEntry
    {
        /// <summary>
        /// The question the player asks to prompt the dialog.
        /// </summary>
        public string Inquiry;

        /// <summary>
        /// The response Draedon gives to the inquiry.
        /// </summary>
        public string Response;

        /// <summary>
        /// The opacity of the bloom flare for this dialog entry. Used to indicate new/unread dialog options to the player.
        /// </summary>
        public float BloomOpacity;

        /// <summary>
        /// The condition upon which this dialog can be read. The most common example of such is chcecking whether a boss of some kind has been defeated.
        /// </summary>
        public Func<bool> Condition;

        // A bit of a scuffed way of identifying text based on a single neat number but it should be fine for all intents and purposes.
        /// <summary>
        /// The (mostly) unique identifier of this dialog based on the response Draedon gives.
        /// </summary>
        public ulong ID
        {
            get
            {
                ulong result = 0uL;
                byte[] bytes = Encoding.UTF8.GetBytes(Response);

                unchecked
                {
                    for (int i = 0; i < bytes.Length; i++)
                        result += (ulong)bytes[i] << (i * 8);
                }
                return result;
            }
        }

        /// <summary>
        /// Whether the local player has seen this dialog instance yet.
        /// </summary>
        public bool HasBeenSeen => Main.LocalPlayer.Calamity().SeenDraedonDialogs.Contains(ID);

        // NOTE: This is a legacy constructor. It remains so that mod calls that rely on it do not break.
        internal DraedonDialogEntry(string inquiry, string response, Func<bool> condition = null)
        {
            // Initialize the condition as a simple "always show up" if nothing else is inputted.
            Condition = condition ?? (() => true);
            Inquiry = inquiry;
            Response = response;
        }

        // This is the proper constructor.
        public DraedonDialogEntry(string localizationKey, Func<bool> condition = null) :
            this(Language.GetTextValue($"{localizationKey}.Inquiry"), Language.GetTextValue($"{localizationKey}.Response"), condition)
        {
        }

        public void Update()
        {
            // Make bloom dissipate if this dialog has been seen.
            if (!HasBeenSeen)
                BloomOpacity = 1f;
            else
                BloomOpacity = MathHelper.Clamp(BloomOpacity - 0.04f, 0f, 1f);
        }
    }
}
