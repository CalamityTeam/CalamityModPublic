using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ParticleBeamofDoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Doom");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                Vector2 vector33 = projectile.position;
                vector33 -= projectile.velocity * 0.25f;
                int num448 = Dust.NewDust(vector33, 1, 1, 206, 0f, 0f, 0, default, 2.5f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].velocity *= 0.1f;
            }
        }
    }
}
