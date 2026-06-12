using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlashRoundFlash : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 95;
            Projectile.height = 95;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.5f);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Utils.Distance(target.Center, Projectile.Center) <= Projectile.width)
            {
                Player Owner = Main.player[Projectile.owner];
                Vector2 launchVel = Utils.DirectionTo(Owner.Center, Projectile.Center);
                target.MoveNPC(launchVel, 5, false, Owner);

                Vector2 dustVel = Utils.DirectionTo(Projectile.Center, target.Center);
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<SquashDust>(), dustVel.RotatedByRandom(0.7f) * Main.rand.NextFloat(9f, 14f));
                    dust.scale = Main.rand.NextFloat(0.45f, 0.75f);
                    dust.noGravity = true;
                    dust.color = Color.White;
                    dust.noLightEmittence = true;
                }
            }
            return false;
        }
    }
}
