using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class CryoBlast : ModProjectile
    {
        private const float Spread = 0.15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 35;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 90;
            projectile.coldDamage = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.scale <= 2.5f)
            {
                projectile.scale *= 1.02f;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(35f * projectile.scale));
            }
            else if (projectile.ai[0] < 2f)
            {
                projectile.ai[0] += 1f;

                // Fire extra waves to the left and right
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(-Spread * (i + 1)), projectile.type, projectile.damage / 2, projectile.knockBack, projectile.owner, projectile.ai[0], 0f);
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(+Spread * (i + 1)), projectile.type, projectile.damage / 2, projectile.knockBack, projectile.owner, projectile.ai[0], 0f);
                }

                projectile.Kill();
            }

            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f || projectile.ai[0] > 0f)
            {
                int ice = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, 0f, 0f, 100, default, projectile.scale * 0.5f);
                Main.dust[ice].noGravity = true;
                Main.dust[ice].velocity *= 0f;
                int snow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 185, 0f, 0f, 100, default, projectile.scale * 0.5f);
                Main.dust[snow].noGravity = true;
                Main.dust[snow].velocity *= 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if ((projectile.timeLeft > 596 && projectile.ai[0] == 0f) || (projectile.timeLeft > 599 && projectile.ai[0] > 0f))
                return false;

            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, texture.Width, height);
            Vector2 origin = new Vector2(texture.Width / 2f, height / 2f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.Center);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, 0f, 0f, 0, new Color(), 0.9f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 shardPos = projectile.oldPosition + 0.5f * projectile.Size;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 shardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (shardVel.X == 0f && shardVel.Y == 0f)
                    {
                        shardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    shardVel.Normalize();
                    shardVel *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(shardPos, shardVel, ProjectileID.Blizzard, projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
