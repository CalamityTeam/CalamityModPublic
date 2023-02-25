using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ORDERSystem : ModSystem
    {
        private static readonly SoundStyle ORDERTrack = new("CalamityMod/Sounds/Custom/ORDER", SoundType.Music);
        private static SlotId orderSoundSlot;

        internal static float DefaultOrderTime = 100f;
        internal static float DefaultResetTime = 240f;

        internal static float remainingPlaytime = 0f;
        internal static float timeUntilReset = 0f;

        private static bool currentlyPlaying = false;

        public override void UpdateUI(GameTime gameTime)
        {
            // Decrement timers
            if (remainingPlaytime > 0)
                --remainingPlaytime;
            if (timeUntilReset > 0)
                --timeUntilReset;

            if (currentlyPlaying)
            {
                // If the reset timer has run out, stop the active sound instance entirely.
                // The next Ricoshot will start the song from the beginning.
                if (timeUntilReset <= 0f)
                {
                    currentlyPlaying = false;
                    if (SoundEngine.TryGetActiveSound(orderSoundSlot, out var activeSound))
                        activeSound.Stop();
                    orderSoundSlot = SlotId.Invalid;
                }

                // Otherwise, set the volume of the active sound instance appropriately.
                else
                {
                    bool foundOrder = SoundEngine.TryGetActiveSound(orderSoundSlot, out var activeSound);
                    if (!foundOrder)
                    {
                        currentlyPlaying = false;
                        return;
                    }

                    float newVolume = MathHelper.Clamp(remainingPlaytime / DefaultOrderTime, 0f, 1f);
                    activeSound.Volume = newVolume;
                    activeSound.Update();
                }
            }
        }

        // DIE!
        public static void JUDGMENT()
        {
            if (!currentlyPlaying)
                orderSoundSlot = SoundEngine.PlaySound(ORDERTrack);

            currentlyPlaying = true;
            remainingPlaytime = DefaultOrderTime;
            timeUntilReset = DefaultResetTime;
        }
    }
}
