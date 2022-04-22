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

        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
            if (!Main.dedServ)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    float offset = (float)Math.Sin(Time / 45f * MathHelper.TwoPi) * 10f * i;
                    Vector2 rotatedOffset = new Vector2(offset, 2f).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2);

                    Dust rainbowDust = Dust.NewDustPerfect(Projectile.Center + rotatedOffset, 261);
                    rainbowDust.color = Main.hslToRgb(Main.rand.NextFloat(), 0.9f, 0.5f);
                    rainbowDust.velocity = Vector2.Zero;
                    rainbowDust.scale = 1.5f;
                    rainbowDust.noGravity = true;
                }
            }

            Time++;
            NPC potentialTarget = Projectile.Center.MinionHoming(1000f, Main.player[Projectile.owner], false);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 5f + Projectile.SafeDirectionTo(potentialTarget.Center) * 13f) / 6f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float redHue = Main.rgbToHsl(Color.Red).X;
                float greenHue = Main.rgbToHsl(Color.SeaGreen).X;
                float blueHue = Main.rgbToHsl(Color.Blue).X;
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Projectile bolt = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPosition, Main.rand.NextVector2CircularEdge(9f, 9f), ModContent.ProjectileType<MortalityBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                bolt.originalDamage = Projectile.originalDamage;
                bolt.localAI[0] = Utils.SelectRandom(Main.rand, redHue, greenHue, blueHue);
            }
        }
    }
}
