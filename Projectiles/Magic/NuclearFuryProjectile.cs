using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NuclearFuryProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Fury");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.extraUpdates = 2;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                int num = 6;
                for (int index1 = 0; index1 < num; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width, (float)projectile.height) / 2f).RotatedBy((double)(index1 - (num / 2 - 1)) * Math.PI / (double)num, new Vector2()) + projectile.Center;
                    Vector2 vector2_2 = ((Main.rand.NextFloat() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 217, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Dust dust = Main.dust[index2];
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity /= 4f;
                    dust.velocity -= projectile.velocity;
                }
                projectile.alpha -= 5;
                if (projectile.alpha < 50)
                    projectile.alpha = 50;
                Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.1f, 0.4f, 0.6f);
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
            int num1 = -1;
            Vector2 targetVec = projectile.Center;
            float maxDistance = 500f;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            if (projectile.ai[0] == 0f && projectile.localAI[0] == 0f)
            {
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == (index + 1f)))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                        float npcDist = Vector2.Distance(npc.Center, targetVec);
                        if (npcDist < (maxDistance + extraDistance) && canHit)
                        {
                            maxDistance = npcDist;
                            targetVec = npc.Center;
                            num1 = index;
                        }
                    }
                }
                if (num1 >= 0)
                {
                    projectile.ai[0] = num1 + 1f;
                    projectile.netUpdate = true;
                }
            }
            if (projectile.localAI[0] == 0f && projectile.ai[0] == 0f)
                projectile.localAI[0] = 30f;
            bool flag = false;
            if (projectile.ai[0] != 0f)
            {
                int index = (int)(projectile.ai[0] - 1);
                if (Main.npc[index].active && !Main.npc[index].dontTakeDamage && Main.npc[index].immune[projectile.owner] == 0)
                {
                    if ((Math.Abs(projectile.Center.X - Main.npc[index].Center.X) + Math.Abs(projectile.Center.Y - Main.npc[index].Center.Y)) < 1000f)
                    {
                        flag = true;
                        targetVec = Main.npc[index].Center;
                    }
                }
                else
                {
                    projectile.ai[0] = 0f;
                    flag = false;
                    projectile.netUpdate = true;
                }
            }
            if (flag)
            {
                double num3 = (double)(targetVec - projectile.Center).ToRotation() - (double)projectile.velocity.ToRotation();
                if (num3 > Math.PI)
                    num3 -= 2.0 * Math.PI;
                if (num3 < -1.0 * Math.PI)
                    num3 += 2.0 * Math.PI;
                projectile.velocity = projectile.velocity.RotatedBy(num3 * 0.1, new Vector2());
            }
            float num4 = projectile.velocity.Length();
            projectile.velocity.Normalize();
            projectile.velocity = projectile.velocity * (num4 + 1f / 400f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 34, 0f, 0f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.buffImmune[BuffID.Wet] = false; //I'm not sorry
            target.AddBuff(BuffID.Wet, 600);
            if (projectile.ai[1] != 1f) //Nuclear Fury
            {
                target.immune[projectile.owner] = 5;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 600);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
