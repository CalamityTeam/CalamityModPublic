using CalamityMod.Dusts;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Skies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class SCalRitualDrama : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float Time => ref Projectile.ai[0];
        public const int TotalRitualTime = 270;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = TotalRitualTime + 420;
        }

        public override void AI()
        {
            // If needed, these effects may continue after the ritual timer, to ensure that there are no awkward
            // background changes between the time it takes for SCal to appear after this projectile is gone.
            // If SCal is already present, this does not happen.
            if (!NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
            {
                SCalSky.OverridingIntensity = Utils.GetLerpValue(90f, TotalRitualTime - 25f, Time, true);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.GetLerpValue(90f, TotalRitualTime - 25f, Time, true);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower *= Utils.GetLerpValue(3400f, 1560f, Main.LocalPlayer.Distance(Projectile.Center), true) * 4f;
            }

            // Summon SCal right before the ritual effect ends.
            // The projectile lingers a little longer, however, to ensure that desync delays in MP do not interfere with the background transition.
            if (Time == TotalRitualTime - 1f)
                SummonSCal();

            if (Time >= TotalRitualTime)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
                    Projectile.Kill();
                return;
            }

            int fireReleaseRate = Time > 150f ? 2 : 1;
            for (int i = 0; i < fireReleaseRate; i++)
            {
                if (Main.rand.NextBool(4))
                {
                    Dust brimstone = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-20f, 24f), Main.rand.NextFloat(10f, 18f)), 267);
                    brimstone.scale = Main.rand.NextFloat(0.7f, 1f);
                    brimstone.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat());
                    brimstone.fadeIn = 0.7f;
                    brimstone.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.5f, 2.8f);
                    brimstone.noGravity = true;
                }
            }

            Time++;
        }

        public void SummonSCal()
        {
            // Summon SCal serverside.
            // All the other acoustic and visual effects can happen client-side.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC scal = CalamityUtils.SpawnBossBetter(Projectile.Center - new Vector2(60f), ModContent.NPCType<SupremeCalamitas>());
                if (Projectile.ai[1] == 1)
                {
                    scal.ModNPC<SupremeCalamitas>().cirrus = true;
                }
            }

            // Make a laugh sound and create a burst of brimstone dust.
            SoundStyle SpawnSound = Projectile.ai[1] == 1 ? SoundID.Item107 : SupremeCalamitas.SpawnSound;
            SoundEngine.PlaySound(SpawnSound, Projectile.Center);

            // Make a sudden screen shake.
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.GetLerpValue(3400f, 1560f, Main.LocalPlayer.Distance(Projectile.Center), true) * 16f;

            // Generate a dust explosion at the ritual's position.
            float burstDirectionVariance = 3;
            float burstSpeed = 14f;
            for (int j = 0; j < 16; j++)
            {
                burstDirectionVariance += j * 2;
                for (int k = 0; k < 40; k++)
                {
                    int type = Projectile.ai[1] == 1 ? 69 : (int)CalamityDusts.Brimstone;
                    Dust burstDust = Dust.NewDustPerfect(Projectile.Center, type);
                    burstDust.scale = Main.rand.NextFloat(3.1f, 3.5f);
                    burstDust.position += Main.rand.NextVector2Circular(10f, 10f);
                    burstDust.velocity = Main.rand.NextVector2Square(-burstDirectionVariance, burstDirectionVariance).SafeNormalize(Vector2.UnitY) * burstSpeed;
                    burstDust.noGravity = true;
                }
                burstSpeed += 1.8f;
            }
        }
    }
}
