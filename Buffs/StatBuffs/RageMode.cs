using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class RageMode : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true; // Because duration is variable, time is not displayed.
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer mp = player.Calamity();

            // If the player still has Rage left to burn, the buff stays active indefinitely.
            if (mp.rage > 0f)
            {
                player.buffTime[buffIndex] = 2; // Every frame, give another frame for the buff to live.
                mp.rageModeActive = true;
            }

            // Otherwise, Rage Mode ends instantly.
            else
            {
                if (player.whoAmI == Main.myPlayer)
                    SoundEngine.PlaySound(CalamityPlayer.RageEndSound);
                player.DelBuff(buffIndex--); // TML documentation requires you to decrement buffIndex if deleting the buff during Update.
                mp.rageModeActive = false;
                mp.rage = 0f;
                player.Calamity().ragePulseTimer = 0;
            }
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = Player.Calamity();

            modPlayer.ragePulseTimer++;
            int dustID = modPlayer.heartOfDarkness ? 240 : 114;

            if (modPlayer.shatteredCommunity && Main.rand.NextBool())
                dustID = 112; //special dust visual for Shattered Community

            if (modPlayer.heartOfDarkness && !modPlayer.shatteredCommunity && Main.rand.NextBool())
                dustID = 90; //special dust visual for Heart of Darkness

            if (modPlayer.ragePulseTimer == 60)
            {
                PlayerCenteredPulseRing pulse = new PlayerCenteredPulseRing(Player, Vector2.Zero, Color.Red, new Vector2(1, 1), 0, 0f, 0.23f, 40);
                GeneralParticleHandler.SpawnParticle(pulse);
                modPlayer.ragePulse = true;
            }

            if (modPlayer.ragePulse)
            {
                modPlayer.ragePulseVisualTimer++;
                if (modPlayer.ragePulseVisualTimer >= 30)
                {
                    PlayerCenteredPulseRing pulse = new PlayerCenteredPulseRing(Player, Vector2.Zero, (modPlayer.shatteredCommunity ? Color.MediumPurple : Color.Red), new Vector2(1, 1), 0, 0f, 0.18f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    modPlayer.ragePulseVisualTimer = 0;
                    modPlayer.ragePulse = false;
                    modPlayer.ragePulseTimer = 0;
                }
            }

            Dust dust = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, dustID);
            dust.scale = Main.rand.NextFloat(0.3f, 0.45f);
            if (dustID == 112)
                dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
            if (dustID == 240)
                dust.scale = Main.rand.NextFloat(0.8f, 0.95f);
            dust.velocity = -Player.velocity / 3;
            dust.noGravity = true;
        }
    }
}
