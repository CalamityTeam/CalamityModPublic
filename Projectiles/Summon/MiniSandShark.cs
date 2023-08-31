using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class MiniSandShark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;

            Projectile.width = Projectile.height = 32;

            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation(); // The projectile looks towards where it's going.
            
            Projectile.netUpdate = true;
        }

        public override void Kill(int timeLeft) // When the projectile hits an enemy, it'll make a dust impact effect.
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage = (int)(Projectile.damage * 0.75f);
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int dustIndex = 0; dustIndex < 36; dustIndex++)
            {
                Dust.NewDust(Projectile.Center,
                    Projectile.width,
                    Projectile.height,
                    85,
                    Projectile.velocity.X / 2f,
                    Projectile.velocity.Y / 2f);
            }
        }
    }
}
