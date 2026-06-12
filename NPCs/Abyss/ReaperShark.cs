using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
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
    [HeavyKnockbackWhitelisted]
    public class ReaperShark : ModNPC
    {
        public static Asset<Texture2D> ManTexture;

        public static readonly SoundStyle SearchRoarSound = new("CalamityMod/Sounds/Custom/ReaperSearchRoar");
        public static readonly SoundStyle EnragedRoarSound = new("CalamityMod/Sounds/Custom/ReaperEnragedRoar");

        public bool hasBeenHit = false;
        public bool reset = false;
        public bool reset2 = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            if (!Main.dedServ)
            {
                ManTexture = ModContent.Request<Texture2D>(Texture + "Man", AssetRequestMode.ImmediateLoad);
            }
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
            NPC.value = Item.buyPrice(gold: 25);
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
                NPC.height = (int)(NPC.height * 1.5f);
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
            bool phase1 = NPC.localAI[2] != 5 && NPC.life > NPC.lifeMax * 0.5;
            bool phase2 = NPC.localAI[2] == 5 || NPC.life <= NPC.lifeMax * 0.5;
            bool phase3 = NPC.life <= NPC.lifeMax * 0.1;
            NPC.chaseable = hasBeenHit;
            if (hasBeenHit == false)
                NPC.TargetClosest(false);
            // If fished out, get mad
            if (Main.player[NPC.target].ownedProjectileCounts[ModContent.ProjectileType<TrustyOldBobber>()] > 0) { NPC.localAI[2] = 5; hasBeenHit = true; }
            if (NPC.soundDelay <= 0)
            {
                NPC.soundDelay = 360;
                if (hasBeenHit)
                {
                    // Roar becomes lower pitch when crippled
                    float pitch = phase3 ? -0.4f : 0;
                    SoundEngine.PlaySound(EnragedRoarSound with { Pitch = pitch }, NPC.Center);
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
                    NPC.damage = 0;
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
                    // Avoid cheap bullshit
                    NPC.damage = 0;

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
                    // Avoid cheap bullshit
                    NPC.damage = 0;

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
                        bool canAttack = hasBeenHit;

                        NPC.TargetClosest(false);

                        if ((!Main.player[NPC.target].dead &&
                            Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) &&
                            (Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(360f)) ||
                            NPC.justHit)
                        {
                            hasBeenHit = true;
                        }

                        if (!canAttack)
                        {
                            // Avoid cheap bullshit
                            NPC.damage = 0;

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
                        if (canAttack)
                        {
                            // Set damage
                            NPC.damage = phase3 ? (int)Math.Round(NPC.defDamage * 0.5) : NPC.defDamage;

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
                        int npcTileX = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                        int npcTileY = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                        if (Main.tile[npcTileX, npcTileY - 1].LiquidAmount > 128)
                        {
                            if (Main.tile[npcTileX, npcTileY + 1].HasTile)
                            {
                                NPC.ai[0] = -1f;
                            }
                            else if (Main.tile[npcTileX, npcTileY + 2].HasTile)
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
                        // Set damage
                        NPC.damage = NPC.defDamage;

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
                float chargeAcceleration = expertMode ? 0.4f : 0.35f;
                float chargeThreshold = expertMode ? 6f : 5.5f;
                int phase2Delay = expertMode ? 28 : 30;
                float chargeVelocity = expertMode ? 12f : 11f;
                Vector2 shorkCenter = NPC.Center;
                Player player = Main.player[NPC.target];
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                {
                    NPC.TargetClosest(true);
                    player = Main.player[NPC.target];
                    NPC.netUpdate = true;
                }
                if (player.dead || Vector2.Distance(player.Center, shorkCenter) > 5600f)
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
                float getRotatedIdiot = (float)Math.Atan2((double)(player.Center.Y - shorkCenter.Y), (double)(player.Center.X - shorkCenter.X));
                if (NPC.spriteDirection == 1)
                {
                    getRotatedIdiot += 3.14159274f;
                }
                if (getRotatedIdiot < 0f)
                {
                    getRotatedIdiot += 6.28318548f;
                }
                if (getRotatedIdiot > 6.28318548f)
                {
                    getRotatedIdiot -= 6.28318548f;
                }
                if (NPC.ai[0] == -1f)
                {
                    getRotatedIdiot = 0f;
                }
                float rotationSpeed = 0.04f;
                if (NPC.ai[0] == 1f)
                {
                    rotationSpeed = 0f;
                }
                if (NPC.rotation < getRotatedIdiot)
                {
                    if ((double)(getRotatedIdiot - NPC.rotation) > 3.1415926535897931)
                    {
                        NPC.rotation -= rotationSpeed;
                    }
                    else
                    {
                        NPC.rotation += rotationSpeed;
                    }
                }
                if (NPC.rotation > getRotatedIdiot)
                {
                    if ((double)(NPC.rotation - getRotatedIdiot) > 3.1415926535897931)
                    {
                        NPC.rotation += rotationSpeed;
                    }
                    else
                    {
                        NPC.rotation -= rotationSpeed;
                    }
                }
                if (NPC.rotation > getRotatedIdiot - rotationSpeed && NPC.rotation < getRotatedIdiot + rotationSpeed)
                {
                    NPC.rotation = getRotatedIdiot;
                }
                if (NPC.rotation < 0f)
                {
                    NPC.rotation += 6.28318548f;
                }
                if (NPC.rotation > 6.28318548f)
                {
                    NPC.rotation -= 6.28318548f;
                }
                if (NPC.rotation > getRotatedIdiot - rotationSpeed && NPC.rotation < getRotatedIdiot + rotationSpeed)
                {
                    NPC.rotation = getRotatedIdiot;
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
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.dontTakeDamage = true;
                    NPC.chaseable = false;
                    NPC.velocity *= 0.98f;
                    int spriteFaceDirection = Math.Sign(player.Center.X - shorkCenter.X);
                    if (spriteFaceDirection != 0)
                    {
                        NPC.direction = spriteFaceDirection;
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
                    if (NPC.ai[2] == 60f)
                    {
                        int dustAmt = 36;
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 expr_80F = (Vector2.Normalize(NPC.velocity) * new Vector2((float)NPC.width / 2f, (float)NPC.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + NPC.Center;
                            Vector2 dustDirection = expr_80F - NPC.Center;
                            int chargeDust = Dust.NewDust(expr_80F + dustDirection, 0, 0, DustID.DungeonWater, dustDirection.X * 2f, dustDirection.Y * 2f, 100, default, 1.4f);
                            Main.dust[chargeDust].noGravity = true;
                            Main.dust[chargeDust].noLight = true;
                            Main.dust[chargeDust].velocity = Vector2.Normalize(dustDirection) * 3f;
                        }
                        SoundEngine.PlaySound(EnragedRoarSound, NPC.Center);
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 75f)
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
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.dontTakeDamage = false;
                    NPC.chaseable = true;
                    if (NPC.ai[1] == 0f)
                    {
                        NPC.ai[1] = (float)(300 * Math.Sign((shorkCenter - player.Center).X));
                    }
                    Vector2 chargeDirection = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - shorkCenter - NPC.velocity) * chargeThreshold;
                    if (NPC.velocity.X < chargeDirection.X)
                    {
                        NPC.velocity.X = NPC.velocity.X + chargeAcceleration;
                        if (NPC.velocity.X < 0f && chargeDirection.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + chargeAcceleration;
                        }
                    }
                    else if (NPC.velocity.X > chargeDirection.X)
                    {
                        NPC.velocity.X = NPC.velocity.X - chargeAcceleration;
                        if (NPC.velocity.X > 0f && chargeDirection.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - chargeAcceleration;
                        }
                    }
                    if (NPC.velocity.Y < chargeDirection.Y)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + chargeAcceleration;
                        if (NPC.velocity.Y < 0f && chargeDirection.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + chargeAcceleration;
                        }
                    }
                    else if (NPC.velocity.Y > chargeDirection.Y)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - chargeAcceleration;
                        if (NPC.velocity.Y > 0f && chargeDirection.Y < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - chargeAcceleration;
                        }
                    }
                    int shorkFacingSign = Math.Sign(player.Center.X - shorkCenter.X);
                    if (shorkFacingSign != 0)
                    {
                        if (NPC.ai[2] == 0f && shorkFacingSign != NPC.direction)
                        {
                            NPC.rotation += 3.14159274f;
                        }
                        NPC.direction = shorkFacingSign;
                        if (NPC.spriteDirection != -NPC.direction)
                        {
                            NPC.rotation += 3.14159274f;
                        }
                        NPC.spriteDirection = -NPC.direction;
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = Vector2.Normalize(player.Center - shorkCenter) * chargeVelocity;
                        NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X);
                        if (shorkFacingSign != 0)
                        {
                            NPC.direction = shorkFacingSign;
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
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    int phase2DustAmt = 7;
                    for (int j = 0; j < phase2DustAmt; j++)
                    {
                        Vector2 arg_E1C_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((float)(NPC.width + 50) / 2f, (float)NPC.height) * 0.75f).RotatedBy((double)(j - (phase2DustAmt / 2 - 1)) * 3.1415926535897931 / (double)(float)phase2DustAmt, default) + shorkCenter;
                        Vector2 phase2DustRotation = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                        int phase2Dust = Dust.NewDust(arg_E1C_0 + phase2DustRotation, 0, 0, DustID.DungeonWater, phase2DustRotation.X * 2f, phase2DustRotation.Y * 2f, 100, default, 1.4f);
                        Main.dust[phase2Dust].noGravity = true;
                        Main.dust[phase2Dust].noLight = true;
                        Main.dust[phase2Dust].velocity /= 4f;
                        Main.dust[phase2Dust].velocity -= NPC.velocity;
                    }
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (float)phase2Delay)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                        return;
                    }
                    if (Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] % 5 == 0)
                    {
                        Vector2 direction = shorkCenter - player.Center;
                        direction.Normalize();
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * 10f, ProjectileID.DemonSickle, 80, 0f, Main.myPlayer);
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
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
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
            NPC.frameCounter %= Main.npcFrameCount[Type];
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
            Asset<Texture2D> npcTexture = Main.zenithWorld ? ManTexture : TextureAssets.Npc[Type];
            Rectangle nframe = npcTexture.Frame(1, 4, 0, (int)NPC.frameCounter);
            Vector2 origin = new Vector2((float)(npcTexture.Value.Width / 2), (float)(npcTexture.Value.Height / Main.npcFrameCount[Type] / 2));
            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)npcTexture.Value.Width, (float)(npcTexture.Value.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            npcOffset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(npcTexture.Value, npcOffset, nframe, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<HadopelagicPressure>(), 300, true);
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
            postPolter.Add(ModContent.ItemType<ReaperTooth>(), 1, 6, 8);
            postPolter.Add(ModContent.ItemType<DeepSeaDumbbell>(), 3);
            postPolter.Add(ModContent.ItemType<Valediction>(), 3);

            var postLevi = npcLoot.DefineConditionalDropSet(DropHelper.PostLevi());
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 10, 17, 14, 22));
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
