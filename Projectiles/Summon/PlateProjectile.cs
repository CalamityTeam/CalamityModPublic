using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class PlateProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plate");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.9995f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.01f;
            Projectile.rotation -= MathHelper.ToRadians(90) * Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 5)
            {
                Projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 3; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, Projectile.damage / 2, 0f, Projectile.owner);
                    if (shard.WithinBounds(Main.maxProjectiles))
                        Main.projectile[shard].DamageType = DamageClass.Summon;
                }
            }
        }
    }
}
