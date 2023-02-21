using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CinderBlossom : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float DelayBetweenShooting => ref Projectile.ai[0];

        public bool CheckForSpawning = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom");
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
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Owner);

            CheckMinionExistince(); // Checks if the minion can still exist.
            EffectWhenSpawned(); // Makes a dust effect when it spawns
            ShootTarget(potentialTarget); // Shoots at the target if there's one.

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
                    moddedOwner.cinderBlossom = false;
                if (moddedOwner.cinderBlossom)
                    Projectile.timeLeft = 2;
            }
        }

        public void EffectWhenSpawned()
        {
            if (CheckForSpawning == false)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 6);
                    dust.noGravity = true;
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
                }
                CheckForSpawning = true;
            }
        }

        public void ShootTarget(NPC target)
        {
            if (target != null && Projectile.owner == Main.myPlayer)
            {
                if (DelayBetweenShooting == 35f) // Shoots once every 35 frames.
                {
                    Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 20f);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<Cinder>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Projectile.originalDamage;

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
