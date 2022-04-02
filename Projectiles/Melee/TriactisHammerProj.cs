using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TriactisHammerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TriactisTruePaladinianMageHammerofMightMelee";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triactis' True Paladinian Mage-Hammer of Might");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 160;
            projectile.height = 160;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.melee = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.5f, 0.75f);
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 30f)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float num42 = 20f;
                float num43 = 5f;
                Vector2 vector2 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                float num44 = Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2 - vector2.X;
                float num45 = Main.player[projectile.owner].position.Y + Main.player[projectile.owner].height / 2 - vector2.Y;
                float num46 = (float)Math.Sqrt(num44 * num44 + num45 * num45);
                if (num46 > 3000f)
                    projectile.Kill();
                num46 = num42 / num46;
                num44 *= num46;
                num45 *= num46;
                if (projectile.velocity.X < num44)
                {
                    projectile.velocity.X = projectile.velocity.X + num43;
                    if (projectile.velocity.X < 0f && num44 > 0f)
                        projectile.velocity.X = projectile.velocity.X + num43;
                }
                else if (projectile.velocity.X > num44)
                {
                    projectile.velocity.X = projectile.velocity.X - num43;
                    if (projectile.velocity.X > 0f && num44 < 0f)
                        projectile.velocity.X = projectile.velocity.X - num43;
                }
                if (projectile.velocity.Y < num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num43;
                    if (projectile.velocity.Y < 0f && num45 > 0f)
                        projectile.velocity.Y = projectile.velocity.Y + num43;
                }
                else if (projectile.velocity.Y > num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num43;
                    if (projectile.velocity.Y > 0f && num45 < 0f)
                        projectile.velocity.Y = projectile.velocity.Y - num43;
                }
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                        projectile.Kill();
                }
            }
            projectile.rotation += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 50);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MageHammerBoom>(), (int)(projectile.damage * 0.25), projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int num621 = 0; num621 < 40; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 70; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MageHammerBoom>(), (int)(projectile.damage * 0.25), projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int num621 = 0; num621 < 40; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 70; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(2) ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
