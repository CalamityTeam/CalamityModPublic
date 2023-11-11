using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveBlob2 : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.HiveBlob.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.1f;
            NPC.aiStyle = -1;
            NPC.damage = 10;
            NPC.width = 25;
            NPC.height = 25;

            NPC.lifeMax = 75;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 1300;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

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

            if (NPC.ai[3] > 0f)
                hiveMind = (int)NPC.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= getFuckedAI ? 10f : 1f; //Relocation rate
                float RandomPositionMultiplier = getFuckedAI ? 2f : 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(180, 361);
                    NPC.ai[0] = Main.rand.Next(-100, 101) * RandomPositionMultiplier; //X position
                    NPC.ai[1] = Main.rand.Next(-100, 101) * RandomPositionMultiplier; //Y position
                    NPC.netUpdate = true;
                }
            }

            NPC.TargetClosest(true);

            float relocateSpeed = getFuckedAI ? 1.2f : death ? 0.8f : revenge ? 0.7f : expertMode ? 0.6f : 0.5f;
            Vector2 randomRelocateVector = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - (NPC.width / 2) - randomRelocateVector.X;
            float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - (NPC.height / 2) - randomRelocateVector.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
            float hiveMindX = Main.npc[hiveMind].position.X + (Main.npc[hiveMind].width / 2);
            float hiveMindY = Main.npc[hiveMind].position.Y + (Main.npc[hiveMind].height / 2);
            Vector2 hiveMindPos = new Vector2(hiveMindX, hiveMindY);
            float randomPosX = hiveMindX + NPC.ai[0];
            float randomPosY = hiveMindY + NPC.ai[1];
            float finalRandPosX = randomPosX - hiveMindPos.X;
            float finalRandPosY = randomPosY - hiveMindPos.Y;
            float finalRandDistance = (float)Math.Sqrt(finalRandPosX * finalRandPosX + finalRandPosY * finalRandPosY);
            finalRandDistance = (Main.getGoodWorld ? 192f : 96f) / finalRandDistance;
            finalRandPosX *= finalRandDistance;
            finalRandPosY *= finalRandDistance;
            if (NPC.position.X < hiveMindX + finalRandPosX)
            {
                NPC.velocity.X = NPC.velocity.X + relocateSpeed;
                if (NPC.velocity.X < 0f && finalRandPosX > 0f)
                    NPC.velocity.X = NPC.velocity.X * 0.8f;
            }
            else if (NPC.position.X > hiveMindX + finalRandPosX)
            {
                NPC.velocity.X = NPC.velocity.X - relocateSpeed;
                if (NPC.velocity.X > 0f && finalRandPosX < 0f)
                    NPC.velocity.X = NPC.velocity.X * 0.8f;
            }
            if (NPC.position.Y < hiveMindY + finalRandPosY)
            {
                NPC.velocity.Y = NPC.velocity.Y + relocateSpeed;
                if (NPC.velocity.Y < 0f && finalRandPosY > 0f)
                    NPC.velocity.Y = NPC.velocity.Y * 0.8f;
            }
            else if (NPC.position.Y > hiveMindY + finalRandPosY)
            {
                NPC.velocity.Y = NPC.velocity.Y - relocateSpeed;
                if (NPC.velocity.Y > 0f && finalRandPosY < 0f)
                    NPC.velocity.Y = NPC.velocity.Y * 0.8f;
            }

            float velocityLimit = getFuckedAI ? 32f : 8f;
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
                if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    NPC.localAI[1] = 180f;

                NPC.localAI[1] += Main.rand.Next(3) + 1f;
                if (NPC.localAI[1] >= 360f && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 80f)
                {
                    NPC.localAI[1] = 0f;
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float projSpeed = death ? 5f : revenge ? 4.5f : expertMode ? 4f : 3.5f;
                        if (Main.getGoodWorld)
                            projSpeed *= 2.4f;

                        Vector2 projDirection = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + (NPC.height / 2));
                        float playerX = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - projDirection.X;
                        float playerY = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - projDirection.Y;
                        float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                        playerDist = projSpeed / playerDist;
                        playerX *= playerDist;
                        playerY *= playerDist;
                        int type = (CalamityWorld.LegendaryMode && CalamityWorld.revenge && Main.rand.NextBool(5)) ? ProjectileID.CursedFlameHostile : ModContent.ProjectileType<VileClot>();
                        int damage = type == ProjectileID.CursedFlameHostile ? 30 : NPC.GetProjectileDamage(type);
                        Vector2 projectileVelocity = new Vector2(playerX, playerY);
                        if (type == ProjectileID.CursedFlameHostile)
                        {
                            Vector2 v = Main.player[NPC.target].Center - NPC.Center - Main.player[NPC.target].velocity * 20f;
                            projectileVelocity = v.SafeNormalize(Vector2.UnitY) * projSpeed;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projDirection, projectileVelocity, type, damage, 0f, Main.myPlayer);
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);

            Vector2 vector2 = NPC.Center - screenPos;
            vector2 -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            vector2 += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (NPC.localAI[1] > 240f)
                color = Color.Lerp(color, Color.Green, MathHelper.Clamp((NPC.localAI[1] - 240f) / 120f, 0f, 1f));

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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hit.HitDirection, -1f, 0, default, 1f);

                if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
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
}
