using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ReLogic.Utilities;
using Terraria.Audio;

namespace CalamityMod.Systems
{
    public class ORDERSystem : ModSystem
    {
        public static float DefaultOrderTime = 70f;
        public static float DefaultResetTime = 200f;

        public static float OrderTime = 0f;
        public static float ResetTimer = 0f;
        public static SlotId OrderSoundSlot;
        public static readonly SoundStyle ORDERTrack = new("CalamityMod/Sounds/Custom/ORDER");

        public static bool OrderPlaying = false;

        public override void PreUpdateProjectiles()
        {
            if (OrderTime > 0)
                OrderTime--;

            if (ResetTimer > 0)
                ResetTimer--;

            if (ResetTimer == 0 && OrderPlaying)
            {
                OrderPlaying = false;
                if (SoundEngine.TryGetActiveSound(OrderSoundSlot, out var orderResult))
                    orderResult.Stop();
                OrderSoundSlot = SlotId.Invalid;
            }

            else if (OrderPlaying)
            {
                if (SoundEngine.TryGetActiveSound(OrderSoundSlot, out var orderResult))
                    orderResult.Volume = OrderTime / DefaultOrderTime;

            }
        }

        public static void ORDER()
        {
            if (!OrderPlaying)
                OrderSoundSlot = SoundEngine.PlaySound(ORDERTrack);

            OrderPlaying = true;
            OrderTime = DefaultOrderTime;
            ResetTimer = DefaultResetTime;
        }
    }
}
