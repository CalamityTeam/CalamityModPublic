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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    public class ProfanedRocks : ModNPC
    {
        private int invinceTime = 180;
        private bool start = true;
        private const double MinDistance = 200D;
        private double distance = MinDistance;
        private const double MinMaxDistance = 300D;

        public const int MaxHP = 8000;
        public const int MaxBossRushHP = 20000;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Profaned Rocks");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.dontTakeDamage = true;
            NPC.width = 50;
            NPC.height = 50;
            NPC.defense = 100;
            NPC.lifeMax = BossRushEvent.BossRushActive ? MaxBossRushHP : MaxHP;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
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
                // Generate dust on spawn
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, 0, 0, 0, default, 1f);

                start = false;
                NPC.ai[3] = NPC.ai[0];
            }

            // Stay invincible for 3 seconds to avoid being instantly killed and don't deal damage to avoid unfair hits
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
            if ((Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] != 0f && Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[1] >= -60f) || NPC.Calamity().newAI[0] < 0f)
            {
                // For safety, always remain in this section of code regardless of what the defender is currently doing
                if (NPC.Calamity().newAI[0] > -1f)
                    NPC.Calamity().newAI[0] = -1f;

                // Get the Guardian Commander's target
                Player player = Main.player[Main.npc[CalamityGlobalNPC.doughnutBoss].target];

                // Distance the rock travels before it starts to fall down
                float maxChargeDistance = 4800f;
                float chargeSpeed = bossRush ? 20f : death ? 18f : revenge ? 17f : expertMode ? 16f : 14f;
                float fallDownGateValue = maxChargeDistance / chargeSpeed;

                // Fall down after some time and blow up if inside tiles
                if (NPC.Calamity().newAI[0] == -3f)
                {
                    // Accelerate towards final velocity
                    Vector2 finalVelocity = new Vector2(NPC.Calamity().newAI[2], NPC.Calamity().newAI[3]);
                    if (NPC.velocity.Length() < finalVelocity.Length())
                    {
                        NPC.velocity *= 1.05f;
                        if (NPC.velocity.Length() > finalVelocity.Length())
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= finalVelocity.Length();
                        }
                    }

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
                                SoundEngine.PlaySound(NPC.DeathSound.GetValueOrDefault(), NPC.Center);

                            NPC.life = 0;
                            NPC.HitEffect();
                            NPC.active = false;
                            NPC.netUpdate = true;
                        }
                    }
                }

                // Charge
                else if (NPC.Calamity().newAI[0] == -2f)
                {
                    // Start off slow
                    Vector2 finalVelocity = NPC.SafeDirectionTo(player.Center, -Vector2.UnitY) * chargeSpeed;
                    NPC.Calamity().newAI[2] = finalVelocity.X;
                    NPC.Calamity().newAI[3] = finalVelocity.Y;
                    NPC.velocity = finalVelocity * 0.1f;
                    NPC.rotation += 0.25f;
                    NPC.Calamity().newAI[0] = -3f;
                    NPC.netUpdate = true;
                }

                // Rotate
                else
                {
                    // Push away from each other in Death and Boss Rush
                    if (death)
                    {
                        float pushVelocity = bossRush ? 0.2f : 0.15f;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active)
                            {
                                if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                                {
                                    if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 160f)
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

                                    // Slow down so they don't push away from each other too far
                                    else
                                        NPC.velocity *= 0.95f;
                                }
                            }
                        }
                    }
                    else
                        NPC.velocity *= 0.95f;

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
                        NPC.Calamity().newAI[0] = -2f;
                        NPC.Calamity().newAI[1] = 0f;
                    }
                }

                return;
            }

            // Distance from Defender Guardian
            double maxDistance = bossRush ? 360D : death ? 340D : revenge ? 330D : expertMode ? 320D : MinMaxDistance;
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

            NPC.DrawBackglow(Color.Orange, 4f, SpriteEffects.None, frame, screenPos);

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

            return minDist <= (NPC.ai[2] == 6f ? 16f : 22f);
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
