using System.Diagnostics.CodeAnalysis;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CinderBlossom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Owner.Center.MinionHoming(1200f, Owner, CalamityPlayer.areThereAnyDamnBosses);
        public ref float DelayBetweenShooting => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;

            Projectile.width = 42;
            Projectile.height = 42;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            CheckMinionExistince(); // Checks if the minion can still exist.
            ShootTarget(Target); // Shoots at the target if there's one.

            Projectile.Center = Owner.Center - Vector2.UnitY * 60f;
            Projectile.rotation += MathHelper.ToRadians(5f * Owner.direction); // The projectile normally will spin in the direction of the player.
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }

        public void CheckMinionExistince()
        {
            Owner.AddBuff(ModContent.BuffType<CinderBlossomBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<CinderBlossom>())
            {
                if (Owner.dead)
                    ModdedOwner.cinderBlossom = false;
                if (ModdedOwner.cinderBlossom)
                    Projectile.timeLeft = 2;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 36; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 6);
                dust.noGravity = true;
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
            }
        }

        public void ShootTarget(NPC target)
        {
            if (target != null && Projectile.owner == Main.myPlayer)
            {
                if (DelayBetweenShooting == 35f) // Shoots once every 35 frames.
                {
                    Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 20f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<Cinder>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    DelayBetweenShooting = 0f;
                    Projectile.netUpdate = true;
                }

                if (DelayBetweenShooting < 35f)
                    DelayBetweenShooting++;
            }
        }

        public override bool? CanDamage() => false;
    }
}
