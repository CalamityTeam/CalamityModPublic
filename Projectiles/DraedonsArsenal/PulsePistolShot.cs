using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulsePistolShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC Target
        {
            get => Main.npc[(int)projectile.ai[0]];
            set => projectile.ai[0] = value.whoAmI;
        }
        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public List<NPC> NPCsAlreadyHit = new List<NPC>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Bolt");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 120 * (1 + projectile.extraUpdates);
        }

        public override void AI()
        {
            Time++;
            if (Time >= 10f || NPCsAlreadyHit.Count > 0)
            {
                if (NPCsAlreadyHit.Count > 0)
                {
                    float angleOffset = MathHelper.WrapAngle(projectile.AngleTo(Target.Center) - projectile.velocity.ToRotation());
                    angleOffset = MathHelper.Clamp(angleOffset, -0.2f, 0.2f);
                    projectile.velocity = projectile.velocity.RotatedBy(angleOffset);
                }
                float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.InverseLerp(10f, 50f, Time, true));

                Dust dust = Dust.NewDustPerfect(projectile.Center, 234);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
                dust.scale = 1.6f;

                // Only the initial shot should have a circle of dust, to prevent potential dust overload.
                if (NPCsAlreadyHit.Count == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float offsetRotationAngle = projectile.velocity.ToRotation() + Time / 20f;
                        float radius = (14f + (float)Math.Cos(Time / 13f) * 6f) * radiusFactor;
                        Vector2 dustPosition = projectile.Center;
                        dustPosition += offsetRotationAngle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                        dust = Dust.NewDustPerfect(dustPosition, Main.rand.NextBool(3) ? 234 : 173);
                        dust.noGravity = true;
                        dust.velocity = -projectile.velocity;
                        dust.scale = 0.8f;
                    }
                }
            }
        }
        public override bool CanDamage() => Time > projectile.MaxUpdates;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!NPCsAlreadyHit.Contains(target))
                NPCsAlreadyHit.Add(target);

            // If the maximum amount of targets have already been hit, die early.
            if (projectile.frameCounter >= 4)
            {
                projectile.Kill();
                return;
            }

            float minDistance = 600f;
            List<NPC> potentialTargets = new List<NPC>();
            for (int i = 0; i < Main.npc.Length; i++)
            {
                bool legalTarget = Main.npc[i] != target && Main.npc[i].active && Main.npc[i].CanBeChasedBy(null);
                float distanceToTarget = projectile.Distance(Main.npc[i].Center);
                if (legalTarget && distanceToTarget < minDistance && !NPCsAlreadyHit.Contains(Main.npc[i]))
                {
                    minDistance = distanceToTarget;
                    // The NPCs at the bottom will always be at the bottom of the list because it works via a minimum distance.
                    potentialTargets.Add(Main.npc[i]);
                }
            }

            if (potentialTargets.Count == 0)
            {
                projectile.Kill();
                return;
            }

            potentialTargets.Reverse(); // Move the closest NPCs to the top for convenience when indexing.
            int maxIndex = Math.Min(potentialTargets.Count, 2); // Pick 1 out of the 2 closest NPCs.
            NPC toHit = potentialTargets[Main.rand.Next(0, maxIndex)];

            // A netcode block such as Main.myPlayer == projectile.owner is not required. OnHitNPC is only run client-side.
            Projectile splitShot = Projectile.NewProjectileDirect(projectile.Center,
                                                                  projectile.velocity,
                                                                  projectile.type,
                                                                  projectile.damage,
                                                                  projectile.knockBack,
                                                                  projectile.owner,
                                                                  toHit.whoAmI);
            splitShot.frameCounter = projectile.frameCounter + 1;
            ((PulsePistolShot)splitShot.modProjectile).NPCsAlreadyHit.AddRange(NPCsAlreadyHit);
            ((PulsePistolShot)splitShot.modProjectile).NPCsAlreadyHit.Add(toHit);
            projectile.Kill();
        }
    }
}
