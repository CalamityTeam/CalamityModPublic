using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    public class ReaperShark : ModNPC
    {
        public static readonly SoundStyle SearchRoarSound = new("CalamityMod/Sounds/Custom/ReaperSearchRoar");
        public static readonly SoundStyle EnragedRoarSound = new("CalamityMod/Sounds/Custom/ReaperEnragedRoar");

        public bool hasBeenHit = false;
        public bool reset = false;
        public bool reset2 = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 6f;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.damage = 160;
            NPC.width = 280;
            NPC.height = 150;
            NPC.defense = 70;
            NPC.lifeMax = 100000; // Previously 190,000
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.HitSound = SoundID.NPCHit56;
            NPC.DeathSound = SoundID.NPCDeath60;
            NPC.knockBackResist = 0f;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ReaperSharkBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer4Biome>().Type };
            if (Main.zenithWorld) // legg
            {
                NPC.height = (int)(NPC.height * 1.5f);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ReaperShark")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(reset);
            writer.Write(reset2);
            writer.Write(hasBeenHit);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reset = reader.ReadBoolean();
            reset2 = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool phase1 = NPC.life > NPC.lifeMax * 0.5;
            bool phase2 = NPC.life <= NPC.lifeMax * 0.5;
            bool phase3 = NPC.life <= NPC.lifeMax * 0.1;
            NPC.chaseable = hasBeenHit;
            if (NPC.soundDelay <= 0)
            {
                NPC.soundDelay = 360;
                if (hasBeenHit)
                {
                    SoundEngine.PlaySound(EnragedRoarSound, NPC.Center);
                }
                else
                {
                    SoundEngine.PlaySound(SearchRoarSound, NPC.Center);
                }
            }
            if (phase3 || phase1)
            {
                if (!reset2 && phase3)
                {
                    NPC.damage /= 2;
                    NPC.noTileCollide = true;
                    NPC.netAlways = true;
                    NPC.localAI[0] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = -16f;
                    NPC.ai[3] = 0f;
                    reset2 = true;
                    NPC.netUpdate = true;
                }

                NPC.spriteDirection = (NPC.direction > 0) ? -1 : 1;
                if (NPC.ai[2] == 0f)
                {
                    NPC.TargetClosest(true);
                    if (!Main.player[NPC.target].dead && (Main.player[NPC.target].Center - NPC.Center).Length() < 170f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        NPC.ai[2] = -16f;
                    }
                    if (NPC.justHit || NPC.localAI[0] >= 420f)
                    {
                        NPC.ai[2] = -16f;
                    }
                    return;
                }

                if (NPC.ai[2] < 0f)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = 1f;
                        NPC.velocity.X = NPC.direction * 2;
                    }
                    return;
                }

                if (NPC.ai[2] == 1f)
                {
                    if (NPC.direction == 0)
                    {
                        NPC.TargetClosest(true);
                    }
                    if (NPC.wet || NPC.noTileCollide)
                    {
                        bool flag14 = hasBeenHit;

                        NPC.TargetClosest(false);

                        if ((!Main.player[NPC.target].dead &&
                            Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) &&
                            (Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(360f)) ||
                            NPC.justHit)
                        {
                            hasBeenHit = true;
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
                            if (NPC.ai[3] > 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            {
                                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
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
                            if (NPC.ai[1] >= 90f)
                            {
                                NPC.ai[3] = 1f;
                                NPC.ai[1] = 0f;
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[3] == 0f)
                            {
                                NPC.noTileCollide = false;
                            }
                            else
                            {
                                NPC.noTileCollide = true;
                            }
                            NPC.TargetClosest(true);
                            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.3f;
                            NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.2f;
                            float speedX = phase3 ? 3f : 12f;
                            float speedY = phase3 ? 2.25f : 9f;
                            if (NPC.velocity.X > speedX)
                            {
                                NPC.velocity.X = speedX;
                            }
                            if (NPC.velocity.X < -speedX)
                            {
                                NPC.velocity.X = -speedX;
                            }
                            if (NPC.velocity.Y > speedY)
                            {
                                NPC.velocity.Y = speedY;
                            }
                            if (NPC.velocity.Y < -speedY)
                            {
                                NPC.velocity.Y = -speedY;
                            }
                        }
                        else
                        {
                            if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            {
                                NPC.noTileCollide = false;
                            }
                            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.2f;
                            if (NPC.velocity.X < -4f || NPC.velocity.X > 4f)
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
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.velocity.Y = (float)Main.rand.Next(-250, -180) * 0.1f; //50 20
                                NPC.velocity.X = (float)Main.rand.Next(-50, 50) * 0.1f; //20 20
                                NPC.netUpdate = true;
                            }
                        }
                        NPC.velocity.Y = NPC.velocity.Y + 0.4f;
                        if (NPC.velocity.Y > 16f)
                        {
                            NPC.velocity.Y = 16f;
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
            else if (phase2)
            {
                if (!reset)
                {
                    NPC.noTileCollide = true;
                    NPC.netAlways = true;
                    NPC.localAI[0] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    reset = true;
                    NPC.netUpdate = true;
                }
                bool expertMode = Main.expertMode;
                int num2 = 30;
                float num3 = expertMode ? 0.4f : 0.35f;
                float scaleFactor = expertMode ? 6f : 5.5f;
                int num4 = expertMode ? 28 : 30;
                float num5 = expertMode ? 12f : 11f;
                int num9 = 90;
                int num16 = 75;
                Vector2 vector = NPC.Center;
                Player player = Main.player[NPC.target];
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                {
                    NPC.TargetClosest(true);
                    player = Main.player[NPC.target];
                    NPC.netUpdate = true;
                }
                if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.4f;
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    NPC.ai[0] = 0f;
                    NPC.ai[2] = 0f;
                }
                if (NPC.localAI[0] == 0f)
                {
                    NPC.localAI[0] = 1f;
                    NPC.alpha = 255;
                    NPC.rotation = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[0] = -1f;
                        NPC.netUpdate = true;
                    }
                }
                float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
                if (NPC.spriteDirection == 1)
                {
                    num17 += 3.14159274f;
                }
                if (num17 < 0f)
                {
                    num17 += 6.28318548f;
                }
                if (num17 > 6.28318548f)
                {
                    num17 -= 6.28318548f;
                }
                if (NPC.ai[0] == -1f)
                {
                    num17 = 0f;
                }
                float num18 = 0.04f;
                if (NPC.ai[0] == 1f)
                {
                    num18 = 0f;
                }
                if (NPC.rotation < num17)
                {
                    if ((double)(num17 - NPC.rotation) > 3.1415926535897931)
                    {
                        NPC.rotation -= num18;
                    }
                    else
                    {
                        NPC.rotation += num18;
                    }
                }
                if (NPC.rotation > num17)
                {
                    if ((double)(NPC.rotation - num17) > 3.1415926535897931)
                    {
                        NPC.rotation += num18;
                    }
                    else
                    {
                        NPC.rotation -= num18;
                    }
                }
                if (NPC.rotation > num17 - num18 && NPC.rotation < num17 + num18)
                {
                    NPC.rotation = num17;
                }
                if (NPC.rotation < 0f)
                {
                    NPC.rotation += 6.28318548f;
                }
                if (NPC.rotation > 6.28318548f)
                {
                    NPC.rotation -= 6.28318548f;
                }
                if (NPC.rotation > num17 - num18 && NPC.rotation < num17 + num18)
                {
                    NPC.rotation = num17;
                }
                if (NPC.ai[0] != -1f)
                {
                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.alpha += 15;
                    }
                    else
                    {
                        NPC.alpha -= 15;
                    }
                    if (NPC.alpha < 0)
                    {
                        NPC.alpha = 0;
                    }
                    if (NPC.alpha > 150)
                    {
                        NPC.alpha = 150;
                    }
                }
                if (NPC.ai[0] == -1f)
                {
                    NPC.dontTakeDamage = true;
                    NPC.chaseable = false;
                    NPC.velocity *= 0.98f;
                    int num19 = Math.Sign(player.Center.X - vector.X);
                    if (num19 != 0)
                    {
                        NPC.direction = num19;
                        NPC.spriteDirection = -NPC.direction;
                    }
                    if (NPC.ai[2] > 20f)
                    {
                        NPC.velocity.Y = -2f;
                        NPC.alpha -= 5;
                        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.alpha += 15;
                        }
                        if (NPC.alpha < 0)
                        {
                            NPC.alpha = 0;
                        }
                        if (NPC.alpha > 150)
                        {
                            NPC.alpha = 150;
                        }
                    }
                    if (NPC.ai[2] == (float)(num9 - 30))
                    {
                        int num20 = 36;
                        for (int i = 0; i < num20; i++)
                        {
                            Vector2 expr_80F = (Vector2.Normalize(NPC.velocity) * new Vector2((float)NPC.width / 2f, (float)NPC.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default) + NPC.Center;
                            Vector2 vector2 = expr_80F - NPC.Center;
                            int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
                            Main.dust[num21].noGravity = true;
                            Main.dust[num21].noLight = true;
                            Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
                        }
                        SoundEngine.PlaySound(EnragedRoarSound, NPC.Center);
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (float)num16)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                        return;
                    }
                }
                else if (NPC.ai[0] == 0f && !player.dead)
                {
                    NPC.dontTakeDamage = false;
                    NPC.chaseable = true;
                    if (NPC.ai[1] == 0f)
                    {
                        NPC.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));
                    }
                    Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - vector - NPC.velocity) * scaleFactor;
                    if (NPC.velocity.X < vector3.X)
                    {
                        NPC.velocity.X = NPC.velocity.X + num3;
                        if (NPC.velocity.X < 0f && vector3.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num3;
                        }
                    }
                    else if (NPC.velocity.X > vector3.X)
                    {
                        NPC.velocity.X = NPC.velocity.X - num3;
                        if (NPC.velocity.X > 0f && vector3.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - num3;
                        }
                    }
                    if (NPC.velocity.Y < vector3.Y)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num3;
                        if (NPC.velocity.Y < 0f && vector3.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num3;
                        }
                    }
                    else if (NPC.velocity.Y > vector3.Y)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num3;
                        if (NPC.velocity.Y > 0f && vector3.Y < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num3;
                        }
                    }
                    int num22 = Math.Sign(player.Center.X - vector.X);
                    if (num22 != 0)
                    {
                        if (NPC.ai[2] == 0f && num22 != NPC.direction)
                        {
                            NPC.rotation += 3.14159274f;
                        }
                        NPC.direction = num22;
                        if (NPC.spriteDirection != -NPC.direction)
                        {
                            NPC.rotation += 3.14159274f;
                        }
                        NPC.spriteDirection = -NPC.direction;
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (float)num2)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = Vector2.Normalize(player.Center - vector) * num5;
                        NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X);
                        if (num22 != 0)
                        {
                            NPC.direction = num22;
                            if (NPC.spriteDirection == 1)
                            {
                                NPC.rotation += 3.14159274f;
                            }
                            NPC.spriteDirection = -NPC.direction;
                        }
                        NPC.netUpdate = true;
                        return;
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    int num24 = 7;
                    for (int j = 0; j < num24; j++)
                    {
                        Vector2 arg_E1C_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((float)(NPC.width + 50) / 2f, (float)NPC.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)(float)num24, default) + vector;
                        Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                        int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                        Main.dust[num25].noGravity = true;
                        Main.dust[num25].noLight = true;
                        Main.dust[num25].velocity /= 4f;
                        Main.dust[num25].velocity -= NPC.velocity;
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (float)num4)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                        return;
                    }
                    if (Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] % 5 == 0)
                    {
                        Vector2 direction = vector - player.Center;
                        direction.Normalize();
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * 10f, ProjectileID.DemonSickle, NPC.damage / 2, 0f, Main.myPlayer);
                    }
                }
            }
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 6400f)
            {
                NPC.active = false;
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
            int newFrameHeight = (int)(frameHeight * (Main.zenithWorld ? 1.5f : 1f));
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.3f,
                PortraitPositionXOverride = 54f,
                PortraitPositionYOverride = -10f,
                SpriteDirection = 1
            };
            value.Position.X += 12f;
            value.Position.Y -= 50f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPC.frameCounter += hasBeenHit || NPC.IsABestiaryIconDummy ? 0.15f : 0.075f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * newFrameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Asset<Texture2D> npcTexture = Main.zenithWorld ? ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/ReaperSharkMan") : TextureAssets.Npc[NPC.type];
            Rectangle nframe = npcTexture.Frame(1, 4, 0, (int)NPC.frameCounter);
            Vector2 origin = new Vector2((float)(npcTexture.Value.Width / 2), (float)(npcTexture.Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)npcTexture.Value.Width, (float)(npcTexture.Value.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            npcOffset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(npcTexture.Value, npcOffset, nframe, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer4 && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<ReaperShark>()))
                return Main.remixWorld ? 10.8f : SpawnCondition.CaveJellyfish.Chance * 1.2f;

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<Voidstone>(), 1, 40, 50);
            npcLoot.Add(ModContent.ItemType<AnechoicCoating>(), 1, 2, 3);

            var postPolter = npcLoot.DefineConditionalDropSet(DropHelper.PostPolter());
            postPolter.Add(ModContent.ItemType<ReaperTooth>(), 1, 3, 4);
            postPolter.Add(ModContent.ItemType<DeepSeaDumbbell>(), 3);
            postPolter.Add(ModContent.ItemType<Valediction>(), 3);

            var postClone = npcLoot.DefineConditionalDropSet(DropHelper.PostCal());
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 10, 17, 14, 22));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
