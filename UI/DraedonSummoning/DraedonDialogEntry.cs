using System;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.UI.DraedonSummoning
{
    public class DraedonDialogEntry
    {
        public string Inquiry;

        public string Response;

        public float BloomOpacity;

        public Func<bool> Condition;

        // A bit of a scuffed way of identifying text based on a single neat number but it should be fine for all intents and purposes.
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

        public bool HasBeenSeen => Main.LocalPlayer.Calamity().SeenDraedonDialogs.Contains(ID);

        public DraedonDialogEntry(string inquiry, string response, Func<bool> condition = null)
        {
            // Initialize the condition as a simple "always show up" if nothing else is inputted.
            Condition = condition ?? (() => true);

            Inquiry = inquiry;
            Response = response;
        }

        public void Update()
        {
            if (!HasBeenSeen)
                BloomOpacity = 1f;
            else
                BloomOpacity = MathHelper.Clamp(BloomOpacity - 0.04f, 0f, 1f);
        }
    }
}
