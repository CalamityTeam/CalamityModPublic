using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class EndoRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 265;
            Projectile.coldDamage = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 5f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = Projectile.position;
                    spawnPosition -= Projectile.velocity * i * 0.25f;
                    int idx = Dust.NewDust(spawnPosition, 1, 1, 113, 0f, 0f, 0, default, 1.25f);
                    Main.dust[idx].position = spawnPosition;
                    Main.dust[idx].scale = Main.rand.NextFloat(0.71f, 0.93f);
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].noGravity = true;
                }
            }
        }
    }
}
