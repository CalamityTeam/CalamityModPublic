using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SageNeedle : ModProjectile
    {
        public const int OnDeathHealValue = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sage Needle");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 150;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(48f);

            // Don't collide with tiles unless the needle is falling.
            projectile.tileCollide = projectile.velocity.Y > 0f;
            if (projectile.velocity.Y < 12f)
                projectile.velocity.Y += 0.16f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int totalSageSpirits = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SageSpirit>()];

            // A slightly-smoothened curve that's a little less potent than a linear curve.
            // Has the potential to eventually surpass a linear curve, but this is infeasible, only happening
            // at around 344 allocated slots.
            float averageDamageMultiplier = (float)(Math.Pow(totalSageSpirits, 0.73D) + Math.Pow(totalSageSpirits, 1.1D)) * 0.5f;

            // 1 slot provides  40 DoT.
            // 2 slots provide  76 DoT.
            // 3 slots provide 111 DoT.
            // 4 slots provide 146 DoT.
            // 5 slots provide 182 DoT.
            int sagePoisonDamage = (int)(40 * averageDamageMultiplier);
            target.AddBuff(ModContent.BuffType<SagePoison>(), 300);
            target.Calamity().sagePoisonDamage = sagePoisonDamage;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 6; i++)
            {
                Dust redGrass = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Grass, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
                redGrass.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 3f);
                redGrass.noGravity = true;
                redGrass.color = Color.Lerp(Color.IndianRed, Color.MediumVioletRed, Main.rand.NextFloat());
            }
        }
    }
}
