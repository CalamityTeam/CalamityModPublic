using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class JewelSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0f;

            if (Main.rand.NextBool(5) && Projectile.frame < 3)
            {
                int crystalDust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 87, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[crystalDust].noGravity = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4 && Projectile.frame > 0)
            {
                Projectile.frame--;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame < 0)
                Projectile.frame = 0;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 3; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, Projectile.damage / 4, 0f, Projectile.owner);
                }
            }
        }
    }
}
