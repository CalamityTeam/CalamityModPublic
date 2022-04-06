using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss3 : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Healer");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 100;
            NPC.height = 80;
            NPC.defense = 30;
            NPC.DR_NERD(0.2f);
            NPC.LifeMaxNERB(25000, 30000, 20000); // Old HP - 25000, 35000
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            AIType = -1;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.doughnutBossHealer = NPC.whoAmI;

            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[Main.npc[CalamityGlobalNPC.doughnutBoss].target];

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            bool isHoly = player.ZoneHallow;
            bool isHell = player.ZoneUnderworldHeight;

            // Become immune over time if target isn't in hell or hallow
            if (!isHoly && !isHell && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
                else
                    NPC.Calamity().CurrentlyEnraged = true;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
            {
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            Vector2 vector96 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num784 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector96.X;
            float num785 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector96.Y;
            float num786 = (float)Math.Sqrt(num784 * num784 + num785 * num785);

            if (num786 > 320f)
            {
                num786 = (Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 3f) / num786;
                num784 *= num786;
                num785 *= num786;
                NPC.velocity.X = (NPC.velocity.X * 25f + num784) / 26f;
                NPC.velocity.Y = (NPC.velocity.Y * 25f + num785) / 26f;
                return;
            }

            if (NPC.velocity.Length() < Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 3f)
                NPC.velocity *= 1.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss3Glow2").Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss3Glow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            Color color42 = Color.Lerp(Color.White, Color.Violet, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                    Color color43 = color42;
                    color43 = Color.Lerp(color43, color36, amount9);
                    color43 = NPC.GetAlpha(color43);
                    color43 *= (num153 - num163) / 15f;
                    spriteBatch.Draw(texture2D16, vector44, NPC.frame, color43, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D16, vector43, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<RelicOfConvergence>(), 4);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH4").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH5").Type, 1f);
                }
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
