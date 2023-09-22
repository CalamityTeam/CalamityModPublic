using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SiriusMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float TimerForShooting => ref Projectile.ai[0];

        public bool CheckForSpawning = false;
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 48;

            Projectile.minionSlots = 6f;
            Projectile.penetrate = -1;

            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.MinionHoming(5000f, Owner); // Constantly tries to find a target.

            CheckMinionExistince(); // Checks if the minion can still exist.
            SpawnEffect(); // Does a dust spawn effect.
            ShootTarget(target); // If there's a target, shoot at the target.

            Projectile.Center = Owner.Center - Vector2.UnitY * 60f; // Stays above the player.

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f); // Passively makes blue light.

            // The timer for the minion shooting.
            if (TimerForShooting < 60f)
                TimerForShooting++;
            else
                TimerForShooting = 0f;

            // Makes the star oscillate.
            Projectile.scale = 0.8f + MathF.Abs(MathF.Cos(MathHelper.Pi * (TimerForShooting / 50f))) / 5f;
        }

        #region Methods

        public void CheckMinionExistince()
        {
            Owner.AddBuff(ModContent.BuffType<SiriusBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<SiriusMinion>())
            {
                if (Owner.dead)
                    moddedOwner.sirius = false;
                if (moddedOwner.sirius)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            if (CheckForSpawning == false)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    float angle = MathHelper.TwoPi / dustAmt * d;
                    Vector2 dustVelocity = angle.ToRotationVector2() * 20f;
                    Dust spawnDust = Dust.NewDustPerfect(Owner.Center - Vector2.UnitY * 60f, 20, dustVelocity);
                    spawnDust.noGravity = true;
                }
                CheckForSpawning = true;
            }
        }

        public void ShootTarget(NPC target)
        {            
            if (target is not null)
            {
                if (TimerForShooting >= 60f && Projectile.owner == Main.myPlayer)
                {
                    // Makes a dust effect on the minion, to make a better effect of it shooting.
                    int dustAmt = 50;
                    for (int d = 0; d < dustAmt; d++)
                    {
                        float angle = MathHelper.TwoPi / dustAmt * d;
                        Vector2 dustVelocity = angle.ToRotationVector2() * 20f;
                        Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 20, dustVelocity);
                        spawnDust.noGravity = true;
                    }

                    // Shoots the beam.
                    Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SiriusBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool? CanDamage() => false;

        #endregion
    }
}
