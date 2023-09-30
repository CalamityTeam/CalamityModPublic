using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class MoonSigil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 250;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 52)
            {
                Projectile.alpha += 5;
                Projectile.scale -= 0.013f;
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.alpha = 255;
                Projectile.Kill();
                return;
            }

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 8f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int cap = 5;
            float capDamageFactor = 0.05f;
            int excessCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] - cap;
            modifiers.SourceDamage *= MathHelper.Clamp(1f - (capDamageFactor * excessCount), 0f, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            float dustSp = 0.2f;
            int dustD = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Vector2 dustspeed = new Vector2(dustSp, dustSp).RotatedBy(MathHelper.ToRadians(dustD));
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 31, dustspeed.X, dustspeed.Y, 200, new Color(213, 242, 232, 200), 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center;
                    Main.dust[d].velocity = dustspeed;
                    dustSp += 0.2f;
                }
                dustD += 90;
                dustSp = 0.2f;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/MoonSigil").Value;
            Main.spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height - 20 * 0.5f
                ),
                new Rectangle(0, 0, 20, 20),
                Color.White,
                Projectile.rotation,
                new Vector2(10, 10),
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
