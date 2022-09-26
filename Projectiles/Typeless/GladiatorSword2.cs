using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GladiatorSword2 : ModProjectile
    {
        private double rotation = 0D;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Sword");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool flag64 = Projectile.type == ModContent.ProjectileType<GladiatorSword2>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.gladiatorSword)
            {
                Projectile.active = false;
                return;
            }

            if (flag64)
            {
                if (player.dead)
                    modPlayer.glSword = false;
                if (modPlayer.glSword)
                    Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.15f, 0.15f, 0f);

            Vector2 vector = player.Center - Projectile.Center;
            Projectile.rotation = vector.ToRotation() - MathHelper.PiOver2;

            Projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);

            // Values are slightly different from the other sword to make this sword marginally slower so the intersection point isn't always at the same spot
            rotation -= 0.09;
            if (rotation <= 0D)
                rotation = 360D;

            Projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }
    }
}
