using CalamityMod.Dusts;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SCalRitualDrama : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float Time => ref projectile.ai[0];
        public const int PulseTime = 45;
        public const int TotalRitualTime = 270;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Calamitous Ritual Drama");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = TotalRitualTime;
        }

		public override void AI()
		{
			SCalSky.OverridingIntensity = Utils.InverseLerp(90f, TotalRitualTime - 25f, Time, true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.InverseLerp(90f, TotalRitualTime - 25f, Time, true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower *= Utils.InverseLerp(3400f, 1560f, Main.LocalPlayer.Distance(projectile.Center), true) * 4f;

            int fireReleaseRate = Time > 150f ? 2 : 1;
            for (int i = 0; i < fireReleaseRate; i++)
			{
                if (Main.rand.NextBool(4))
                {
                    Dust brimstone = Dust.NewDustPerfect(projectile.Center + new Vector2(Main.rand.NextFloat(-20f, 24f), Main.rand.NextFloat(10f, 18f)), 267);
                    brimstone.scale = Main.rand.NextFloat(0.7f, 1f);
                    brimstone.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat());
                    brimstone.fadeIn = 0.7f;
                    brimstone.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.5f, 2.8f);
                    brimstone.noGravity = true;
                }
            }

            Time++;
		}

		public override void Kill(int timeLeft)
		{
            if (Main.netMode != NetmodeID.MultiplayerClient)
                CalamityUtils.SpawnBossBetter(projectile.Center - new Vector2(60f), ModContent.NPCType<SupremeCalamitas>());

            // Make a laugh sound and create a burst of brimstone dust.
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SupremeCalamitasSpawn"), projectile.Center);
            Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, projectile.Center);

            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.InverseLerp(3400f, 1560f, Main.LocalPlayer.Distance(projectile.Center), true) * 16f;

            // Generate a dust explosion
            float burstDirectionVariance = 3;
            float burstSpeed = 14f;
            for (int j = 0; j < 16; j++)
            {
                burstDirectionVariance += j * 2;
                for (int k = 0; k < 40; k++)
                {
                    Dust burstDust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
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
