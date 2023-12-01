using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class Voidragon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 150;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 150 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 12f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustVel = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    dustVel += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    dustVel = dustVel.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int shadowDust = Dust.NewDust(Projectile.Center, 0, 0, 173, 0f, 0f, 160, default, 1f);
                    Main.dust[shadowDust].scale = 1.1f;
                    Main.dust[shadowDust].noGravity = true;
                    Main.dust[shadowDust].position = Projectile.Center + dustVel;
                    Main.dust[shadowDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[shadowDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[shadowDust].position) * 1.25f;
                }
            }
            Projectile.alpha -= 15;
            int alphaControl = 150;
            if (Projectile.Center.Y >= Projectile.ai[1])
            {
                alphaControl = 0;
            }
            if (Projectile.alpha < alphaControl)
            {
                Projectile.alpha = alphaControl;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
            if (Main.rand.NextBool(16))
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int extraDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[extraDust].velocity = value3 * 0.66f;
                Main.dust[extraDust].position = Projectile.Center + value3 * 12f;
            }
            if (Main.rand.NextBool(48) && Main.netMode != NetmodeID.Server)
            {
                int voidGore = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[voidGore].velocity *= 0.66f;
                Main.gore[voidGore].velocity += Projectile.velocity * 0.3f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.light = 0.9f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
                if (Main.rand.NextBool(20) && Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.1f / 255f, (255 - Projectile.alpha) * 0.7f / 255f, (255 - Projectile.alpha) * 0.15f / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlasmaExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 1f);
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }
    }
}
