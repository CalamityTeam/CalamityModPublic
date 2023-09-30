using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CatastropheClaymoreSparkle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float ProjectileType => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            int dustType = (ProjectileType == 2f ? 57 : ProjectileType == 1f ? 56 : 73); // Frostbite, Ichor, Hellfire respectively
            for (int i = 0; i < 3; i++)
            {
                Dust trail = Dust.NewDustPerfect(Projectile.Center, dustType);
                trail.noGravity = true;
                trail.scale = 1.35f;
                trail.velocity = Projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (ProjectileType)
            {
                case 1f:
                    target.AddBuff(BuffID.Frostburn2, 90);
                    break;
                case 2f:
                    target.AddBuff(BuffID.Ichor, 60);
                    break;
                default:
                    target.AddBuff(BuffID.OnFire3, 120);
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            int dustType = (ProjectileType == 2f ? 57 : ProjectileType == 1f ? 56 : 73); // Frostbite, Ichor, Hellfire respectively
            float effectiveVelocity = Projectile.velocity.Length() * Projectile.MaxUpdates;
            for (int i = 0; i < 60; i++)
            {
                Dust boom = Dust.NewDustPerfect(Projectile.Center, dustType);
                boom.noGravity = true;
                boom.scale = Main.rand.NextFloat(0.8f, 1.7f);
                boom.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.Pi / 6f) * Main.rand.NextFloat(-2.4f, -1.2f) * effectiveVelocity;
            }
        }
    }
}
