using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DemonBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil Fork");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.alpha = 180;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                173,
                (int)CalamityDusts.Brimstone,
                172
            });

            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }

            bool tileCheck = WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.position.X / 16, (int)projectile.position.Y / 16));
            Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f)];
            dust.position = projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;

            if (tileCheck)
                dust.noLight = true;
            
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.ToRadians(45);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            bool tileCheck = WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.position.X / 16, (int)projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    (int)CalamityDusts.Brimstone,
                    172
                });
                Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    (int)CalamityDusts.Brimstone,
                    172
                });
                int dustInt = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[dustInt].noGravity = true;
                Main.dust[dustInt].velocity *= 3f;
                if (tileCheck)
                {
                    Main.dust[dustInt].noLight = true;
                }
                dustInt = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
                Main.dust[dustInt].velocity *= 2f;
                Main.dust[dustInt].noGravity = true;
                if (tileCheck)
                {
                    Main.dust[dustInt].noLight = true;
                }
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 90);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
