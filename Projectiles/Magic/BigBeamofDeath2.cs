using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BigBeamofDeath2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 80;
        }

        public override void AI()
        {
            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity * 0.25f;
            int beamDust = Dust.NewDust(projPos, 1, 1, 206, 0f, 0f, 0, default, 3f);
            Main.dust[beamDust].position = projPos;
            Main.dust[beamDust].velocity *= 0.1f;
        }
    }
}
