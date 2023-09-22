using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SummonBrimstoneExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Main.projFrames[Projectile.type] * 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                CreateInitialDust();
                Projectile.localAI[0] = 1f;
            }

            // Emit crimson light.
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 1.1f);
            if (Projectile.timeLeft % 5f == 4f)
                Projectile.frame++;
        }

        public void CreateInitialDust()
        {
            float randomnessSmoothness = 3f;
            for (int i = 0; i < 10; i++)
            {
                randomnessSmoothness += i;

                int offsetVariance = (int)(randomnessSmoothness * 4f);
                for (int j = 0; j < 30; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Square(-randomnessSmoothness, randomnessSmoothness).SafeNormalize(Vector2.UnitY) * randomnessSmoothness * 0.48f;
                    Dust dust = Dust.NewDustDirect(Projectile.Center, offsetVariance, offsetVariance, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.6f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                }
            }

            Projectile.Damage();
            Projectile.ExpandHitboxBy(48);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // Prevent absurd quantities of damage to the player.
            modifiers.SourceDamage *= 0.018f;
        }
    }
}
