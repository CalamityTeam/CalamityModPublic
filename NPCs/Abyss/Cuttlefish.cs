using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class Cuttlefish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cuttlefish");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.damage = 34;
            NPC.width = 50;
            NPC.height = 28;
            NPC.defense = 8;
            NPC.lifeMax = 110;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit33;
            NPC.DeathSound = SoundID.NPCDeath28;
            NPC.knockBackResist = 0.3f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CuttlefishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            int num = 200;
            if (NPC.ai[2] == 0f)
            {
                NPC.alpha = num;
                NPC.TargetClosest(true);
                if (!Main.player[NPC.target].dead && (Main.player[NPC.target].Center - NPC.Center).Length() < 170f &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.ai[2] = -16f;
                }
                if (NPC.justHit)
                {
                    NPC.ai[2] = -16f;
                }
                if (NPC.collideX)
                {
                    NPC.velocity.X = NPC.velocity.X * -1f;
                    NPC.direction *= -1;
                }
                if (NPC.collideY)
                {
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
                NPC.velocity.X = NPC.velocity.X + NPC.direction * 0.02f;
                NPC.rotation = NPC.velocity.X * 0.4f;
                if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.95f;
                }
                if (NPC.ai[0] == -1f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    if (NPC.velocity.Y < -1f)
                    {
                        NPC.ai[0] = 1f;
                    }
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    if (NPC.velocity.Y > 1f)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                int num268 = (int)(NPC.position.X + (NPC.width / 2)) / 16;
                int num269 = (int)(NPC.position.Y + (NPC.height / 2)) / 16;
                if (Main.tile[num268, num269 - 1] == null)
                {
                    Main.tile[num268, num269 - 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 1] == null)
                {
                    Main.tile[num268, num269 + 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 2] == null)
                {
                    Main.tile[num268, num269 + 2] = new Tile();
                }
                if (Main.tile[num268, num269 - 1].LiquidAmount > 128)
                {
                    if (Main.tile[num268, num269 + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[num268, num269 + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                }
                if (NPC.velocity.Y > 1.2 || NPC.velocity.Y < -1.2)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.99f;
                }
                return;
            }
            if (NPC.ai[2] < 0f)
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= num / 16;
                    if (NPC.alpha < 0)
                    {
                        NPC.alpha = 0;
                    }
                }
                NPC.ai[2] += 1f;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[2] = 1f;
                    NPC.velocity.X = NPC.direction * 2;
                }
                return;
            }
            if (NPC.ai[2] == 1f)
            {
                NPC.chaseable = true;
                if (NPC.direction == 0)
                {
                    NPC.TargetClosest(true);
                }
                if (NPC.wet || NPC.noTileCollide)
                {
                    bool flag14 = false;
                    NPC.TargetClosest(false);
                    if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead)
                    {
                        flag14 = true;
                    }
                    if (!flag14)
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.noTileCollide = false;
                        }
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
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            if (NPC.ai[3] > 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            {
                                NPC.ai[3] = 0f;
                                NPC.ai[1] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (NPC.ai[3] == 0f)
                        {
                            NPC.ai[1] += 1f;
                        }
                        if (NPC.ai[1] >= 150f)
                        {
                            NPC.ai[3] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.netUpdate = true;
                        }
                        if (NPC.ai[3] == 0f)
                        {
                            NPC.alpha = 0;
                            NPC.noTileCollide = false;
                        }
                        else
                        {
                            NPC.alpha = 200;
                            NPC.noTileCollide = true;
                        }
                        NPC.TargetClosest(true);
                        NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.2f;
                        NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.2f;
                        if (NPC.velocity.X > 9f)
                        {
                            NPC.velocity.X = 9f;
                        }
                        if (NPC.velocity.X < -9f)
                        {
                            NPC.velocity.X = -9f;
                        }
                        if (NPC.velocity.Y > 7f)
                        {
                            NPC.velocity.Y = 7f;
                        }
                        if (NPC.velocity.Y < -7f)
                        {
                            NPC.velocity.Y = -7f;
                        }
                    }
                    else
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.noTileCollide = false;
                        }
                        NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.1f;
                        if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
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
                    NPC.velocity.Y = NPC.velocity.Y + 0.25f;
                    if (NPC.velocity.Y > 7f)
                    {
                        NPC.velocity.Y = 7f;
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
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet && !NPC.noTileCollide)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/CuttlefishGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/CuttlefishGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/CuttlefishGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Darkness, 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer2 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<AnechoicCoating>(), 2);
            int inkBombDropRate = Main.expertMode ? 50 : 100;
            DropHelper.DropItemChance(NPC, ModContent.ItemType<InkBomb>(), inkBombDropRate, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
