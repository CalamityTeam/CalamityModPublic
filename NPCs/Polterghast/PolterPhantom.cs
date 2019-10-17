using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    public class PolterPhantom : ModNPC
    {
        private int despawnTimer = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.damage = 210;
            npc.width = 90;
            npc.height = 120;
            npc.lifeMax = CalamityWorld.revenge ? 150000 : 130000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 225000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1100000 : 900000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            CalamityGlobalNPC.ghostBossClone = npc.whoAmI;

            npc.alpha -= 5;
            if (npc.alpha < 50)
                npc.alpha = 50;

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.5f, 0.25f, 0.75f);

            npc.TargetClosest(true);

            Vector2 vector = npc.Center;

            if (Vector2.Distance(Main.player[npc.target].Center, vector) > 6000f)
                npc.active = false;

            bool speedBoost1 = false;
            bool despawnBoost = false;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            if (npc.timeLeft < 1500)
                npc.timeLeft = 1500;

            int[] array2 = new int[4];
            float num730 = 0f;
            float num731 = 0f;
            int num732 = 0;
            int num;
            for (int num733 = 0; num733 < 200; num733 = num + 1)
            {
                if (Main.npc[num733].active && Main.npc[num733].type == ModContent.NPCType<PolterghastHook>())
                {
                    num730 += Main.npc[num733].Center.X;
                    num731 += Main.npc[num733].Center.Y;
                    array2[num732] = num733;
                    num732++;
                    if (num732 > 3)
                        break;
                }
                num = num733;
            }
            num730 /= (float)num732;
            num731 /= (float)num732;

            float num734 = 3f;
            float num735 = 0.03f;
            if (!Main.player[npc.target].ZoneDungeon && !CalamityWorld.bossRushActive && (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                speedBoost1 = true;
                num734 += 8f;
                num735 = 0.15f;
            }
            else
                despawnTimer++;

            if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < 300f)
            {
                num734 = 18f;
                num735 = 0.12f;
            }

            if (expertMode)
            {
                num734 += revenge ? 1.5f : 1f;
                num734 *= revenge ? 1.25f : 1.1f;
                num735 += revenge ? 0.015f : 0.01f;
                num735 *= revenge ? 1.2f : 1.1f;
            }

            Vector2 vector91 = new Vector2(num730, num731);
            float num736 = Main.player[npc.target].Center.X - vector91.X;
            float num737 = Main.player[npc.target].Center.Y - vector91.Y;

            if (despawnBoost)
            {
                num737 *= -1f;
                num736 *= -1f;
                num734 += 8f;
            }

            float num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));
            int num739 = 500;
            if (speedBoost1)
                num739 += 500;
            if (expertMode)
                num739 += 150;

            if (num738 >= (float)num739)
            {
                num738 = (float)num739 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(vector.X, vector.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));

            if (num738 < num734)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = num734 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            if (npc.velocity.X < num736)
            {
                npc.velocity.X = npc.velocity.X + num735;
                if (npc.velocity.X < 0f && num736 > 0f)
                    npc.velocity.X = npc.velocity.X + num735 * 2f;
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X = npc.velocity.X - num735;
                if (npc.velocity.X > 0f && num736 < 0f)
                    npc.velocity.X = npc.velocity.X - num735 * 2f;
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y = npc.velocity.Y + num735;
                if (npc.velocity.Y < 0f && num737 > 0f)
                    npc.velocity.Y = npc.velocity.Y + num735 * 2f;
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y = npc.velocity.Y - num735;
                if (npc.velocity.Y > 0f && num737 < 0f)
                    npc.velocity.Y = npc.velocity.Y - num735 * 2f;
            }

            Vector2 vector92 = new Vector2(vector.X, vector.Y);
            float num740 = Main.player[npc.target].Center.X - vector92.X;
            float num741 = Main.player[npc.target].Center.Y - vector92.Y;
            npc.rotation = (float)Math.Atan2((double)num741, (double)num740) + 1.57f;

            npc.damage = npc.defDamage;

            if (speedBoost1)
            {
                npc.damage *= 2;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 150, 255, npc.alpha);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (npc.frame.Y > frameHeight * 3)
            {
                npc.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
