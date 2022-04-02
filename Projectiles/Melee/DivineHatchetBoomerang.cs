using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DivineHatchetBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/DivineHatchet";

        private bool hasHitEnemy = false;
        private static int Lifetime = 300;
        private static int ReboundTime = 100;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorching Seeker");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 4;
            projectile.timeLeft = Lifetime;
            projectile.melee = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //holy dust
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            // Boomerang rotation
            projectile.rotation += 0.4f * (float)projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Returns after some number of frames in the air
            if (projectile.timeLeft < Lifetime - ReboundTime)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] != 0f)
            {
                float returnSpeed = 26f;
                float acceleration = 1.4f;

                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();

                //home in on nearby NPCs if not returning to player
                if (projectile.penetrate != -1)
                {
                    if (!hasHitEnemy)
                    {
                        float range = 999f;
                        for (int a = 0; a < Main.npc.Length; a++)
                        {
                            if (Main.npc[a].CanBeChasedBy(projectile, false))
                            {
                                if (Vector2.Distance(Main.npc[a].Center, projectile.Center) < range)
                                {
                                    Vector2 newVelocity = Main.npc[a].Center - projectile.Center;
                                    newVelocity.Normalize();
                                    newVelocity *= 15f;
                                    projectile.velocity = newVelocity;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (targetNPC >= 0)
                        {
                            Vector2 newVelocity = Main.npc[targetNPC].Center - projectile.Center;
                            newVelocity.Normalize();
                            newVelocity *= 15f;
                            projectile.velocity = newVelocity;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //inflict Holy Flames for 6 seconds
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            if (projectile.penetrate != -1)
            {
                //find a nearby NPC to track
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    bool hasHitNPC = false;
                    for (int j = 0; j < previousNPCs.Count; j++)
                    {
                        if (previousNPCs[j] == i)
                        {
                            hasHitNPC = true;
                        }
                    }

                    NPC npc = Main.npc[i];
                    if (npc == target)
                    {
                        previousNPCs.Add(i);
                    }
                    if (npc.CanBeChasedBy(projectile, false) && npc != target && !hasHitNPC)
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    hasHitEnemy = true;
                    targetNPC = index;
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    velocityNew *= 15f;
                    projectile.velocity = velocityNew;
                }
                else
                {
                    projectile.penetrate = -1;
                    projectile.ai[0] = 1f;
                    targetNPC = -1;
                }
            }

            // After its last hit, starts returning instead of vanishing. Can pierce infinitely on the way back.
            if (projectile.penetrate == 1)
            {
                projectile.penetrate = -1;
                projectile.ai[0] = 1f;
                targetNPC = -1;
            }
        }
    }
}
