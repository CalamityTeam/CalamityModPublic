using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class JewelSpike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.appliesImmunityTimeOnSingleHits = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0f;

            if (Main.rand.NextBool(5) && Projectile.frame < 3)
            {
                int crystalDust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemTopaz, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
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

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 2; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    float SpeedY = -Projectile.velocity.Y * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, Projectile.damage / 3, 0f, Projectile.owner);
                }
            }
        }
    }
}
