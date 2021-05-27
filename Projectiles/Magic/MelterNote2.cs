using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MelterNote2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().ResistDamagePenaltyHarshness = 1.65f;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.99f;
            projectile.velocity.Y *= 0.99f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.02f;
                if (projectile.scale >= 1.25f)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale -= 0.02f;
                if (projectile.scale <= 0.75f)
                {
                    projectile.localAI[0] = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255);
        }
    }
}
