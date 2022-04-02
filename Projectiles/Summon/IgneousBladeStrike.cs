using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class IgneousBladeStrike : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/IgneousBlade";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 86;
            projectile.height = 36;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.timeLeft = 360;
            projectile.alpha = 127;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(28, 41); i++)
            {
                Dust.NewDustPerfect(
                    projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                    6,
                    Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
            }
            Main.PlaySound(SoundID.Item14, projectile.Center);
        }
    }
}
