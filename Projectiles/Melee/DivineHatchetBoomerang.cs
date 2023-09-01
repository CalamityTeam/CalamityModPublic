using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DivineHatchetBoomerang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/SeekingScorcher";

        private bool hasHitEnemy = false;
        private static int Lifetime = 300;
        private static int ReboundTime = 100;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //holy dust
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            // Boomerang rotation
            Projectile.rotation += 0.4f * (float)Projectile.direction;

            // Boomerang sound
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // Returns after some number of frames in the air
            if (Projectile.timeLeft < Lifetime - ReboundTime)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] != 0f)
            {
                float returnSpeed = 26f;
                float acceleration = 1.4f;

                Player owner = Main.player[Projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    Projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();

                //home in on nearby NPCs if not returning to player
                if (Projectile.penetrate != -1)
                {
                    if (!hasHitEnemy)
                    {
                        float range = 999f;
                        for (int a = 0; a < Main.npc.Length; a++)
                        {
                            if (Main.npc[a].CanBeChasedBy(Projectile, false))
                            {
                                if (Vector2.Distance(Main.npc[a].Center, Projectile.Center) < range)
                                {
                                    Vector2 newVelocity = Main.npc[a].Center - Projectile.Center;
                                    newVelocity.Normalize();
                                    newVelocity *= 15f;
                                    Projectile.velocity = newVelocity;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (targetNPC >= 0)
                        {
                            Vector2 newVelocity = Main.npc[targetNPC].Center - Projectile.Center;
                            newVelocity.Normalize();
                            newVelocity *= 15f;
                            Projectile.velocity = newVelocity;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //inflict Holy Flames for 6 seconds
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            if (Projectile.penetrate != -1)
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
                    if (npc.CanBeChasedBy(Projectile, false) && npc != target && !hasHitNPC)
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
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
                    velocityNew = Main.npc[index].Center - Projectile.Center;
                    velocityNew.Normalize();
                    velocityNew *= 15f;
                    Projectile.velocity = velocityNew;
                }
                else
                {
                    Projectile.penetrate = -1;
                    Projectile.ai[0] = 1f;
                    targetNPC = -1;
                }
            }

            // After its last hit, starts returning instead of vanishing. Can pierce infinitely on the way back.
            if (Projectile.penetrate == 1)
            {
                Projectile.penetrate = -1;
                Projectile.ai[0] = 1f;
                targetNPC = -1;
            }
        }
    }
}
