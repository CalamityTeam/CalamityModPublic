using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class FuckYou : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.6f);
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1] * 0.5f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= (float)(3 * Main.projFrames[projectile.type]))
            {
                projectile.Kill();
                return;
            }
            int num3 = projectile.frameCounter + 1;
            projectile.frameCounter = num3;
            if (num3 >= 3)
            {
                projectile.frameCounter = 0;
                num3 = projectile.frame + 1;
                projectile.frame = num3;
                if (num3 >= Main.projFrames[projectile.type])
                {
                    projectile.hide = true;
                }
            }
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = (int)(52f * projectile.scale);
                projectile.Center = projectile.position;
                Main.PlaySound(SoundID.Item14, projectile.position);
                for (int num991 = 0; num991 < 4; num991 = num3 + 1)
                {
                    int num992 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num992].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    num3 = num991;
                }
                for (int num993 = 0; num993 < 10; num993 = num3 + 1)
                {
                    int num994 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 200, default, 2.7f);
                    Main.dust[num994].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[num994].noGravity = true;
                    Dust dust = Main.dust[num994];
                    dust.velocity *= 3f;
                    num994 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num994].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    dust = Main.dust[num994];
                    dust.velocity *= 2f;
                    Main.dust[num994].noGravity = true;
                    Main.dust[num994].fadeIn = 2.5f;
                    num3 = num993;
                }
                for (int num995 = 0; num995 < 5; num995 = num3 + 1)
                {
                    int num996 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 0, default, 2.7f);
                    Main.dust[num996].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num996].noGravity = true;
                    Dust dust = Main.dust[num996];
                    dust.velocity *= 3f;
                    num3 = num995;
                }
                for (int num997 = 0; num997 < 10; num997 = num3 + 1)
                {
                    int num998 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num998].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num998].noGravity = true;
                    Dust dust = Main.dust[num998];
                    dust.velocity *= 3f;
                    num3 = num997;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return projectile.ai[0] > 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 127);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            projectile.direction = Main.player[projectile.owner].direction;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)((double)damage * 0.3);
        }
    }
}
