using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SoulPiercerBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercer");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 240;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                Vector2 vector33 = projectile.position;
                vector33 -= projectile.velocity;
                projectile.alpha = 255;
                int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.014f;
                Main.dust[num448].velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int x = 0; x < 3; x++)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(projectile.Center, target.Center, true, -500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<SoulPiercerBolt>(), (int)(projectile.damage * 0.5), 0f, projectile.owner, false, 0f);
                }
            }
        }
    }
}
