using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float AITimer => ref Projectile.ai[0];

        public ref float TimerForShooting => ref Projectile.ai[1];

        public int ProbeIndex;

        public bool CheckForSpawning = false;

        public float ProbePositionAngle // Calculates where each minion should be placed.
        {
            get
            {
                float probeCount = Owner.ownedProjectileCounts[Type];
                if (probeCount <= 1f)
                    probeCount = 1f;

                return MathHelper.TwoPi * ProbeIndex / probeCount + AITimer * 0.025f;
                // "MathHelper.TwoPi / ProbeIndex * probeCount"s the position itself.
                // "AITimer * [Modifier]"s how fast it spins.
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;

            Projectile.width = 36;
            Projectile.height = 30;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.MinionHoming(2500f, Owner); // Detects a target at a certain distance.
            
            Vector2 idleDestination = Owner.Center + ProbePositionAngle.ToRotationVector2() * 150f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, idleDestination, 0.15f);
            AITimer++;
            // Makes the projectile be around the player and evenly spaced-out from eachother.

            CheckMinionExistince(); // Checks if the minion can still exist.
            SpawnEffect(); // Makes a dust spawn effect where the minion spawns.
            LookInCorrectDirection(target); // Looks at the target.
            ShootTarget(target); // Shoots at the target if there's one

            Projectile.netUpdate = true;
        }

        #region Methods

        public void CheckMinionExistince()
        {
            Owner.AddBuff(ModContent.BuffType<AstralProbeBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<AstralProbeSummon>())
            {
                if (Owner.dead)
                    moddedOwner.astralProbe = false;
                if (moddedOwner.astralProbe)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            if (CheckForSpawning == false)
            {
                int dustAmt = 120;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    float angle = MathHelper.TwoPi / dustAmt * dustIndex;
                    Vector2 velocity = angle.ToRotationVector2() * 10f;
                    int randomDust = Utils.SelectRandom(Main.rand, new int[]
                    {
                        ModContent.DustType<AstralOrange>(),
                        ModContent.DustType<AstralBlue>()
                    });

                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, randomDust, velocity);
                    spawnDust.customData = false;
                    spawnDust.noGravity = true;
                    spawnDust.velocity *= 0.3f;
                    spawnDust.scale = velocity.Length() * 0.1f;
                }
                CheckForSpawning = true;
            }
        }

        public void LookInCorrectDirection(NPC target) // I feel really fucking proud of how organized I made it. ~Memes
        {
            Vector2 lookHere = (target is not null) ? target.Center : Main.MouseWorld; // Looks at the mouse if there's no enemy, if there is one, it'll look at the enemy.
            int direction = (lookHere.X - Projectile.Center.X > 0).ToDirectionInt(); // If the target is at it's right, look at the right, if not, at it's left.
            float rotation = (lookHere - Projectile.Center).ToRotation(); // Points directly at the target.

            Projectile.spriteDirection = direction;
            Projectile.rotation = (direction == -1) ? rotation + MathHelper.Pi : rotation;
            // When the target is at the left, the sprite will flip, but not the rotation, meaning it'll be looking in the opposite direction, so we rotate it 180º degrees if so.

            Projectile.netUpdate = true;
        }

        public void ShootTarget(NPC target)
        {
            if (target != null && Main.myPlayer == Projectile.owner)
            {
                if (TimerForShooting == 80f)
                {
                    Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 35f);
                    
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<AstralProbeRound>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    SoundEngine.PlaySound(SoundID.Item12 with { Volume = SoundID.Item12.Volume * 0.5f, PitchVariance = 1f }, Projectile.position);

                    TimerForShooting = 0f; 
                    Projectile.netUpdate = true;
                }
                
                if (TimerForShooting < 80f)
                    TimerForShooting++;
            }
        }

        public override bool? CanDamage() => false;

        #endregion
    }
}
