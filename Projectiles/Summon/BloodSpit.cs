using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodSpit : ModProjectile
    {
        public const int OnDeathHealValue = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spit");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.77f, Projectile.Opacity * 0.15f, Projectile.Opacity * 0.08f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.frameCounter++ > 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 5, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 2f);
                dust.noGravity = true;
            }

            Main.player[Projectile.owner].HealEffect(OnDeathHealValue, false);
            Main.player[Projectile.owner].statLife += OnDeathHealValue;
            if (Main.player[Projectile.owner].statLife > Main.player[Projectile.owner].statLifeMax2)
                Main.player[Projectile.owner].statLife = Main.player[Projectile.owner].statLifeMax2;
        }
    }
}
