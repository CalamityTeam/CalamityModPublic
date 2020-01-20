using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;

namespace CalamityMod.NPCs.CeaselessVoid
{
    public class DarkEnergy3 : ModNPC
    {
        public int invinceTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.dontTakeDamage = true;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 50;
            npc.lifeMax = 6000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                npc.lifeMax = 24000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 44000;
            }
            double HPBoost = (double)CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0.3f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            if (invinceTime > 0)
            {
                invinceTime--;
            }
            else
            {
                npc.damage = expertMode ? 240 : 120;
                if (CalamityWorld.revenge)
                    npc.damage = 300;
                npc.dontTakeDamage = false;
            }

            double mult = 0.5 +
                (CalamityWorld.revenge ? 0.2 : 0.0) +
                (CalamityWorld.death ? 0.2 : 0.0);
            if ((double)npc.life < (double)npc.lifeMax * mult || CalamityWorld.bossRushActive)
            {
                npc.knockBackResist = 0f;
            }

            if (npc.ai[1] == 0f)
            {
                npc.scale -= 0.01f;
                npc.alpha += 15;
                if (npc.alpha >= 125)
                {
                    npc.alpha = 130;
                    npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.scale += 0.01f;
                npc.alpha -= 15;
                if (npc.alpha <= 0)
                {
                    npc.alpha = 0;
                    npc.ai[1] = 0f;
                }
            }
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            if (!player.active || player.dead || CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }

            float num1372 = expertMode ? 10f : 8f;
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
                num1372 += 2f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
                num1372 += 2f;

            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
            float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                if (projectile.type == ModContent.ProjectileType<MoltenAmputatorProj>())
                    damage = (int)((double)damage * 0.9);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
