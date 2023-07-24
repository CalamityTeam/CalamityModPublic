using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StygianShieldThrown : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        // Stats
        public const int ReboundTime = 90;
        public const int MaxBounces = 1;
        public const float MaxHomingRange = 640f; // 40 blocks
        public const float ReturnPiercingDamageMult = 0.6f;

        private List<int> PreviousNPCs = new List<int>() { -1 };
        public Player Owner => Main.player[Projectile.owner];
        public ref float AirTime => ref Projectile.ai[0];

        public const float TotalTrailLength = 35f;
        public static readonly SoundStyle ThrowSound = CommonCalamitySounds.LouderSwingWoosh;
        public static readonly SoundStyle CollisionSound = CommonCalamitySounds.ExoHitSound;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = (int)TotalTrailLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (AirTime == 0f)
                SoundEngine.PlaySound(ThrowSound, Projectile.Center);

            // Boomerang rotation
            Projectile.rotation += Projectile.direction * 0.4f;

            // Trailing circular particles
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero && i % 7 == 3)
                {
                    float lengthRatio = i / TotalTrailLength;
                    Vector2 truePos = Projectile.oldPos[i] + (Projectile.Size * 0.5f);
                    Color redGradient = Color.Lerp(Color.LightPink, Color.Red, lengthRatio);
                    Color blackGradient = Color.Lerp(new Color(40, 40, 40), Color.Black, lengthRatio);
                    float rotMotion = Projectile.timeLeft * MathHelper.TwoPi / 60f;
                    float trueRot = Projectile.oldRot[i] + rotMotion;
                    Vector2 squishFactor = new Vector2 (0.8f, 1f);

                    Particle redSmear = new SemiCircularSmearVFX(truePos, redGradient, trueRot, 0.3f, squishFactor);
                    GeneralParticleHandler.SpawnParticle(redSmear);
                    Particle blackSmear = new SemiCircularSmearVFX(truePos, blackGradient, trueRot + MathHelper.Pi, 0.3f, squishFactor);
                    GeneralParticleHandler.SpawnParticle(blackSmear);
                }
            }

            // Returns after some number of frames in the air
            AirTime++;
            if (AirTime >= ReboundTime)
                ReturnToOwner();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CollisionSound, Projectile.Center);

            // Disallow the NPC to be targeted again
            PreviousNPCs.Add(target.whoAmI);
            if(SeekNPC() == -1)
                ReturnToOwner();

            // Return hits have diminishing damage
            if (Projectile.numHits >= MaxBounces)
                Projectile.damage = (int)(Projectile.damage * ReturnPiercingDamageMult);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(CollisionSound, Projectile.Center);

            if(SeekNPC() == -1)
                ReturnToOwner();

            // Counts as "hitting" something
            Projectile.numHits++;
            return false;
        }

        public int SeekNPC()
        {
            // Return if exceeding max number of bounces
            if (Projectile.numHits >= MaxBounces)
                return -1;

            // Find the closest NPC targetable
            float range = MaxHomingRange;
            int targetNPC = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (!target.CanBeChasedBy(Projectile) || PreviousNPCs.Contains(i))
                    continue;

                float distance = Vector2.Distance(target.Center, Projectile.Center);
                if (distance < range && Collision.CanHit(Projectile, target))
                {
                    range = distance;
                    targetNPC = i;
                }
            }

            // Move towards the target if found and reset airtime
            if (targetNPC != -1f)
            {
                AirTime = 0f;
                Projectile.velocity = Projectile.SafeDirectionTo(Main.npc[targetNPC].Center) * 15f;
            }
            return targetNPC;
        }

        public void ReturnToOwner()
        {
            // No more seeking
            AirTime = ReboundTime;
            Projectile.numHits = MaxBounces;
            Projectile.tileCollide = false;

            // Swiftly move back towards the player
            Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center) * 15f;

            // Delete the projectile if it touches its owner or too far away.
            if (Projectile.Hitbox.Intersects(Owner.Hitbox) || Vector2.Distance(Projectile.Center, Owner.Center) >= 3000f)
                Projectile.Kill();
        }
        
        // Preventing unintended collisions with the floor
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return true;
        }
    }
}
