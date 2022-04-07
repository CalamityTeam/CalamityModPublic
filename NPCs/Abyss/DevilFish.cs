using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
namespace CalamityMod.NPCs.Abyss
{
    public class DevilFish : ModNPC
    {
        public bool brokenMask = false;
        public int hitCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil Fish");
            Main.npcFrameCount[NPC.type] = 16;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90;
            NPC.width = 126;
            NPC.height = 66;
            NPC.defense = 999999;
            NPC.lifeMax = 400;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.85f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<DevilFishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(brokenMask);
            writer.Write(hitCounter);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            brokenMask = reader.ReadBoolean();
            hitCounter = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            float speedBoost = brokenMask ? 1.5f : 1;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            NPC.noGravity = true;
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            NPC.chaseable = brokenMask;

            if (hitCounter >= 5 && !brokenMask)
            {
                brokenMask = true;
                NPC.HitSound = SoundID.NPCHit1;
                NPC.defense = 15;
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DevilFishMask" + i).Type, 1f);
                    }
                }
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/DevilMaskBreak"), (int)NPC.position.X, (int)NPC.position.Y);
            }

            if (NPC.wet)
            {
                bool flag14 = brokenMask;
                NPC.TargetClosest(false);
                if (player.statLife <= player.statLifeMax2 / 4)
                {
                    flag14 = true;
                }
                if ((!player.wet || player.dead || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height)) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.direction *= -1;
                        NPC.netUpdate = true;
                    }
                    if (NPC.collideY)
                    {
                        NPC.netUpdate = true;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                            NPC.directionY = -1;
                            NPC.ai[0] = -1f;
                        }
                        else if (NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                            NPC.directionY = 1;
                            NPC.ai[0] = 1f;
                        }
                    }
                }
                if (flag14)
                {
                    NPC.TargetClosest(true);
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * (CalamityWorld.death ? 0.5f : 0.25f) * speedBoost;
                    NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * (CalamityWorld.death ? 0.3f : 0.15f) * speedBoost;
                    float velocity = CalamityWorld.death ? 12f : 6f;
                    if (NPC.velocity.X > velocity * speedBoost)
                    {
                        NPC.velocity.X = velocity * speedBoost;
                    }
                    if (NPC.velocity.X < -velocity * speedBoost)
                    {
                        NPC.velocity.X = -velocity * speedBoost;
                    }
                    if (NPC.velocity.Y > velocity * speedBoost)
                    {
                        NPC.velocity.Y = velocity * speedBoost;
                    }
                    if (NPC.velocity.Y < -velocity * speedBoost)
                    {
                        NPC.velocity.Y = -velocity * speedBoost;
                    }
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.1f;
                    if (NPC.velocity.X < -2.5f || NPC.velocity.X > 2.5f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.95f;
                    }
                    if (NPC.ai[0] == -1f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                        if ((double)NPC.velocity.Y < -0.3)
                        {
                            NPC.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                        if ((double)NPC.velocity.Y > 0.3)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                int num259 = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1].LiquidAmount > 128)
                {
                    if (Main.tile[num258, num259 + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                if ((double)NPC.velocity.Y > 0.4 || (double)NPC.velocity.Y < -0.4)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.94f;
                    if ((double)NPC.velocity.X > -0.2 && (double)NPC.velocity.X < 0.2)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                NPC.ai[0] = 1f;
            }
            NPC.rotation = NPC.velocity.Y * (float)NPC.direction * 0.1f;
            if ((double)NPC.rotation < -0.2)
            {
                NPC.rotation = -0.2f;
            }
            if ((double)NPC.rotation > 0.2)
            {
                NPC.rotation = 0.2f;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return brokenMask;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
            }
            if (!brokenMask)
            {
                if (NPC.frame.Y > frameHeight * 7)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 8 || NPC.frame.Y > frameHeight * 15)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/DevilFishGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/DevilFishGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Red);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/DevilFishGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 180, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.225f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer2 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            return 0f;
        }

        public static void ChooseDevilfishLoot(NPCLoot npcLoot)
        {
            int minCells = Main.expertMode ? 2 : 1;
            int maxCells = Main.expertMode ? 3 : 2;
            npcLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<Lumenite>(), 2);
            npcLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<DepthCells>(), 2, minCells, maxCells);
            npcLoot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<ChaoticOre>(), 1, 3, 9);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => ChooseDevilfishLoot(npcLoot);

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }

            if (hitCounter <= 5)
            {
                ++hitCounter;
            }
        }
    }
}
