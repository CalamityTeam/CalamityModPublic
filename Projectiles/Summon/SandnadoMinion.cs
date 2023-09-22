using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SandnadoMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer ModdedOwner => Owner.Calamity();

        public ref float TimerForShooting => ref Projectile.ai[0];

        public bool CheckForSpawning = false;
        
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;

            Projectile.width = 40;
            Projectile.height = 43;

            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            NPC potentialTarget = Projectile.Center.MinionHoming(2000f, Owner); // Detects a nearby target at a given distance.

            if (potentialTarget is not null)
            {
                MoveToTarget(potentialTarget);
                ShootTarget(potentialTarget);
                // If there's a target, the minion will go towards the target and will shoot at it.
            }
            else
                Idle();

            CanMinionExist(); // Checks if the minion can still exist.
            OnSpawn(); // Does something when spawning, like a dust effect.
            DoAnimation(); // Does the animation of the minion.
            Projectile.MinionAntiClump(); // Prevents the minions from going on top of eachother.

            Projectile.netUpdate = true;
        }

        #region Methods

        public void CanMinionExist()
        {
            Owner.AddBuff(ModContent.BuffType<Sandnado>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<SandnadoMinion>())
            {
                if (Owner.dead)
                    ModdedOwner.sandnado = false;
                if (ModdedOwner.sandnado)
                    Projectile.timeLeft = 2;
            }
        }

        public void OnSpawn()
        {
            if (CheckForSpawning == false)
            {
                int dustAmount = 75;
                for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
                {
                    float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(5f, 6.5f);
                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 85, velocity);
                }
                CheckForSpawning = true;
            }
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 8 % Main.projFrames[Projectile.type];
        }

        public void Idle()
        {
            if (Projectile.WithinRange(Owner.Center, 1200f) && !Projectile.WithinRange(Owner.Center, 300f)) // If the minion starts to get far, force the minion to go to you.
            {
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;
                Projectile.netUpdate = true;
            }
            else if (!Projectile.WithinRange(Owner.Center, 160f)) // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            {
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 40f;
                Projectile.netUpdate = true;
            }

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, 1200f))
            {
                Projectile.position = Owner.Center;
                Projectile.velocity *= 0.3f;
                Projectile.netUpdate = true;
            }
        }

        public void MoveToTarget(NPC target)
        {
            Vector2 vecToTarget = target.Center - Projectile.Center;
            float targetDist = vecToTarget.Length();
            vecToTarget.Normalize();
            //If farther than 200 pixels, move toward it
            if (targetDist > 200f)
            {
                float speedMult = (targetDist > 400f) ? 12f : (targetDist > 250) ? 6f : 3f;
                vecToTarget *= speedMult;
                Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
            }
            //Otherwise, back it up slowly
            else
            {
                float speedMult = -3f;
                vecToTarget *= speedMult;
                Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
            }
        }

        public void ShootTarget(NPC target)
        {
            int randomDelay = Main.rand.Next(-5, 6);
            Vector2 targetPos = target.Center;
            Vector2 ProjVel = targetPos - Projectile.Center;
            ProjVel.Normalize();
            ProjVel *= SandSharknadoStaff.ProjSpeed;
            if (TimerForShooting == SandSharknadoStaff.FireSpeed + randomDelay && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    ProjVel,
                    ModContent.ProjectileType<MiniSandShark>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner);
                
                TimerForShooting = 0f;
            }
            
            if (TimerForShooting < SandSharknadoStaff.FireSpeed)
                TimerForShooting++;
        }

        public override bool? CanDamage() => false;

        #endregion
    }
}
