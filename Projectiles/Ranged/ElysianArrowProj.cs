using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElysianArrowProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/ElysianArrow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.aiStyle = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            for (int num621 = 0; num621 < 2; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 6; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
            float x = projectile.position.X + Main.rand.Next(-400, 400);
            float y = projectile.position.Y - Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num15 = projectile.position.X + (projectile.width / 2) - vector.X;
            float num16 = projectile.position.Y + (projectile.height / 2) - vector.Y;
            num15 += Main.rand.Next(-100, 101);
            int num17 = 25;
            float num18 = (float)Math.Sqrt(num15 * num15 + num16 * num16);
            num18 = num17 / num18;
            num15 *= num18;
            num16 *= num18;
            if (projectile.owner == Main.myPlayer)
            {
                int num19 = Projectile.NewProjectile(x, y, num15, num16, ModContent.ProjectileType<SkyFlareFriendly>(), projectile.damage / 2, 5f, projectile.owner, 0f, 0f);
                Main.projectile[num19].ai[1] = projectile.position.Y;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
