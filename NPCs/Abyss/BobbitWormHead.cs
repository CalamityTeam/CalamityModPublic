using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class BobbitWormHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bobbit Worm");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            NPC.damage = 150;
            NPC.width = 80;
            NPC.height = 40;
            NPC.defense = 50;
            NPC.DR_NERD(0.25f);
            NPC.lifeMax = 6000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BobbitWormBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            if (!Main.npc[(int)NPC.ai[2]].active || (int)NPC.ai[2] < 0)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.noTileCollide = true;
                float num659 = 14f;
                Vector2 vector79 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num660 = Main.npc[(int)NPC.ai[2]].Center.X - vector79.X;
                float num661 = Main.npc[(int)NPC.ai[2]].Center.Y - vector79.Y;
                float num662 = (float)Math.Sqrt((double)(num660 * num660 + num661 * num661));
                if (num662 < 11f + num659)
                {
                    NPC.rotation = 0f;
                    NPC.velocity.X = num660;
                    NPC.velocity.Y = num661;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.TargetClosest(true);
                        //if (npc.Center.Y + ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                        //    Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 50f : 100f) > Main.player[npc.target].Center.Y &&
                        if (NPC.Center.Y + Main.player[NPC.target].Calamity().GetAbyssAggro(100f, 50f) > Main.player[NPC.target].Center.Y &&
                            Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 1f;
                            return;
                        }
                        NPC.ai[1] = 0f;
                        return;
                    }
                }
                else
                {
                    num662 = num659 / num662;
                    NPC.velocity.X = num660 * num662;
                    NPC.velocity.Y = num661 * num662;
                    NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) - 1.57f;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.noTileCollide = true;
                NPC.collideX = false;
                NPC.collideY = false;
                float num663 = 16f;
                Vector2 vector80 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num664 = Main.player[NPC.target].Center.X - vector80.X;
                float num665 = Main.player[NPC.target].Center.Y - vector80.Y;
                float num666 = (float)Math.Sqrt((double)(num664 * num664 + num665 * num665));
                num666 = num663 / num666;
                NPC.velocity.X = num664 * num666;
                NPC.velocity.Y = num665 * num666;
                NPC.ai[0] = 2f;
                NPC.rotation = (float)Math.Atan2((double)-(double)NPC.velocity.Y, (double)-(double)NPC.velocity.X) - 1.57f;
            }
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                {
                    if (NPC.velocity.X > 0f && NPC.Center.X > Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.X < 0f && NPC.Center.X < Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                else
                {
                    if (NPC.velocity.Y > 0f && NPC.Center.Y > Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.Y < 0f && NPC.Center.Y < Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                Vector2 vector81 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num667 = Main.npc[(int)NPC.ai[2]].Center.X - vector81.X;
                float num668 = Main.npc[(int)NPC.ai[2]].Center.Y - vector81.Y;
                float num669 = (float)Math.Sqrt((double)(num667 * num667 + num668 * num668));
                if (num669 > 700f || NPC.collideX || NPC.collideY)
                {
                    NPC.noTileCollide = true;
                    NPC.ai[0] = 0f;
                    return;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                float num671 = 16f;
                float num672 = 0.25f;
                Vector2 vector82 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num673 = Main.player[NPC.target].Center.X - vector82.X;
                float num674 = Main.player[NPC.target].Center.Y - vector82.Y;
                float num675 = (float)Math.Sqrt((double)(num673 * num673 + num674 * num674));
                num675 = num671 / num675;
                num673 *= num675;
                num674 *= num675;
                if (NPC.velocity.X < num673)
                {
                    NPC.velocity.X = NPC.velocity.X + num672;
                    if (NPC.velocity.X < 0f && num673 > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + num672 * 2f;
                    }
                }
                else if (NPC.velocity.X > num673)
                {
                    NPC.velocity.X = NPC.velocity.X - num672;
                    if (NPC.velocity.X > 0f && num673 < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - num672 * 2f;
                    }
                }
                if (NPC.velocity.Y < num674)
                {
                    NPC.velocity.Y = NPC.velocity.Y + num672;
                    if (NPC.velocity.Y < 0f && num674 > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num672 * 2f;
                    }
                }
                else if (NPC.velocity.Y > num674)
                {
                    NPC.velocity.Y = NPC.velocity.Y - num672;
                    if (NPC.velocity.Y > 0f && num674 < 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num672 * 2f;
                    }
                }
                NPC.rotation = (float)Math.Atan2((double)-(double)NPC.velocity.Y, (double)-(double)NPC.velocity.X);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            float drawPositionX = Main.npc[(int)NPC.ai[2]].Center.X - center.X;
            float drawPositionY = Main.npc[(int)NPC.ai[2]].Center.Y - center.Y;
            float rotation = (float)Math.Atan2((double)drawPositionY, (double)drawPositionX) - 1.57f;
            bool draw = true;
            while (draw)
            {
                float totalDrawDistance = (float)Math.Sqrt((double)(drawPositionX * drawPositionX + drawPositionY * drawPositionY));
                if (totalDrawDistance < 16f)
                {
                    draw = false;
                }
                else
                {
                    totalDrawDistance = 16f / totalDrawDistance;
                    drawPositionX *= totalDrawDistance;
                    drawPositionY *= totalDrawDistance;
                    center.X += drawPositionX;
                    center.Y += drawPositionY;
                    drawPositionX = Main.npc[(int)NPC.ai[2]].Center.X - center.X;
                    drawPositionY = Main.npc[(int)NPC.ai[2]].Center.Y - center.Y;
                    drawPositionY += 4f;
                    Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Height)), color, rotation,
                        new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Width * 0.5f, (float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override void NPCLoot()
        {
            int minCells = Main.expertMode ? 7 : 5;
            int maxCells = Main.expertMode ? 10 : 7;
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<DepthCells>(), DownedBossSystem.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<BobbitHook>(), DownedBossSystem.downedPolterghast, 3, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/BobbitWorm"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/BobbitWorm2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/BobbitWorm3"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/BobbitWorm4"), 1f);
            }
        }
    }
}
