using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TarraEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 170 && target.CanBeChasedBy(Projectile);

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int cap = 2;
            float capDamageFactor = 0.05f;
            int excessCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] - cap;
            modifiers.SourceDamage *= MathHelper.Clamp(1f - (capDamageFactor * excessCount), 0f, 1f);
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                float x2 = Projectile.position.X - Projectile.velocity.X / 10f * (float)i;
                float y2 = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)i;
                Vector2 dspeed = Projectile.velocity * Main.rand.NextFloat(0.7f, 0.4f);
                int greenDust = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
                Main.dust[greenDust].alpha = Projectile.alpha;
                Main.dust[greenDust].position.X = x2;
                Main.dust[greenDust].position.Y = y2;
                Main.dust[greenDust].velocity = dspeed;
                Main.dust[greenDust].noGravity = true;
                Main.dust[greenDust].noLight = true;
            }

            if (Projectile.timeLeft < 170)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 9f, 20f);
        }
    }
}
