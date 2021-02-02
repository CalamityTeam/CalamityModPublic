using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class MomentumCapacitorOrb : ModProjectile
    {
        public const float Radius = 180f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 4;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.momentumCapacitor)
                projectile.Kill();
            if (Main.rand.NextBool(4))
            {
                float numDust = MathHelper.TwoPi * Radius / 5f;
                float angleIncrement = MathHelper.TwoPi / numDust;
                Vector2 dustOffset = Vector2.UnitX * Radius;
                dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
                for (int i = 0; i < (int)numDust; i++)
                {
                    dustOffset = dustOffset.RotatedBy(angleIncrement);
                    int dustIndex = Dust.NewDust(projectile.Center, 1, 1, 226);
                    Main.dust[dustIndex].position = projectile.Center + dustOffset;
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Main.dust[dustIndex].velocity *= 0f;
                    Main.dust[dustIndex].scale = 0.5f;
                }
            }
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                if (Main.projectile[k].type == ModContent.ProjectileType<SlickCaneProjectile>() ||
                    Main.projectile[k].type == ModContent.ProjectileType<MalachiteProj>() ||
                    Main.projectile[k].type == ModContent.ProjectileType<DuneHopperProjectile>())
                    continue;
                if (Main.projectile[k].owner == projectile.owner && Main.projectile[k].Calamity().rogue &&
                    !Main.projectile[k].Calamity().momentumCapacitatorBoost && Main.projectile[k].friendly &&
                    Vector2.Distance(Main.projectile[k].Center, projectile.Center) < Radius)
                {
                    Main.projectile[k].damage = (int)(Main.projectile[k].damage * 1.15f);
                    Main.projectile[k].Calamity().momentumCapacitatorBoost = true;
                }
            }
        }
    }
}
