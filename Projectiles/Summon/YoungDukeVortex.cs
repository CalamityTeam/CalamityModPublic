using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDukeVortex : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = 120;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 408;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.scale = .1f;
                Projectile.ai[0] = 1f;
            }

            if (Projectile.scale < 1f)
                Projectile.scale += .025f;

            Projectile.rotation += MathHelper.PiOver4 / 6;
        }
    }
}
