using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class PlagueStingerGoliathV2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/PlagueStingerGoliath";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exploding Plague Stinger");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
			projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.position.Y > projectile.ai[1])
                projectile.tileCollide = true;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
			Texture2D glow = ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow");
            Vector2 origin = new Vector2(glow.Width / 2, glow.Height / Main.projFrames[projectile.type] / 2);
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            drawPos -= new Vector2(glow.Width, glow.Height / Main.projFrames[projectile.type]) * 1f / 2f;
            drawPos += origin * 1f + new Vector2(0f, 0f + 4f + projectile.gfxOffY);
            Color color = new Color(127 - projectile.alpha, 127 - projectile.alpha, 127 - projectile.alpha, 0).MultiplyRGBA(Color.Red);
            Main.spriteBatch.Draw(glow, drawPos, null, color, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlagueExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}
