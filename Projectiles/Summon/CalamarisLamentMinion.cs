using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CalamarisLamentMinion : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<CalamarisLamentMinion>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<CalamarisLamentBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.CalamarisLament;

        // Increased to be able to target DoG reliably.
        public override float EnemyDistanceDetection => CalamarisLament.EnemyDistanceDetection;

        // It's an Abyss-origin summon, it deserves having visibility in Abyss areas.
        public override bool PreventTargettingUntilTargetHit => false;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public enum AIState { Idle, Shooting, Latching }
        public AIState State { get => (AIState)Projectile.ai[1]; set => Projectile.ai[1] = (int)value; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            // Half the size of one frame so it looks better while latching to a target.
            Projectile.width = Projectile.height = 31;
        }

        public override void MinionAI()
        {
            // Ambient idle sounds.
            if (Main.zenithWorld)
            {
                if (Main.rand.NextBool(750))
                {
                    SoundEngine.PlaySound(CalamarisLament.GFB, Projectile.Center);
                }
            }
            else
            {
                if (Main.rand.NextBool(600))
                {
                    SoundStyle glubNoise = Main.rand.NextBool() ? SoundID.Zombie35 : SoundID.Zombie34;
                    SoundStyle trollBirdChirpingSound = SoundID.Zombie16;

                    // 1/200th chance to do a bird chirping sound.
                    SoundEngine.PlaySound(Main.rand.NextBool(200) ? trollBirdChirpingSound : glubNoise, Projectile.Center);
                }
            }

            switch (State)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Shooting:
                    ShootingState();
                    break;
                case AIState.Latching:
                    LatchingState();
                    break;
            }
        }

        #region AI Methods

        private void IdleState()
        {
            // The minion will hover around the owner.
            if (!Projectile.WithinRange(Owner.Center, 320f))
            {
                Projectile.velocity = (Projectile.velocity + Projectile.SafeDirectionTo(Owner.Center)) * 0.9f;
                SyncVariables();
            }

            // The minion will teleport on the owner if they get far enough.
            if (!Projectile.WithinRange(Owner.Center, 1200f))
            {
                Projectile.Center = Owner.Center;
                SyncVariables();
            }

            Projectile.rotation = Projectile.rotation.AngleTowards(MathHelper.ToRadians(Projectile.velocity.X * 2f), .1f);
            Projectile.MinionAntiClump(0.5f);

            if (Target is not null)
                SwitchState(AIState.Shooting);
        }

        private void ShootingState()
        {
            if (Target is not null)
            {
                Vector2 toTargetDirection = Projectile.SafeDirectionTo(Target.Center);

                // The minion will hover around the owner more strongly.
                if (!Projectile.WithinRange(Owner.Center, 320f))
                {
                    float inertia = 8f;
                    Projectile.velocity = (Projectile.velocity * inertia + Projectile.SafeDirectionTo(Owner.Center) * 25f) / (inertia + 1f);
                    SyncVariables();
                }

                // A random chance to increase the fire rate timer a bit so they're not always synced and looks nicer.
                // In balance terms, this barely means anything, it's probably like a 5% DMG increase or less.
                ShootingTimer += Main.rand.NextBool(30) ? 2 : 1;
                if (ShootingTimer >= CalamarisLament.ShootingFireRate && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (toTargetDirection * (CalamarisLament.ShootingProjectileSpeed + 10f)).RotatedByRandom(MathHelper.PiOver4), ModContent.ProjectileType<CalamarisLamentProjectile>(), Projectile.damage, 1f, Projectile.owner, Target.whoAmI);

                    // Flavor recoil effect.
                    Projectile.velocity -= toTargetDirection * 3f;

                    // Flavor dust effect.
                    for (int i = 0; i < 15; i++)
                    {
                        Dust shootDust = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * Projectile.height / 2, 109, toTargetDirection.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 7f), Scale: Main.rand.NextFloat(0.5f, 1.5f), Alpha: 127);
                        shootDust.noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item111, Projectile.Center);

                    ShootingTimer = 0f;
                    SyncVariables();
                }

                Projectile.rotation = Projectile.rotation.AngleTowards(toTargetDirection.ToRotation() - MathHelper.PiOver2, .1f);
                Projectile.MinionAntiClump(0.5f);

                if (Owner.WithinRange(Target.Center, CalamarisLament.LatchingDistanceRequired))
                    SwitchState(AIState.Latching);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void LatchingState()
        {
            if (Target is not null)
            {
                Vector2 toTargetDirection = Projectile.SafeDirectionTo(Target.Center);

                // If the minion is not on the target's hithox, which means they're not "latched", go to the target.
                if (!Projectile.getRect().Intersects(Target.getRect()))
                {
                    float inertia = 10f;
                    Projectile.velocity = (Projectile.velocity * inertia + toTargetDirection * (Target.velocity.Length() + CalamarisLament.LatchingExtraTargettingSpeed)) / (inertia + 1f);
                    Projectile.rotation = Projectile.rotation.AngleTowards(toTargetDirection.ToRotation() - MathHelper.PiOver2, .3f);
                    Projectile.MinionAntiClump(0.5f);
                    SyncVariables();
                }

                // Deaccelerate fast but not instantly stop so it doesn't bug.
                else
                {
                    Projectile.velocity *= 0.2f;
                    SyncVariables();
                }

                if (!Owner.WithinRange(Target.Center, CalamarisLament.LatchingDistanceRequired))
                    SwitchState(AIState.Shooting);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void SwitchState(AIState state)
        {
            State = state;
            SyncVariables();
        }

        private void SyncVariables()
        {
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            IFrames = 30;

            int dustAmount = 40;
            for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
            {
                float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * 8f;
                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 109, velocity);
                spawnDust.noGravity = true;
                spawnDust.noLight = true;
            }
        }

        // The minion will only have contact damage if it's on latching mode.
        public override bool? CanDamage() => (State == AIState.Latching) ? null : false;

        // The minion will do 1.5x damage if it's latched on.
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= CalamarisLament.LatchingDamageMultiplier;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
