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
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.momentumCapacitor)
                Projectile.Kill();
            if (Main.rand.NextBool(4))
            {
                float numDust = MathHelper.TwoPi * Radius / 5f;
                float angleIncrement = MathHelper.TwoPi / numDust;
                Vector2 dustOffset = Vector2.UnitX * Radius;
                dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
                for (int i = 0; i < (int)numDust; i++)
                {
                    dustOffset = dustOffset.RotatedBy(angleIncrement);
                    int dustIndex = Dust.NewDust(Projectile.Center, 1, 1, 226);
                    Main.dust[dustIndex].position = Projectile.Center + dustOffset;
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
                if (Main.projectile[k].owner == Projectile.owner && Main.projectile[k].Calamity().rogue &&
                    !Main.projectile[k].Calamity().momentumCapacitatorBoost && Main.projectile[k].friendly &&
                    Vector2.Distance(Main.projectile[k].Center, Projectile.Center) < Radius)
                {
                    Main.projectile[k].damage = (int)(Main.projectile[k].damage * 1.15f);
                    Main.projectile[k].Calamity().momentumCapacitatorBoost = true;
                }
            }
        }
    }
}
