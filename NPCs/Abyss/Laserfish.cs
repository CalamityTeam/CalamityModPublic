using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
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
    public class Laserfish : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laserfish");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 20;
            npc.width = 60;
            npc.height = 26;
            npc.defense = 25;
            npc.lifeMax = 240;
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<CrushDepth>()] = true;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath26;
            npc.knockBackResist = 0.65f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<LaserfishBanner>();
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
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.justHit)
            {
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (npc.wet)
            {
                bool flag14 = hasBeenHit;
                npc.TargetClosest(false);
                if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                    //(Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                    //Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 200f : 400f) *
                    //(Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f))
                    (Main.player[npc.target].Center - npc.Center).Length() < Main.player[npc.target].Calamity().GetAbyssAggro(400f, 200f))
                {
                    flag14 = true;
                }
                if ((!Main.player[npc.target].wet || Main.player[npc.target].dead) && flag14)
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
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * (CalamityWorld.death ? 0.3f : 0.15f);
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * (CalamityWorld.death ? 0.3f : 0.15f);
					float velocity = CalamityWorld.death ? 8f : 4f;
					if (npc.velocity.X > velocity)
                    {
                        npc.velocity.X = velocity;
                    }
                    if (npc.velocity.X < -velocity)
                    {
                        npc.velocity.X = -velocity;
                    }
                    if (npc.velocity.Y > velocity)
                    {
                        npc.velocity.Y = velocity;
                    }
                    if (npc.velocity.Y < -velocity)
                    {
                        npc.velocity.Y = -velocity;
                    }
                    npc.localAI[0] += (CalamityWorld.death ? 2f : 1f);
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            float speed = 5f;
                            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                            float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                            float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                            float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                            num8 = speed / num8;
                            num6 *= num8;
                            num7 *= num8;
                            int damage = 40;
                            if (Main.expertMode)
                            {
                                damage = 30;
                            }
                            int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 25f : -25f), npc.Center.Y + (Main.player[npc.target].position.Y > npc.Center.Y ? 5f : -5f), num6, num7, 259, damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[beam].tileCollide = true;
                        }
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -1f || npc.velocity.X > 1f)
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
            if ((double)npc.rotation < -0.1)
            {
                npc.rotation = -0.1f;
            }
            if ((double)npc.rotation > 0.1)
            {
                npc.rotation = 0.1f;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
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
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/LaserfishGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/LaserfishGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Yellow);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/LaserfishGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer2 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer3 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(1000000) && CalamityWorld.revenge)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HalibutCannon>());
            }
            if (CalamityWorld.downedCalamitas)
            {
                if (Main.rand.NextBool(2))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Lumenite>());
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Laserfish"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Laserfish2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Laserfish3"), 1f);
            }
        }
    }
}
