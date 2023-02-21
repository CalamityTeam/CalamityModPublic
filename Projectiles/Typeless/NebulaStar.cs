using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class NebulaStar : BaseSporeSacProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

            if (Projectile.owner == Main.myPlayer && !FadingOut)
            {
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 1f, 1f, 0.3f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<NebulaDust>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, ModContent.Request<Texture2D>(Texture).Value.Width, ModContent.Request<Texture2D>(Texture).Value.Height);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/NebulaStarGlow").Value, Projectile.Center - Main.screenPosition, frame, Color.White * ((255 - Projectile.alpha) / 255f), Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }
    }
}
