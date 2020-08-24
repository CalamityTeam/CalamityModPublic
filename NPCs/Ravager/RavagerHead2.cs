using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerHead2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.width = 80;
            npc.height = 80;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.dontTakeDamage = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.dontTakeDamage = false;
                npc.life = 0;
                HitEffect(npc.direction, 9999);
                npc.netUpdate = true;
                return;
            }
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            if (npc.timeLeft < 1800)
            {
                npc.timeLeft = 1800;
            }
            float num = 5f;
            float num2 = 0.1f;
            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (num7 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y += 0.023f;
                }
                else
                {
                    npc.velocity.Y -= 0.023f;
                }
                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X += 0.023f;
                }
                else
                {
                    npc.velocity.X -= 0.023f;
                }
                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }
            }
            if (Main.player[npc.target].dead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X += num2;
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X -= num2;
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y += num2;
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y -= num2;
            }
            npc.ai[1] += 1f;
            int nukeTimer = 720;
            if (npc.ai[1] >= (float)nukeTimer)
            {
                Main.PlaySound(SoundID.Item62, npc.position);
                npc.TargetClosest(true);
                npc.ai[1] = 0f;
                Vector2 shootFromVector = new Vector2(npc.Center.X, npc.Center.Y);
                float nukeSpeed = 1f;
                float playerDistanceX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - shootFromVector.X;
                float playerDistanceY = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - shootFromVector.Y;
                float totalPlayerDistance = (float)Math.Sqrt((double)(playerDistanceX * playerDistanceX + playerDistanceY * playerDistanceY));
                totalPlayerDistance = nukeSpeed / totalPlayerDistance;
                playerDistanceX *= totalPlayerDistance;
                playerDistanceY *= totalPlayerDistance;
                int nukeDamage = Main.expertMode ? 45 : 60;
                int projectileType = ModContent.ProjectileType<ScavengerNuke>();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, playerDistanceX, playerDistanceY, projectileType, nukeDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            npc.localAI[0] += 1f;
            if ((double)Main.npc[CalamityGlobalNPC.scavenger].life < (double)Main.npc[CalamityGlobalNPC.scavenger].lifeMax * 0.3 || death)
            {
                npc.localAI[0] += 1f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.scavenger].life < (double)Main.npc[CalamityGlobalNPC.scavenger].lifeMax * 0.1 || death)
            {
                npc.localAI[0] += 1f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.scavenger].life < (double)Main.npc[CalamityGlobalNPC.scavenger].lifeMax * 0.5 || death)
            {
                npc.localAI[1] += 1f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.scavenger].life < (double)Main.npc[CalamityGlobalNPC.scavenger].lifeMax * 0.25 || death)
            {
                npc.localAI[1] += 1f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 900f)
            {
                npc.localAI[0] = 0f;
                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    Main.PlaySound(SoundID.Item33, npc.position);
                    int num8 = 40;
                    int num9 = ModContent.ProjectileType<ScavengerLaser>();
                    Projectile.NewProjectile(vector.X, vector.Y, num4, num5, num9, num8 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[1] >= 30f)
            {
                npc.localAI[1] = 0f;
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    int num8 = 45;
                    int num9 = 259;
                    Projectile.NewProjectile(vector.X, vector.Y, num4, num5, num9, num8 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            int num10 = (int)npc.position.X + npc.width / 2;
            int num11 = (int)npc.position.Y + npc.height / 2;
            num10 /= 16;
            num11 /= 16;
            if (!WorldGen.SolidTile(num10, num11))
            {
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0f, 0.25f);
            }
            if (num4 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
            }
            if (num4 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + 3.14f;
            }
            float num12 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -num12;
                if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                {
                    npc.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < 12f)
                    {
                        npc.velocity.X *= 1.05f;
                    }
                }
                else
                {
                    npc.velocity.X *= 0.9f;
                }
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead3"), 1f);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
