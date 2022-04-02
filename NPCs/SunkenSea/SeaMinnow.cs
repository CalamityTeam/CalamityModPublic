using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Critters;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class SeaMinnow : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Minnow");
            Main.npcFrameCount[npc.type] = 4;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.1f;
            npc.noGravity = true;
            npc.damage = 0;
            npc.width = 36;
            npc.height = 22;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.aiStyle = -1;
            aiType = -1;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SeaMinnowBanner>();
            npc.chaseable = false;
            npc.catchItem = (short)ModContent.ItemType<SeaMinnowItem>();
        }

        public override void AI()
        {
            CalamityAI.PassiveSwimmingAI(npc, mod, 3, 150f, 0.25f, 0.15f, 6f, 6f, 0.05f);
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.noGravity = true;
            bool flag14 = false;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.wet)
            {
                npc.TargetClosest(false);
                if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
                    (Main.player[npc.target].Center - npc.Center).Length() < 150f)
                {
                    flag14 = true;
                }
                if ((!Main.player[npc.target].wet || Main.player[npc.target].dead) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
                    if (npc.collideX || npc.velocity.X == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * -3f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -3f;
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
                    npc.velocity.X = npc.velocity.X - (float)npc.direction * 0.25f;
                    npc.velocity.Y = npc.velocity.Y - (float)npc.directionY * 0.15f;
                    if (npc.velocity.X > 6f)
                    {
                        npc.velocity.X = 6f;
                    }
                    if (npc.velocity.X < -6f)
                    {
                        npc.velocity.X = -6f;
                    }
                    if (npc.velocity.Y > 6f)
                    {
                        npc.velocity.Y = 6f;
                    }
                    if (npc.velocity.Y < -6f)
                    {
                        npc.velocity.Y = -6f;
                    }
                    npc.direction *= -1;
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
            npc.rotation = npc.velocity.X * 0.05f;
            if ((double)npc.rotation < -0.1)
            {
                npc.rotation = -0.1f;
            }
            if ((double)npc.rotation > 0.1)
            {
                npc.rotation = 0.1f;
                return;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}
