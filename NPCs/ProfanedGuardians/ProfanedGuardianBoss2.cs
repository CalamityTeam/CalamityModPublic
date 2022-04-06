using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss2 : ModNPC
    {
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Defender");
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
            NPC.defense = 50;
            NPC.DR_NERD(0.4f);
            NPC.LifeMaxNERB(43750, 52500, 30000); // Old HP - 40000, 50000
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
            writer.Write(healTimer);
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            healTimer = reader.ReadInt32();
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
            CalamityGlobalNPC.doughnutBossDefender = NPC.whoAmI;

            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool healerAlive = false;
            if (CalamityGlobalNPC.doughnutBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                    healerAlive = true;
            }

            // Healing
            if (healerAlive)
            {
                float healGateValue = 60f;
                healTimer++;
                if (healTimer >= healGateValue)
                {
                    healTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = NPC.lifeMax / 20;
                        if (healAmt > NPC.lifeMax - NPC.life)
                            healAmt = NPC.lifeMax - NPC.life;

                        if (healAmt > 0)
                        {
                            NPC.life += healAmt;
                            NPC.HealEffect(healAmt, true);
                            NPC.netUpdate = true;
                        }
                    }
                }
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

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
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

            bool biomeEnraged = biomeEnrageTimer <= 0;

            if (NPC.ai[0] == 0f)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                NPC.ai[3] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float divisor = (malice || biomeEnraged) ? 30f : death ? 40f : revenge ? 45f : expertMode ? 50f : 60f;
                    if (NPC.ai[3] % divisor == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                        int type = ModContent.ProjectileType<FlareDust>();
                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(player.Center - NPC.Center) * NPC.velocity.Length() * 1.5f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                    }
                }

                Vector2 targetVector = player.Center - NPC.Center;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                if (NPC.ai[3] >= phaseGateValue && !healerAlive)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[2] = targetVector.X;
                    NPC.ai[3] = targetVector.Y;
                    NPC.netUpdate = true;
                }

                Vector2 vector96 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num784 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector96.X;
                float num785 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector96.Y;
                float num786 = (float)Math.Sqrt(num784 * num784 + num785 * num785);

                if (num786 > (healerAlive ? 160f : 80f))
                {
                    num786 = (Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 5f) / num786;
                    num784 *= num786;
                    num785 *= num786;
                    NPC.velocity.X = (NPC.velocity.X * 25f + num784) / 26f;
                    NPC.velocity.Y = (NPC.velocity.Y * 25f + num785) / 26f;
                    return;
                }

                if (NPC.velocity.Length() < Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 5f)
                    NPC.velocity *= 1.1f;
            }
            else if (NPC.ai[0] == 1f)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                NPC.velocity *= 0.8f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 18f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                    velocity.Normalize();
                    velocity *= (malice || biomeEnraged) ? 32f : death ? 27f : revenge ? 24.5f : expertMode ? 22f : 17f;
                    NPC.velocity = velocity;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    float projectileGateValue = (malice || biomeEnraged) ? 30f : death ? 40f : revenge ? 45f : expertMode ? 50f : 60f;
                    if (NPC.localAI[0] >= projectileGateValue && Vector2.Distance(NPC.Center, player.Center) > 160f)
                    {
                        NPC.localAI[0] = 0f;

                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                        float velocity = 5f;
                        int totalProjectiles = 6;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<ProfanedSpear>();
                        int damage = NPC.GetProjectileDamage(type);
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }

                NPC.ai[1] += 1f;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                if (NPC.ai[1] >= phaseGateValue)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 24f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 targetVector = player.Center - NPC.Center;
                    targetVector.Normalize();
                    if (targetVector.HasNaNs())
                        targetVector = new Vector2(NPC.direction, 0f);

                    float inertia = (malice || biomeEnraged) ? 35f : death ? 40f : revenge ? 42f : expertMode ? 45f : 50f;
                    float num1006 = 0.111111117f * inertia;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector * (NPC.velocity.Length() + num1006)) / inertia;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] -= 1f;
                if (NPC.ai[1] <= 0f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;
            if (NPC.ai[0] == 2f)
                num153 = 10;

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

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss2Glow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

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
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<RelicOfResilience>(), 4);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 240, true);
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
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT3").Type, 1f);
                }
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
