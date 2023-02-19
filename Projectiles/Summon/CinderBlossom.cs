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
        
        public float DelayBetweenShooting
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            CheckMinionExistance(); // Checks if the minion can still exist.
            EffectWhenSpawned(); // Makes a dust effect when it spawns
            
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Owner);
            if (Projectile.owner == Main.myPlayer)
            {
                if (potentialTarget != null)
                {
                    if (DelayBetweenShooting == 35f) // Shoots once every 35 frames.
                    {
                        Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, 20f);
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

            Projectile.Center = Owner.Center - Vector2.UnitY * 60f;
            Projectile.rotation += MathHelper.ToRadians((potentialTarget is not null) ? 3f * (potentialTarget.Center.X - Projectile.Center.X > 0).ToDirectionInt() : 1.5f * Owner.direction);
            // The projectile normally will spin in the direction of the player.
            // If an enemy is detected, it will spin faster, in the direction of where the enemy is.
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }

        public void CheckMinionExistance()
        {
            Owner.AddBuff(ModContent.BuffType<CinderBlossomBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<CinderBlossom>())
            {
                if (Owner.dead)
                {
                    moddedOwner.cinderBlossom = false;
                }
                if (moddedOwner.cinderBlossom)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        public void EffectWhenSpawned()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 6);
                    dust.noGravity = true;
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
                }
                Projectile.localAI[0] = 1f;
            }
        }

        public override bool? CanDamage() => false;
    }
}
