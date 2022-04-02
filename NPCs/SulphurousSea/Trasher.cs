using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class Trasher : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trasher");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.noGravity = true;
            npc.damage = 50;
            npc.width = 150;
            npc.height = 40;
            npc.defense = 22;
            npc.lifeMax = 200;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 3, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath5;
            npc.knockBackResist = 0f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<TrasherBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? -1 : 1;
            npc.chaseable = hasBeenHit;
            if (npc.justHit)
            {
                hasBeenHit = true;
            }
            if (npc.wet)
            {
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                }
                npc.noTileCollide = false;
                bool flag14 = hasBeenHit;
                npc.TargetClosest(false);
                if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                    (Main.player[npc.target].Center - npc.Center).Length() < 200f)
                {
                    flag14 = true;
                }
                if (Main.player[npc.target].dead && flag14)
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
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * (CalamityWorld.death ? 0.6f : 0.3f);
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * (CalamityWorld.death ? 0.2f : 0.1f);
                    float velocityX = CalamityWorld.death ? 20f : 10f;
                    float velocityY = CalamityWorld.death ? 10f : 5f;
                    if (npc.velocity.X > velocityX)
                    {
                        npc.velocity.X = velocityX;
                    }
                    if (npc.velocity.X < -velocityX)
                    {
                        npc.velocity.X = -velocityX;
                    }
                    if (npc.velocity.Y > velocityY)
                    {
                        npc.velocity.Y = velocityY;
                    }
                    if (npc.velocity.Y < -velocityY)
                    {
                        npc.velocity.Y = -velocityY;
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -1.5f || npc.velocity.X > 1.5f)
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
                if (Collision.WetCollision(npc.position, npc.width, npc.height))
                {
                    npc.noTileCollide = false;
                    npc.netUpdate = true;
                    return;
                }
                npc.noTileCollide = true;
                float num823 = 1f;
                npc.TargetClosest(true);
                bool flag51 = false;
                if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.death)
                {
                    num823 = 1.5f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25 || CalamityWorld.death)
                {
                    num823 = 2.5f;
                }
                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 20f)
                {
                    flag51 = true;
                }
                if (flag51)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                else
                {
                    if (npc.direction > 0)
                    {
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    }
                    if (npc.direction < 0)
                    {
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                    }
                }
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);
                bool flag52 = false;
                if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height - 16f)
                {
                    flag52 = true;
                }
                if (flag52)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.5f;
                }
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if (npc.velocity.Y > -0.2f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.2f;
                    }
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if (npc.velocity.Y < 0.1f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.5f;
                    }
                }
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * 0.05f;
            npc.rotation = MathHelper.Clamp(npc.rotation, -0.1f, 0.1f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 1.25 : 1.0;
            if (npc.frameCounter < 6.0)
            {
                npc.frame.Y = 0;
            }
            else if (npc.frameCounter < 12.0)
            {
                npc.frame.Y = frameHeight;
            }
            else if (npc.frameCounter < 18.0)
            {
                npc.frame.Y = frameHeight * 2;
            }
            else if (npc.frameCounter < 24.0)
            {
                npc.frame.Y = frameHeight * 3;
            }
            else
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = 0;
            }
            if (!npc.wet)
            {
                npc.frame.Y = npc.frame.Y + frameHeight * 4;
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur || (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water))
            {
                return 0.1f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            if (!NPC.savedAngler && !NPC.AnyNPCs(NPCID.Angler) && !NPC.AnyNPCs(NPCID.SleepingAngler) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Angler, 0, 0f, 0f, 0f, 0f, 255);
                NPC.savedAngler = true;
            }
            DropHelper.DropItemChance(npc, ItemID.DivingHelmet, 20);
            DropHelper.DropItemChance(npc, ModContent.ItemType<TrashmanTrashcan>(), 20);
            DropHelper.DropItemCondition(npc, ItemID.Gatligator, Main.hardMode, 10, 1, 1);
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Trasher"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Trasher2"), 1f);
            }
        }
    }
}
