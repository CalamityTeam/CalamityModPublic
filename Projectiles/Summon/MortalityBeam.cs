using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MortalityBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            if (!Main.dedServ)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    float offset = (float)Math.Sin(Time / 45f * MathHelper.TwoPi) * 10f * i;
                    Vector2 rotatedOffset = new Vector2(offset, 2f).RotatedBy(projectile.velocity.ToRotation() + MathHelper.PiOver2);

                    Dust rainbowDust = Dust.NewDustPerfect(projectile.Center + rotatedOffset, 261);
                    rainbowDust.color = Main.hslToRgb(Main.rand.NextFloat(), 0.9f, 0.5f);
                    rainbowDust.velocity = Vector2.Zero;
                    rainbowDust.scale = 1.5f;
                    rainbowDust.noGravity = true;
                }
            }

            Time++;
            NPC potentialTarget = projectile.Center.MinionHoming(1000f, Main.player[projectile.owner], false);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 5f + projectile.SafeDirectionTo(potentialTarget.Center) * 13f) / 6f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                float redHue = Main.rgbToHsl(Color.Red).X;
                float greenHue = Main.rgbToHsl(Color.SeaGreen).X;
                float blueHue = Main.rgbToHsl(Color.Blue).X;
                Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Projectile bolt = Projectile.NewProjectileDirect(spawnPosition, Main.rand.NextVector2CircularEdge(9f, 9f), ModContent.ProjectileType<MortalityBolt>(), projectile.damage, projectile.knockBack, projectile.owner);
                bolt.localAI[0] = Utils.SelectRandom(Main.rand, redHue, greenHue, blueHue);
            }
        }
    }
}
