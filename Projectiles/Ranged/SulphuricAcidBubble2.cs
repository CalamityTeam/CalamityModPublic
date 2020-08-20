using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SulphuricAcidBubble2 : ModProjectile
    {
        public float counter = 0f;
        public float counter2 = 0f;
        public int killCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 6)
            {
                projectile.frame = 0;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (counter >= 120f)
                {
                    counter = 0f;
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector15.X, vector15.Y, ModContent.ProjectileType<SulphuricAcidMist2>(), (int)(250f * Main.player[projectile.owner].RangedDamage()), 1f, projectile.owner, 0f, 0f);
                }
                else
                    counter += 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 6f)
                {
                    int num982 = 20;
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= num982;
                    }
                    if (projectile.alpha < 80)
                    {
                        projectile.alpha = 80;
                    }
                }
                if (projectile.ai[1] >= 45f)
                {
                    projectile.ai[1] = 45f;
                    if (counter2 < 1f)
                    {
                        counter2 += 0.002f;
                        projectile.scale += 0.002f;
                        projectile.width = (int)(30f * projectile.scale);
                        projectile.height = (int)(30f * projectile.scale);
                    }
                    else
                    {
                        projectile.width = 60;
                        projectile.height = 60;
                    }
                    if (projectile.wet)
                    {
                        if (projectile.velocity.Y > 0f)
                        {
                            projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                        }
                        if (projectile.velocity.Y > -1f)
                        {
                            projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                        }
                    }
                    else if (projectile.velocity.Y > -2f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                    }
                }
                killCounter++;
                if (killCounter >= 200)
                {
                    projectile.Kill();
                }
            }
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(3, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.buffImmune[BuffID.Venom] && target.aiStyle != 6)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 600);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            Main.PlaySound(SoundID.Item54, projectile.Center);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = projectile.alpha;
                num3 = num246;
            }
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }
    }
}
