using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class VanquisherArrowSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/VanquisherArrow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.arrow = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override void PostDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 90)
            {
                Vector2 origin = new Vector2(0f, 0f);
                Color color = Color.White;
                if (Projectile.timeLeft < 85)
                {
                    byte b2 = (byte)(Projectile.timeLeft * 3);
                    byte a2 = (byte)(100f * (b2 / 255f));
                    color = new Color(b2, b2, b2, a2);
                }
                Texture2D baseTexture = ModContent.Request<Texture2D>(Texture).Value;
                Rectangle frame = new Rectangle(0, 0, baseTexture.Width, baseTexture.Height);
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Ammo/VanquisherArrowGlow").Value, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(0, 0, 0, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }
    }
}
