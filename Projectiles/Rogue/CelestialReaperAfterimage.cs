using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CelestialReaperAfterimage : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CelestialReaper";

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 51;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = 180;
            Projectile.Calamity().CannotProc=true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(30f); // Buzzsaw scythe.

            if (Projectile.timeLeft < 150)
            {
                NPC target = Projectile.Center.ClosestNPCAt(640f);
                if (target != null)
                    Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(target.Center) * 20f) / 21f;
            }

            Projectile.alpha += 5;
        }
    }
}
