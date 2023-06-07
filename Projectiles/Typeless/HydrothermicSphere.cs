using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class HydrothermicSphere : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 170 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 2f);
            fire.noGravity = true;
            fire.velocity = Vector2.Zero;

            if (Projectile.timeLeft < 170)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 9f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire3, 180);
    }
}
