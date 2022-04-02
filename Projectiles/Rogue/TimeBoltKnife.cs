using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TimeBoltKnife : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TimeBolt";

        private int maxPenetrate = 6;
        private int penetrationAmt = 6;
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Time Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = penetrationAmt;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(penetrationAmt);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            penetrationAmt = reader.ReadInt32();
        }

        public override void AI()
        {
            if (!initialized)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    maxPenetrate = 11;
                    penetrationAmt = maxPenetrate;
                }
                initialized = true;
            }

            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.03f;

            // If projectile hasn't hit anything yet
            if (projectile.ai[0] == 0f)
            {
                projectile.tileCollide = true;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 7f)
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        226,
                        229
                    });
                    Vector2 center = projectile.Center;
                    Vector2 vector74 = new Vector2(-4f, 4f);
                    vector74 += new Vector2(-4f, 4f);
                    vector74 = vector74.RotatedBy((double)projectile.rotation, default);
                    int dust = Dust.NewDust(center + vector74 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 1f);
                    Dust dust2 = Main.dust[dust];
                    dust2.velocity *= 0.1f;
                    if (Main.rand.Next(6) != 0)
                        dust2.noGravity = true;
                }
                float scalar = 0.01f;
                int alphaAmt = 5;
                int alphaCeiling = alphaAmt * 15;
                int alphaFloor = 0;
                if (projectile.localAI[0] > 7f)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.scale -= scalar;

                        projectile.alpha += alphaAmt;
                        if (projectile.alpha > alphaCeiling)
                        {
                            projectile.alpha = alphaCeiling;
                            projectile.localAI[1] = 1f;
                        }
                    }
                    else if (projectile.localAI[1] == 1f)
                    {
                        projectile.scale += scalar;

                        projectile.alpha -= alphaAmt;
                        if (projectile.alpha <= alphaFloor)
                        {
                            projectile.alpha = alphaFloor;
                            projectile.localAI[1] = 0f;
                        }
                    }
                }
            }

            // If projectile has hit an enemy and has 'split'
            else if (projectile.ai[0] >= 1f && projectile.ai[0] < (float)(1 + penetrationAmt))
            {
                projectile.tileCollide = false;
                projectile.alpha += 15;
                projectile.velocity *= 0.98f;
                projectile.localAI[0] = 0f;

                if (projectile.alpha >= 255)
                {
                    if (projectile.ai[0] == 1f)
                    {
                        projectile.Kill();
                        return;
                    }

                    int whoAmI = -1;
                    Vector2 targetSpot = projectile.Center;
                    float detectRange = 1000f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                            if (targetDist < detectRange)
                            {
                                detectRange = targetDist;
                                targetSpot = npc.Center;
                                whoAmI = i;
                            }
                        }
                    }

                    if (whoAmI >= 0)
                    {
                        projectile.netUpdate = true;
                        projectile.ai[0] += (float)penetrationAmt;
                        projectile.position = targetSpot + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * 100f - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
                        projectile.velocity = Vector2.Normalize(targetSpot - projectile.Center) * 18f;
                    }
                    else
                        projectile.Kill();
                }

                if (Main.rand.NextBool(3))
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        226,
                        229
                    });
                    Vector2 center = projectile.Center;
                    Vector2 vector75 = new Vector2(-4f, 4f);
                    vector75 += new Vector2(-4f, 4f);
                    vector75 = vector75.RotatedBy((double)projectile.rotation, default);
                    int dust = Dust.NewDust(center + vector75 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
                    Dust dust2 = Main.dust[dust];
                    dust2.velocity *= 0.1f;
                    dust2.noGravity = true;
                }
            }

            // If 'split' projectile has a target
            else if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
            {
                projectile.scale = 0.9f;
                projectile.tileCollide = false;

                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 15f)
                {
                    projectile.alpha += 51;
                    projectile.velocity *= 0.8f;

                    if (projectile.alpha >= 255)
                        projectile.Kill();
                }
                else
                {
                    projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.velocity *= 0.98f;
                }

                projectile.localAI[0] += 1f;

                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    226,
                    229
                });
                Vector2 center = projectile.Center;
                Vector2 vector76 = new Vector2(-4f, 4f);
                vector76 += new Vector2(-4f, 4f);
                vector76 = vector76.RotatedBy((double)projectile.rotation, default);
                int dust = Dust.NewDust(center + vector76 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 0.1f;
                dust2.noGravity = true;
            }

            float colorScale = (float)projectile.alpha / 255f;
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.3f * colorScale, 0.4f * colorScale, 1f * colorScale);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200) * ((255f - (float)projectile.alpha) / 255f);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = 1f;
            projectile.ai[1] = 0f;
            projectile.netUpdate = true;
            projectile.velocity = oldVelocity / 2f;

            if (penetrationAmt == maxPenetrate)
                SlowTime();

            penetrationAmt = 2;

            return false;
        }

        public override bool CanDamage()
        {
            // Do not do damage if a tile is hit OR if projectile has 'split' and hasn't been live for more than 5 frames
            if ((((int)(projectile.ai[0] - 1f) / penetrationAmt == 0 && penetrationAmt < 3) || projectile.ai[1] < 5f) && projectile.ai[0] != 0f)
                return false;
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (penetrationAmt == maxPenetrate)
                SlowTime();

            // If 'split' projectile hits an enemy
            if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
                projectile.ai[0] = 0f;

            // Becomes 5 on first hit, then 4, and so on
            penetrationAmt--;

            // Hits enemy, ai[0] = 0f + 4f = 4f on first hit
            // ai[0] = 4f - 1f = 3f on second hit
            // ai[0] = 3f - 1f = 2f on third hit
            if (projectile.ai[0] == 0f)
                projectile.ai[0] += (float)penetrationAmt;
            else
                projectile.ai[0] -= (float)(penetrationAmt + 1);

            projectile.ai[1] = 0f;
            projectile.netUpdate = true;
        }

        private void SlowTime()
        {
            Main.PlaySound(SoundID.Item114, projectile.Center);

            float radius = projectile.Calamity().stealthStrike ? 500f : 300f;
            int numDust = (int)(0.2f * MathHelper.TwoPi * radius);
            float angleIncrement = MathHelper.TwoPi / (float)numDust;
            Vector2 dustOffset = new Vector2(radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; i++)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    226,
                    229
                });
                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType);
                Main.dust[dust].position = projectile.Center + dustOffset;
                if (Main.rand.Next(6) != 0)
                    Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].scale = 0.3f;
            }

            int buffType = ModContent.BuffType<TimeSlow>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.buffImmune[buffType] && Vector2.Distance(projectile.Center, npc.Center) <= radius)
                {
                    if (npc.FindBuffIndex(buffType) == -1)
                        npc.AddBuff(buffType, 60, false);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
                target.AddBuff(ModContent.BuffType<TimeSlow>(), 120);
        }
    }
}
