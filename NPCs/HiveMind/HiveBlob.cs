using System;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveBlob : ModNPC
    {
        private const float NormalShootGate = 240f;
        private const float FastShootGate = 180f;
        private const float TelegraphDuration = 120f;
        private bool FastVariant => NPC.ai[2] > 0f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public static int VileClotDamage = 8; // 32

        // Legendary Mode exclusive
        public static int CursedFlameDamage = 15; // 60

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.1f;
            NPC.aiStyle = -1;
            NPC.damage = 0; // No contact damage
            NPC.width = 25;
            NPC.height = 25;

            NPC.lifeMax = 50;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 1300;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;

            NPC.knockBackResist = 0.9f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<HiveMind>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.HiveBlob")
            });
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool getFuckedAI = Main.zenithWorld;

            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // When Hive Mind starts flying around
            bool phase2 = Main.npc[hiveMind].life / (float)Main.npc[hiveMind].lifeMax < 0.8f;

            if (phase2)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            NPC.alpha = Main.npc[hiveMind].alpha;

            if (NPC.ai[3] > 0f)
                hiveMind = (int)NPC.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= getFuckedAI ? 10f : 1f; //Relocation rate
                float RandomPositionMultiplier = getFuckedAI ? 4f : 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(180, 361);
                    NPC.ai[0] = Main.rand.Next(-100, 101) * RandomPositionMultiplier; //X position
                    NPC.ai[1] = Main.rand.Next(-100, 101) * RandomPositionMultiplier; //Y position
                    NPC.netUpdate = true;
                }
            }

            float relocateSpeed = getFuckedAI ? 1.2f : death ? 0.8f : revenge ? 0.7f : expertMode ? 0.6f : 0.5f;
            float acceleration = 0.8f;
            float distanceFromMind = FastVariant ? 96f : 128f;
            if (Main.getGoodWorld)
                distanceFromMind *= 2;

            float hiveMindX = Main.npc[hiveMind].Center.X;
            float hiveMindY = Main.npc[hiveMind].Center.Y;
            Vector2 hiveMindPos = new Vector2(hiveMindX, hiveMindY);
            float randomPosX = hiveMindX + NPC.ai[0];
            float randomPosY = hiveMindY + NPC.ai[1];
            float finalRandPosX = randomPosX - hiveMindPos.X;
            float finalRandPosY = randomPosY - hiveMindPos.Y;
            float finalRandDistance = (float)Math.Sqrt(finalRandPosX * finalRandPosX + finalRandPosY * finalRandPosY);
            finalRandDistance = distanceFromMind / finalRandDistance;
            finalRandPosX *= finalRandDistance;
            finalRandPosY *= finalRandDistance;

            if (NPC.position.X < hiveMindX + finalRandPosX)
            {
                NPC.velocity.X += relocateSpeed;
                if (NPC.velocity.X < 0f && finalRandPosX > 0f)
                    NPC.velocity.X *= acceleration;
            }
            else if (NPC.position.X > hiveMindX + finalRandPosX)
            {
                NPC.velocity.X -= relocateSpeed;
                if (NPC.velocity.X > 0f && finalRandPosX < 0f)
                    NPC.velocity.X *= acceleration;
            }
            if (NPC.position.Y < hiveMindY + finalRandPosY)
            {
                NPC.velocity.Y += relocateSpeed;
                if (NPC.velocity.Y < 0f && finalRandPosY > 0f)
                    NPC.velocity.Y *= acceleration;
            }
            else if (NPC.position.Y > hiveMindY + finalRandPosY)
            {
                NPC.velocity.Y -= relocateSpeed;
                if (NPC.velocity.Y > 0f && finalRandPosY < 0f)
                    NPC.velocity.Y *= acceleration;
            }

            float velocityLimit = relocateSpeed * 16f;
            if (NPC.velocity.X > velocityLimit)
                NPC.velocity.X = velocityLimit;
            if (NPC.velocity.X < -velocityLimit)
                NPC.velocity.X = -velocityLimit;
            if (NPC.velocity.Y > velocityLimit)
                NPC.velocity.Y = velocityLimit;
            if (NPC.velocity.Y < -velocityLimit)
                NPC.velocity.Y = -velocityLimit;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float shootGateValue = FastVariant ? FastShootGate : NormalShootGate;
                if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[Main.npc[hiveMind].target].position, Main.player[Main.npc[hiveMind].target].width, Main.player[Main.npc[hiveMind].target].height))
                    NPC.localAI[1] = shootGateValue * 0.5f;

                if (NPC.localAI[1] < shootGateValue)
                {
                    NPC.localAI[1] += 1f;
                    if (NPC.localAI[1] < shootGateValue - TelegraphDuration)
                        NPC.localAI[1] += Main.rand.Next(2);
                    if (death)
                        NPC.localAI[1] += 1f;
                }

                if (NPC.alpha > 0)
                    return;

                if (NPC.localAI[1] >= shootGateValue && Vector2.Distance(Main.player[Main.npc[hiveMind].target].Center, NPC.Center) > 80f)
                {
                    NPC.localAI[1] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[Main.npc[hiveMind].target].position, Main.player[Main.npc[hiveMind].target].width, Main.player[Main.npc[hiveMind].target].height))
                    {
                        float projSpeed = death ? 8f : revenge ? 7f : expertMode ? 6f : 4f;
                        if (Main.getGoodWorld)
                            projSpeed *= 1.5f;

                        Vector2 projDirection = NPC.Center;
                        float playerX = Main.player[Main.npc[hiveMind].target].Center.X - projDirection.X;
                        float playerY = Main.player[Main.npc[hiveMind].target].Center.Y - projDirection.Y;
                        float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                        playerDist = projSpeed / playerDist;
                        playerX *= playerDist;
                        playerY *= playerDist;
                        int type = (Main.getGoodWorld && Main.rand.NextBool(5)) ? ProjectileID.CursedFlameHostile : ModContent.ProjectileType<VileClot>();
                        int damage = type == ProjectileID.CursedFlameHostile ? CursedFlameDamage : VileClotDamage;
                        Vector2 projectileVelocity = new Vector2(playerX, playerY);
                        if (type == ProjectileID.CursedFlameHostile)
                        {
                            Vector2 v = Main.player[Main.npc[hiveMind].target].Center - NPC.Center - Main.player[Main.npc[hiveMind].target].velocity * 20f;
                            projectileVelocity = v.SafeNormalize(Vector2.UnitY) * projSpeed;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projDirection, projectileVelocity, type, damage, 0f, Main.myPlayer);
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        public override bool CanHitNPC(NPC target) => NPC.alpha == 0; // Can only be hit while fully visible

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);

            Vector2 vector2 = NPC.Center - screenPos;
            vector2 -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            vector2 += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            float shootGateValue = FastVariant ? FastShootGate : NormalShootGate;
            if (NPC.localAI[1] > shootGateValue - TelegraphDuration)
                color = Color.Lerp(color, Color.LimeGreen * NPC.Opacity, MathHelper.Clamp((NPC.localAI[1] - (shootGateValue - TelegraphDuration)) / TelegraphDuration, 0f, 1f));

            spriteBatch.Draw(texture, vector2, NPC.frame, color, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hit.HitDirection, -1f, 0, default, 1f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && FastVariant && Main.zenithWorld)
            {
                // Spawn even more blobs on death
                for (int i = 1; i < 3; i++)
                {
                    Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveBlob>());
                }
            }
        }
    }
}
