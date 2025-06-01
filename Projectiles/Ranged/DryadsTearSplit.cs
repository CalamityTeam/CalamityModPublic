using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DryadsTearSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 210;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 10;
            AIType = ProjectileID.Bullet;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[2] >= 30 && target.CanBeChasedBy(Projectile);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.6f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            dust.color = Main.rand.NextBool(3) ? Color.MediumAquamarine : Color.Lime;

            if (speed == 0f)
                speed = Projectile.velocity.Length();

            if (Projectile.ai[2] >= 30)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 500f, speed, 12f);
            }
        }
    }
}
