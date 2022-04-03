using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class AlphaSeeker : ModProjectile
    {
        private const float MaxSpeed = 15f;
        private const float HomingStartRange = 400f;

        public static float moveSpeed = 2f;
        public static float rotateSpeed = 0.04f;
        public static int lifetime = 120;
        public static int returnTime = 60;
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alpha Virus Seeker");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = lifetime;
            Projectile.Calamity().rogue = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!initialized)
            {
                Projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                Projectile.localAI[1] = Main.rand.NextFloat(-rotateSpeed, rotateSpeed);
                initialized = true;
            }

            // ai[0], when set to 1, makes the projectile cling to a parent projectile instead.
            // in this case, ai[1] is the index of the parent projectile.
            // otherwise, ai[1] is the index of the homing target, because it homes in.
            if (Projectile.ai[0] == 1f)
            {
                // projectile.localAI[0] controls the distance from the parent projectile
                Projectile.tileCollide = false;

                Projectile parent = Main.projectile[0];
                bool active = false;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.identity == Projectile.ai[1] && p.active)
                    {
                        parent = p;
                        active = true;
                    }
                }

                if (active)
                {
                    Vector2 pos = new Vector2(0, Projectile.localAI[0]);
                    pos = pos.RotatedBy(Projectile.rotation);

                    Projectile.Center = parent.Center + pos;
                    Projectile.rotation += Projectile.localAI[1];

                    if (Projectile.timeLeft > returnTime)
                    {
                        Projectile.localAI[0] += moveSpeed;
                    }
                    else
                    {
                        Projectile.localAI[0] -= moveSpeed;
                    }

                    // Also look for an enemy to home in on every few frames. If you find one, switch to homing mode.
                    if (Projectile.timeLeft % 6 == 0)
                    {
                        int targetID = AcquireTarget();
                        if (targetID != -1)
                        {
                            Projectile.ai[0] = 0f; // Switch to homing mode
                            Projectile.ai[1] = targetID + 1f; // Already have a target loaded in
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                HomingAI();
            }

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity.Y = -0.15f;
        }

        // TODO -- shamelessly copied from Nanoblack Reaper. This should be made into a Utils function ASAP
        private void HomingAI()
        {
            // If we don't currently have a target, go try and get one!
            int targetID = (int)Projectile.ai[1] - 1;
            if (targetID < 0)
                targetID = AcquireTarget();

            // Save the target, whether we have one or not.
            Projectile.ai[1] = targetID + 1f;

            // If there's no target, just don't do anything. Otherwise home in.
            if (targetID < 0)
                return;
            NPC target = Main.npc[targetID];

            // Adds a multiple of the towards-target vector to its velocity every frame.
            float homingFactor = 2.5f;
            Vector2 posDiff = target.Center - Projectile.Center;
            posDiff = posDiff.SafeNormalize(Vector2.Zero);
            posDiff *= homingFactor;
            Vector2 newVelocity = Projectile.velocity += posDiff;

            // Caps speed to make sure it doesn't go too fast.
            if (newVelocity.Length() >= MaxSpeed)
            {
                newVelocity = newVelocity.SafeNormalize(Vector2.Zero);
                newVelocity *= MaxSpeed;
            }

            Projectile.velocity = newVelocity;
        }

        // TODO -- shamelessly copied from Nanoblack Reaper. This should be made into a Utils function ASAP
        private int AcquireTarget()
        {
            int target = -1;
            float minDist = HomingStartRange;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.type == NPCID.TargetDummy)
                    continue;

                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float xDist = Projectile.Center.X - npc.Center.X;
                    float yDist = Projectile.Center.Y - npc.Center.Y;
                    float distToNPC = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                    if (distToNPC < minDist)
                    {
                        minDist = distToNPC;
                        target = i;
                    }
                }
            }
            return target;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
