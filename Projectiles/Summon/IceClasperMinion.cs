using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class IceClasperMinion : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer ModdedOwner => Owner.Calamity();

        public enum IceClasperAIState
        {
            FollowOwner,
            Ram
        }

        public IceClasperAIState CurrentState
        {
            get => (IceClasperAIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public ref float TimerForShooting => ref Projectile.ai[1];

        public ref float AfterimageInterpolant => ref Projectile.localAI[0];

        public bool hasSpawned = false;
        
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = AncientIceChunk.IFrames;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.coldDamage = true;

            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.minion = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {   
            CheckMinionExistence();
            
            if (!hasSpawned)
            {
                SpawnEffect();
                hasSpawned = true;
            }

            // Detects a target at a given distance.
            NPC potentialTarget = Projectile.Center.MinionHoming(AncientIceChunk.EnemyDistanceDetection, Owner);
            switch (CurrentState)
            {
                case IceClasperAIState.FollowOwner:
                    State_FollowOwner(potentialTarget);
                    break;
                case IceClasperAIState.Ram:
                    State_Ram(potentialTarget);
                    break;
            }

            // Flavor dust visual to show that it is ghostly.
            if (Main.rand.NextBool(10))
            {
                int ghostDust = Dust.NewDust(Projectile.position, 31, 62, 56, -0.5f * Projectile.rotation.ToRotationVector2().X, -0.5f * Projectile.rotation.ToRotationVector2().Y);
                Main.dust[ghostDust].noLight = true;
                Main.dust[ghostDust].noLightEmittence = true;
            }

            DoAnimation();
            DoRotation(potentialTarget);
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());
            Projectile.MinionAntiClump(0.5f);
        }

        #region Methods

        public void State_FollowOwner(NPC target)
        {
            // If the minion starts to get far, force the minion to go to you.
            if (Projectile.WithinRange(Owner.Center, AncientIceChunk.EnemyDistanceDetection) && !Projectile.WithinRange(Owner.Center, AncientIceChunk.MaxDistanceFromOwner + 100f))
            {
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;
                Projectile.netUpdate = true;
            } 

            // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            else if (!Projectile.WithinRange(Owner.Center, AncientIceChunk.MaxDistanceFromOwner))
            {
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 40f;
                Projectile.netUpdate = true;
            }

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, AncientIceChunk.EnemyDistanceDetection))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity *= .3f;
                Projectile.netUpdate = true;
            }

            // If the target is not null but not in range to dash: shoot.
            // If in range to dash: dash.
            if (target is not null)
            {
                if (Owner.WithinRange(target.Center, AncientIceChunk.DistanceToDash))
                {
                    CurrentState = IceClasperAIState.Ram;
                    Projectile.netUpdate = true;
                }
                else
                    ShootTarget(target);
            }
        }

        public void State_Ram(NPC target)
        {
            // If there's no target while dashing or the player's gone far enough from the target: back to shooting.
            if (target is null || !Owner.WithinRange(Projectile.Center, AncientIceChunk.DistanceToStopDash))
            {
                CurrentState = IceClasperAIState.FollowOwner;
                Projectile.netUpdate = true;
            }
            else if (target is not null)
            {
                // The distance to the target plus a small number so it's not 0, it'd break calculations.
                float distToTarget = Projectile.Distance(target.Center) + .01f;

                // The minion will head towards it's rotation.
                // If the target's close, the minion'll speed up, and viceversa, so it doesn't circle around the target doing nothing.
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * (AncientIceChunk.MinVelocity + (12f / (distToTarget * .01f)));
                Projectile.velocity = Vector2.Clamp(Projectile.velocity, Vector2.One * -25f, Vector2.One * 25f);
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(target.Center), .001f * distToTarget);
            }
        }

        public void ShootTarget(NPC target)
        {
            if (TimerForShooting >= AncientIceChunk.TimeToShoot && Projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 20f);

                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                Projectile.Center, 
                velocity, 
                ModContent.ProjectileType<IceClasperSummonProjectile>(), 
                (int)(Projectile.damage * AncientIceChunk.ProjectileDMGMultiplier), 
                Projectile.knockBack, 
                Projectile.owner);

                // Flavor recoil effect.
                Projectile.velocity -= velocity * .1f;

                // For the fucking love of any god you can think of, this sound sucks but I can't find another one that fits better.
                SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);

                TimerForShooting = 0f;
                Projectile.netUpdate = true;
            }
            
            if (TimerForShooting < AncientIceChunk.TimeToShoot)
                TimerForShooting++;
        }

        public void DoRotation(NPC target)
        {
            if (target is null)
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.velocity.ToRotation(), .15f);    
            else if (target is not null && !Projectile.WithinRange(target.Center, AncientIceChunk.DistanceToDash))
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(target.Center), .15f);
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<IceClasperBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<IceClasperMinion>())
            {
                if (Owner.dead)
                    ModdedOwner.iClasper = false;
                if (ModdedOwner.iClasper)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            int dustAmt = 45;
            for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
            {
                float angle = MathHelper.TwoPi / dustAmt * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 56, velocity);
                spawnDust.customData = false;
                spawnDust.noGravity = true;
                spawnDust.velocity *= .75f;
                spawnDust.scale = velocity.Length() * .2f;
            }
        }

        public override bool? CanDamage() => (CurrentState == IceClasperAIState.Ram) ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            NPC target = Projectile.Center.MinionHoming(1200f, Owner);
            
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            // Flavor fade-in-and-out for afterimages.
            AfterimageInterpolant += ((target is not null && Owner.WithinRange(target.Center, 450f)) || CurrentState == IceClasperAIState.Ram) ? .05f : -.05f;
            AfterimageInterpolant = MathHelper.Clamp(AfterimageInterpolant, 0f, 1f);
            float AfterimageFade = MathHelper.Lerp(0f, 1f, AfterimageInterpolant);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = new Color(0.05f, 0.33f, 0.63f) with { A = 25 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length) * AfterimageFade;
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
            
            return false;
        }

        #endregion
    }
}
