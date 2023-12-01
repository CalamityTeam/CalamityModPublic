using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailShrapnel : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Vector2 rotationMult = new Vector2(6f, 12f);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustRotation = Vector2.UnitX * -15f;
                    dustRotation = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)i * 3.14159274f), default) * rotationMult * 0.75f;
                    int shrapnelDust = Dust.NewDust(Projectile.Center, 0, 0, 173, 0f, 0f, 160, default, 0.75f);
                    Main.dust[shrapnelDust].noGravity = true;
                    Main.dust[shrapnelDust].position = Projectile.Center + dustRotation;
                    Main.dust[shrapnelDust].velocity = Projectile.velocity;
                }
            }

            int extraDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1f);
            Main.dust[extraDust].noGravity = true;

            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 20f);
        }
    }
}
