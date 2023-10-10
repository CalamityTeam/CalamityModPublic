using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SageNeedle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const int OnDeathHealValue = 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(48f);

            // Don't collide with tiles unless the needle is falling.
            Projectile.tileCollide = Projectile.velocity.Y > 0f;
            if (Projectile.velocity.Y < 12f)
                Projectile.velocity.Y += 0.16f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int totalSageSpirits = Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SageSpirit>()];

            // A slightly-smoothened curve that's a little less potent than a linear curve.
            // Has the potential to eventually surpass a linear curve, but this is infeasible, only happening
            // at around 344 allocated slots.
            float averageDamageMultiplier = (float)(Math.Pow(totalSageSpirits, 0.73D) + Math.Pow(totalSageSpirits, 1.1D)) * 0.5f;

            // 1 slot provides  25 DoT.
            // 2 slots provide  48 DoT.
            // 3 slots provide 70 DoT.
            // 4 slots provide 92 DoT.
            // 5 slots provide 114 DoT.
            int sagePoisonDamage = (int)(50 * averageDamageMultiplier);
            target.AddBuff(ModContent.BuffType<SagePoison>(), 300);
            target.Calamity().sagePoisonDamage = sagePoisonDamage;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 6; i++)
            {
                Dust redGrass = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Grass, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
                redGrass.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 3f);
                redGrass.noGravity = true;
                redGrass.color = Color.Lerp(Color.IndianRed, Color.MediumVioletRed, Main.rand.NextFloat());
            }
        }
    }
}
