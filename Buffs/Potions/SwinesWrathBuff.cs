using System;
using CalamityMod.DataStructures;
using CalamityMod.Items.Potions;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class SwinesWrathBuff : ModBuff
    {
        public static readonly SoundStyle TimerSound = new("CalamityMod/Sounds/Custom/TickingTimer");
        public static readonly SoundStyle TimerSound2 = new("CalamityMod/Sounds/Custom/TickingTimerReverb");

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            TheConcoctionPlayer concoctionPlayer = player.GetModPlayer<TheConcoctionPlayer>();

            if (concoctionPlayer.swinesWrathCounter < 2 && concoctionPlayer.swinesWrathCounter != -1)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.SwinesWrath" + Main.rand.Next(1, 7 + 1)).ToNetworkText(player.name)), 1000, -1);
                concoctionPlayer.swinesWrathCounter = -1;
                concoctionPlayer.spamTimer = 0;
            }

            else if (concoctionPlayer.swinesWrathCounter % 60 == 59)
            {
                SoundEngine.PlaySound(TimerSound2 with { Volume = MathF.Pow((600f - concoctionPlayer.swinesWrathCounter) / 600f, 2) * 0.8f, MaxInstances = -1 }, player.Center); // Gets louder as time progresses
                SoundEngine.PlaySound(TimerSound with { Volume = ((600f - concoctionPlayer.swinesWrathCounter) / 600f), MaxInstances = -1 }, player.Center); // Gets louder as time progresses
            }
        }
    }
}
