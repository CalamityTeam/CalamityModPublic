using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Skies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
            if (Projectile.timeLeft == 689)
            {
                for (int i = 0; i < 2; i++)
                {
                    Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), 0f, 0.55f, 270, false);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
                Particle bloom2 = new BloomParticle(Projectile.Center, Vector2.Zero, Color.White, 0f, 0.5f, 270, false);
                GeneralParticleHandler.SpawnParticle(bloom2);
            }
            if (Projectile.timeLeft == 689 - 180)
            {
                Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, new Color(121, 21, 77), 0f, 0.85f, 90, false);
                GeneralParticleHandler.SpawnParticle(bloom);
            }

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
                if (Main.rand.NextBool())
                {
                    float variance = Main.rand.NextFloat(-25f, 25f);
                    Dust brimstone = Dust.NewDustPerfect(Projectile.Center + new Vector2(variance, 20), 267);
                    brimstone.scale = Main.rand.NextFloat(0.35f, 1.2f);
                    brimstone.color = Main.rand.NextBool() ? Color.Red : new Color(121, 21, 77);
                    brimstone.fadeIn = 0.7f;
                    brimstone.velocity = -Vector2.UnitY.RotatedBy(variance * 0.02f) * Main.rand.NextFloat(1.1f, 2.1f) * (Time * 0.023f);
                    brimstone.noGravity = true;
                }
            }

            Time++;
        }

        public void SummonSCal()
        {
            Vector2 spawnPosition = Projectile.Center - new Vector2(53f, 39f);
            // Summon SCal serverside.
            // All the other acoustic and visual effects can happen client-side.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC scal = CalamityUtils.SpawnBossBetter(spawnPosition, ModContent.NPCType<SupremeCalamitas>());
                if (Projectile.ai[1] == 1)
                {
                    scal.ModNPC<SupremeCalamitas>().permafrost = true;
                }
            }

            // Make a laugh sound and create a burst of brimstone dust.
            SoundStyle SpawnSound = Projectile.ai[1] == 1 ? Cryogen.DeathSound : SupremeCalamitas.SpawnSound;
            SoundEngine.PlaySound(SpawnSound, Projectile.Center);

            // Make a sudden screen shake.
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.GetLerpValue(3400f, 1560f, Main.LocalPlayer.Distance(Projectile.Center), true) * 16f;

            // Generate a dust explosion at the ritual's position.
            for (int i = 0; i < 90; i++)
            {
                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, Projectile.ai[1] == 1 ? DustID.IceGolem : (int)CalamityDusts.Brimstone, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 1.2f));
                spawnDust.noGravity = true;
                spawnDust.scale = Main.rand.NextFloat(1.2f, 2.3f);
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 sparkVel = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.1f);
                GlowOrbParticle orb = new GlowOrbParticle(Projectile.Center + sparkVel * 2, sparkVel, false, 120, Main.rand.NextFloat(1.55f, 2.75f), Projectile.ai[1] == 1 ? Color.Magenta : Color.Red, true, true);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Projectile.ai[1] == 1 ? Color.Cyan : Color.Red, new Vector2(2f, 2f), 0, 0f, 2.7f, 60);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Projectile.ai[1] == 1 ? Color.LightCyan : new Color(121, 21, 77), new Vector2(2f, 2f), 0, 0f, 2.1f, 60);
            GeneralParticleHandler.SpawnParticle(pulse2);
        }
    }
}
