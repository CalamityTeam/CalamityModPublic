using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ToxicannonShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/FlakAcid";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cannon Shot");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.timeLeft = 480;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
        }
        public override void AI()
        {
            if (projectile.ai[0]++ <= 30f)
            {
                projectile.alpha = (int)MathHelper.Lerp(255f, 0f, projectile.ai[0] / 30f);
            }
            if (projectile.velocity.Y < 20f)
                projectile.velocity.Y += 0.3f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Math.Sign(projectile.velocity.Y) != Math.Sign(projectile.oldVelocity.Y) && projectile.ai[0] >= 5f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }
        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 180);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
            for (int i = 0; i <= 40; i++)
            {
                int idx = Dust.NewDust(projectile.position, 230, 230, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (Main.dust[idx].position - projectile.Center).Length() / 25f;
                Main.dust[idx].scale = 2.5f;
            }
            for (int i = 0; i <= 90; i++)
            {
                int idx = Dust.NewDust(projectile.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid);
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 12f;
                Main.dust[idx].scale = 3f;
                Main.dust[idx].noGravity = true;
            }
            if (Main.rand.NextBool(3))
            {
                int count = Main.rand.Next(1, 3 + 1);
                for (int i = 0; i < count; i++)
                {
                    float angle = MathHelper.TwoPi * i / count;
                    angle += Main.rand.NextFloat(0.1f, 0.35f) * Main.rand.NextBool(2).ToDirectionInt();
                    Projectile.NewProjectile(projectile.Center, angle.ToRotationVector2() * Main.rand.NextFloat(4f, 16f),
                        ModContent.ProjectileType<ToxicannonDrop>(), projectile.damage, 2.5f, projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
