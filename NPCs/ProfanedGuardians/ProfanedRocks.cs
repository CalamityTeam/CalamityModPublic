using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    public class ProfanedRocks : ModNPC
    {
        private int invinceTime = 180;
        private bool start = true;
        private const double MinDistance = 80D;
        private double distance = MinDistance;
        private const double MinMaxDistance = 320D;

        public const int MaxHP = 12000;
        public const int MaxBossRushHP = 20000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Rocks");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.dontTakeDamage = true;
            NPC.width = 30;
            NPC.height = 30;
            NPC.scale = 1.5f;
            NPC.defense = 100;
            NPC.lifeMax = BossRushEvent.BossRushActive ? MaxBossRushHP : MaxHP;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<ProfanedGuardianDefender>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Profaned rocks summoned by a defender guardian to protect its commander.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(start);
            writer.Write(distance);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.noGravity);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            start = reader.ReadBoolean();
            distance = reader.ReadDouble();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.noGravity = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Set the degrees used for rotation
            if (start)
            {
                start = false;
                NPC.ai[3] = NPC.ai[0];
            }

            // Stay invincible for 3 seconds to avoid being instantly killed
            if (invinceTime > 0)
            {
                NPC.damage = 0;
                invinceTime--;
            }
            else
            {
                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.7f, 0.55f, 0f);

            // Force despawn if Defender Guardian isn't active
            if (CalamityGlobalNPC.doughnutBossDefender < 0 || !Main.npc[CalamityGlobalNPC.doughnutBossDefender].active || CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Set time left just in case something dumb happens with despawning
            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Spin and fly at the target
            if (Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] != 0f)
            {
                // Get the Guardian Commander's target
                Player player = Main.player[Main.npc[CalamityGlobalNPC.doughnutBoss].target];

                // Distance the rock travels before it starts to fall down
                float maxChargeDistance = 2400f;
                float chargeSpeed = bossRush ? 20f : death ? 18f : revenge ? 17f : expertMode ? 16f : 14f;
                float fallDownGateValue = maxChargeDistance / chargeSpeed;

                // Fall down after some time and blow up if inside tiles
                if (NPC.Calamity().newAI[0] == -2f)
                {
                    NPC.rotation += 0.25f;
                    NPC.Calamity().newAI[1] += 1f;
                    if (NPC.Calamity().newAI[1] >= fallDownGateValue)
                    {
                        NPC.noGravity = false;
                        NPC.velocity.Y += 0.1f;

                        // Die if insied any tiles
                        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            if (NPC.DeathSound.HasValue)
                                SoundEngine.PlaySound(NPC.DeathSound.GetValueOrDefault(), NPC.position);

                            NPC.life = 0;
                            NPC.HitEffect();
                            NPC.active = false;
                            NPC.netUpdate = true;
                        }
                    }
                }

                // Charge
                else if (NPC.Calamity().newAI[0] == -1f)
                {
                    NPC.velocity = NPC.SafeDirectionTo(player.Center, -Vector2.UnitY) * chargeSpeed;
                    NPC.rotation += 0.25f;
                    NPC.Calamity().newAI[0] = -2f;
                    NPC.netUpdate = true;
                }

                // Rotate
                else
                {
                    // Slow down so they don't push away from each other too far
                    NPC.velocity *= 0.95f;

                    // Push away from each other
                    float pushVelocity = bossRush ? 0.18f : death ? 0.16f : revenge ? 0.15f : expertMode ? 0.14f : 0.12f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                            {
                                if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 32f * NPC.scale)
                                {
                                    if (NPC.position.X < Main.npc[i].position.X)
                                        NPC.velocity.X -= pushVelocity;
                                    else
                                        NPC.velocity.X += pushVelocity;

                                    if (NPC.position.Y < Main.npc[i].position.Y)
                                        NPC.velocity.Y -= pushVelocity;
                                    else
                                        NPC.velocity.Y += pushVelocity;
                                }
                            }
                        }
                    }

                    // Rotate faster and charge
                    NPC.Calamity().newAI[1] += 1f;
                    float chargeGateValue = bossRush ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                    chargeGateValue += chargeGateValue * 0.5f * NPC.ai[1];
                    float anglularSpeed = NPC.Calamity().newAI[1] / chargeGateValue;
                    anglularSpeed = 0.05f + anglularSpeed * 0.2f;
                    NPC.rotation += anglularSpeed;

                    // Charge
                    if (NPC.Calamity().newAI[1] >= chargeGateValue)
                    {
                        NPC.netUpdate = true;
                        NPC.Calamity().newAI[0] = -1f;
                        NPC.Calamity().newAI[1] = 0f;
                    }
                }

                return;
            }

            // Distance from Defender Guardian
            double maxDistance = bossRush ? 380D : death ? 360D : revenge ? 350D : expertMode ? 340D : MinMaxDistance;
            double rateOfChangeIncrease = (maxDistance / MinMaxDistance) - 1D;
            double rateOfChange = (NPC.ai[1] * 0.5f) + 2D + rateOfChangeIncrease;
            if (NPC.Calamity().newAI[0] == 0f)
            {
                distance += rateOfChange;
                if (distance >= maxDistance)
                {
                    distance = maxDistance;
                    NPC.Calamity().newAI[0] = 1f;
                }
            }
            else
            {
                distance -= rateOfChange;
                if (distance <= MinDistance)
                {
                    distance = MinDistance;
                    NPC.Calamity().newAI[0] = 0f;
                }
            }

            // Rotation velocity
            float minRotationVelocity = 0.5f;
            float rotationVelocityIncrease = death ? 0.2f : revenge ? 0.15f : expertMode ? 0.1f : 0f;
            rotationVelocityIncrease += rotationVelocityIncrease * (NPC.ai[1] * 0.5f);

            // Rotate around Defender Guardian
            NPC parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<ProfanedGuardianDefender>())];
            double degrees = NPC.ai[3];
            double radians = degrees * (Math.PI / 180);
            NPC.position.X = parent.Center.X - (int)(Math.Cos(radians) * distance) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(radians) * distance) - NPC.height / 2;
            NPC.rotation = (float)radians;
            NPC.ai[3] += minRotationVelocity + rotationVelocityIncrease;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int npcType = (int)NPC.ai[2];
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedRocks" + npcType.ToString()).Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            spriteBatch.Draw(texture, drawPos, frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
                player.AddBuff(ModContent.BuffType<HolyFlames>(), 120, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= (NPC.ai[2] == 6f ? 10f : 14f) * NPC.scale;
        }

        public override bool CheckDead() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int npcType = (int)NPC.ai[2];
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedRocksGore" + npcType.ToString()).Type, NPC.scale);
                }

                for (int k = 0; k < 30; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
