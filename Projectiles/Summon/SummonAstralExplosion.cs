using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SummonAstralExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 150;
            projectile.height = 150;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 30;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            // Bluish cyan light
            Lighting.AddLight(projectile.Center, 66f / 255f, 189f / 255f, 181f / 255f);
            float dustCount = 15f;
            dustCount *= 0.7f;
            projectile.ai[0] += 4f;
            int counter = 0;
            while (counter < dustCount)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralBlue>(),
                    ModContent.DustType<AstralOrange>()
                });
                Dust dust = Dust.NewDustPerfect(projectile.Center, dustType, Vector2.Zero, 100, default, 1.325f);
                dust.noGravity = true;
                dust.position += Utils.RandomVector2(Main.rand, -10f, 10f);
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1.5f, 5f);
                counter++;
            }
        }
    }
}
