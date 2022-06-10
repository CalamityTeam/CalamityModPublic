using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class OceanBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 24)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 60f)
                Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 245, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
            }
        }
    }
}
