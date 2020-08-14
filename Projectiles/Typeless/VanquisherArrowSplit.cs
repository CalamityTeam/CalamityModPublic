using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class VanquisherArrowSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.arrow = true;
            projectile.timeLeft = 90;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 600f, 20f, 20f);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft < 90)
			{
				Vector2 origin = new Vector2(0f, 0f);
				Color color = Color.White;
				if (projectile.timeLeft < 85)
				{
					byte b2 = (byte)(projectile.timeLeft * 3);
					byte a2 = (byte)(100f * (b2 / 255f));
					color = new Color(b2, b2, b2, a2);
				}
				Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
				spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Typeless/VanquisherArrowSplitGlow"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(0, 0, 0, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
		}
    }
}
