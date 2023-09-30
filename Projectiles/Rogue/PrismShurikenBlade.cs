using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class PrismShurikenBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Projectile.MaxUpdates * 300;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, false, 850f, 19f, 30f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 15; i++)
            {
                Vector2 circularOffsetDirection = (MathHelper.TwoPi * i / 15f).ToRotationVector2().RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2);
                Vector2 spawnPosition = Projectile.Center - (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Projectile.height * 1.25f;
                spawnPosition += circularOffsetDirection * new Vector2(12f, 7f);

                Dust energy = Dust.NewDustPerfect(spawnPosition, 261);
                energy.velocity = circularOffsetDirection * new Vector2(3f, 2f);
                energy.color = Color.Cyan;
                energy.scale = 0.7f;
                energy.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }
    }
}
