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
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Abyss
{
    public class DevilFishAlt : ModNPC
    {
        public bool brokenMask = false;
        public int hitCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil Fish");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.Calamity().canBreakPlayerDefense = true;
            npc.damage = 90;
            npc.width = 126;
            npc.height = 66;
            npc.defense = 999999;
            npc.lifeMax = 400;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.85f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<DevilFishBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(brokenMask);
            writer.Write(hitCounter);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            brokenMask = reader.ReadBoolean();
            hitCounter = reader.ReadInt32();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            float speedBoost = brokenMask ? 1.5f : 1;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            npc.chaseable = brokenMask;

            if (hitCounter >= 5 && !brokenMask)
            {
                brokenMask = true;
                npc.HitSound = SoundID.NPCHit1;
                npc.defense = 15;
                for (int i = 1; i < 4; i++)
                {
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DevilFishMask" + i + (i == 3 ? "Alt" : "")), 1f);
                }
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/DevilMaskBreak"), (int)npc.position.X, (int)npc.position.Y);
            }

            if (npc.wet)
            {
                bool flag14 = brokenMask;
                npc.TargetClosest(false);
                if (player.statLife <= player.statLifeMax2 / 4)
                {
                    flag14 = true;
                }
                if ((!player.wet || player.dead || !Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height)) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                if (flag14)
                {
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * (CalamityWorld.death ? 0.5f : 0.25f) * speedBoost;
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * (CalamityWorld.death ? 0.3f : 0.15f) * speedBoost;
                    float velocity = CalamityWorld.death ? 12f : 6f;
                    if (npc.velocity.X > velocity * speedBoost)
                    {
                        npc.velocity.X = velocity * speedBoost;
                    }
                    if (npc.velocity.X < -velocity * speedBoost)
                    {
                        npc.velocity.X = -velocity * speedBoost;
                    }
                    if (npc.velocity.Y > velocity * speedBoost)
                    {
                        npc.velocity.Y = velocity * speedBoost;
                    }
                    if (npc.velocity.Y < -velocity * speedBoost)
                    {
                        npc.velocity.Y = -velocity * speedBoost;
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -2.5f || npc.velocity.X > 2.5f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.95f;
                    }
                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                        if ((double)npc.velocity.Y < -0.3)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                        if ((double)npc.velocity.Y > 0.3)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1] == null)
                {
                    Main.tile[num258, num259 - 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 1] == null)
                {
                    Main.tile[num258, num259 + 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 2] == null)
                {
                    Main.tile[num258, num259 + 2] = new Tile();
                }
                if (Main.tile[num258, num259 - 1].liquid > 128)
                {
                    if (Main.tile[num258, num259 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.94f;
                    if ((double)npc.velocity.X > -0.2 && (double)npc.velocity.X < 0.2)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.3f;
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
            if ((double)npc.rotation < -0.2)
            {
                npc.rotation = -0.2f;
            }
            if ((double)npc.rotation > 0.2)
            {
                npc.rotation = 0.2f;
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
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y += frameHeight;
            }
            if (!brokenMask)
            {
                if (npc.frame.Y > frameHeight * 7)
                {
                    npc.frame.Y = 0;
                }
            }
            else
            {
                if (npc.frame.Y < frameHeight * 8 || npc.frame.Y > frameHeight * 15)
                {
                    npc.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/DevilFishGlowAlt").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/DevilFishGlowAlt").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Red);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/DevilFishGlowAlt"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 180, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer1 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.225f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer2 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 0.5f);
            int minCells = Main.expertMode ? 2 : 1;
            int maxCells = Main.expertMode ? 3 : 2;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<DepthCells>(), CalamityWorld.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ChaoticOre>(), NPC.downedGolemBoss, 1f, 3, 9);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }

            if (hitCounter <= 5)
            {
                ++hitCounter;
            }
        }
    }
}
