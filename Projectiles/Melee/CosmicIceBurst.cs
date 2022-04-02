using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicIceBurst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Ice");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1];
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= (float)(3 * Main.projFrames[projectile.type]))
            {
                projectile.Kill();
                return;
            }
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.hide = true;
                }
            }
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight(projectile.Center, 0.517f, (float)Main.DiscoG / 300f, 0.85f);
            if (projectile.ai[0] == 1f)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = (int)(52f * projectile.scale);
                projectile.Center = projectile.position;
                projectile.Damage();
                Main.PlaySound(SoundID.Item62, projectile.position);
                for (int num1000 = 0; num1000 < 2; num1000++)
                {
                    int num1001 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 187, 0f, 0f, 100, new Color(150, 255, 255), 1f);
                    Main.dust[num1001].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                }
                for (int num1002 = 0; num1002 < 3; num1002++)
                {
                    int num1003 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 200, new Color(150, 255, 255), 1f);
                    Main.dust[num1003].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].velocity *= 3f;
                    num1003 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, new Color(150, 255, 255), 0.6f);
                    Main.dust[num1003].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[num1003].velocity *= 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].fadeIn = 2.5f;
                }
                for (int num1004 = 0; num1004 < 2; num1004++)
                {
                    int num1005 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 0, new Color(150, 255, 255), 1f);
                    Main.dust[num1005].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num1005].noGravity = true;
                    Main.dust[num1005].velocity *= 3f;
                }
                for (int num1006 = 0; num1006 < 3; num1006++)
                {
                    int num1007 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 187, 0f, 0f, 0, new Color(150, 255, 255), 0.6f);
                    Main.dust[num1007].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num1007].noGravity = true;
                    Main.dust[num1007].velocity *= 3f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            if (projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[projectile.type])
            {
                color25 = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f));
            }
            Vector2 vector42 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D33 = Main.projectileTexture[projectile.type];
            Rectangle rectangle15 = texture2D33.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color alpha5 = projectile.GetAlpha(color25);
            Vector2 origin11 = rectangle15.Size() / 2f;
            return true;
        }

        /*public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, Main.DiscoG, 255, 127);
        }*/

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            projectile.direction = Main.player[projectile.owner].direction;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            projectile.direction = Main.player[projectile.owner].direction;
        }
    }
}
