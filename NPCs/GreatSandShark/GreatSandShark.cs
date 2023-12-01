using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.NPCs.Astral;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.GreatSandShark
{
    [AutoloadBossHead]
    public class GreatSandShark : ModNPC
    {
        private bool resetAI = false;

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/GreatSandSharkRoar");
	public static readonly SoundStyle HurtSound = new("CalamityMod/Sounds/NPCHit/GreatSandSharkHit");
	public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/GreatSandSharkDeath");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.6f,
                PortraitPositionXOverride = 70f
            };
            value.Position.X += 60f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 15f;
            NPC.damage = 100;
            NPC.width = 300;
            NPC.height = 120;
            NPC.defense = 40;
            NPC.DR_NERD(0.25f);
            NPC.LifeMaxNERB(9200, 11000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.DeathSound = DeathSound;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.rarity = 2;
            if (Main.zenithWorld)
            {
                NPC.Calamity().VulnerableToHeat = true;
                NPC.Calamity().VulnerableToSickness = false;
            }
            else
            {
                NPC.Calamity().VulnerableToCold = true;
                NPC.Calamity().VulnerableToSickness = true;
                NPC.Calamity().VulnerableToWater = true;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Sandstorm,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.GreatSandShark")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(resetAI);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(NPC.Calamity().newAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            resetAI = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            NPC.Calamity().newAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode;
            bool revenge = CalamityWorld.revenge;
            bool death = CalamityWorld.death;
            bool lowLife = NPC.life <= NPC.lifeMax * (expertMode ? 0.75 : 0.5);
            bool lowerLife = NPC.life <= NPC.lifeMax * (expertMode ? 0.35 : 0.2);
            bool youMustDie = !Main.player[NPC.target].ZoneDesert;

            if (!Terraria.GameContent.Events.Sandstorm.Happening)
            {
                CalamityUtils.StartSandstorm();
                CalamityNetcode.SyncWorld();
            }

            if (NPC.soundDelay <= 0)
            {
                NPC.soundDelay = 480;
                SoundEngine.PlaySound(RoarSound, NPC.position);
            }

            if (NPC.localAI[3] >= 1f || Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 1000f)
            {
                if (!resetAI)
                {
                    NPC.localAI[0] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    resetAI = true;
                    NPC.netUpdate = true;
                }

                int chargeTime = expertMode ? 35 : 50;
                float chargeAcceleration = expertMode ? 0.5f : 0.42f;
                float chargeThreshold = expertMode ? 7.5f : 6.7f;
                int chargeDelay = expertMode ? 28 : 30;
                float chargeVelocity = expertMode ? 15.5f : 14f;
                if (revenge || lowerLife)
                {
                    chargeAcceleration *= 1.1f;
                    chargeThreshold *= 1.1f;
                    chargeVelocity *= 1.1f;
                }
                if (death)
                {
                    chargeAcceleration *= 1.1f;
                    chargeThreshold *= 1.1f;
                    chargeVelocity *= 1.1f;
                    chargeDelay = 25;
                }
                if (youMustDie)
                {
                    chargeAcceleration *= 1.5f;
                    chargeThreshold *= 1.5f;
                    chargeVelocity *= 1.5f;
                    chargeDelay = 20;
                }

                Vector2 shorkCenter = NPC.Center;
                Player player = Main.player[NPC.target];

                if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                {
                    NPC.TargetClosest();
                    player = Main.player[NPC.target];
                    NPC.netUpdate = true;
                }

                if (player.dead || Vector2.Distance(player.Center, shorkCenter) > 5600f)
                {
                    NPC.velocity.Y += 0.4f;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    NPC.ai[0] = 0f;
                    NPC.ai[2] = 0f;
                }

                float getRotatedIdiot = (float)Math.Atan2(player.Center.Y - shorkCenter.Y, player.Center.X - shorkCenter.X);
                if (NPC.spriteDirection == 1)
                    getRotatedIdiot += MathHelper.Pi;
                if (getRotatedIdiot < 0f)
                    getRotatedIdiot += MathHelper.TwoPi;
                if (getRotatedIdiot > MathHelper.TwoPi)
                    getRotatedIdiot -= MathHelper.TwoPi;

                float rotationSpeed = 0.04f;
                if (NPC.ai[0] == 1f)
                    rotationSpeed = 0f;

                if (NPC.rotation < getRotatedIdiot)
                {
                    if ((double)(getRotatedIdiot - NPC.rotation) > MathHelper.Pi)
                        NPC.rotation -= rotationSpeed;
                    else
                        NPC.rotation += rotationSpeed;
                }
                if (NPC.rotation > getRotatedIdiot)
                {
                    if ((double)(NPC.rotation - getRotatedIdiot) > MathHelper.Pi)
                        NPC.rotation += rotationSpeed;
                    else
                        NPC.rotation -= rotationSpeed;
                }

                if (NPC.rotation > getRotatedIdiot - rotationSpeed && NPC.rotation < getRotatedIdiot + rotationSpeed)
                    NPC.rotation = getRotatedIdiot;

                if (NPC.rotation < 0f)
                    NPC.rotation += MathHelper.TwoPi;
                if (NPC.rotation > MathHelper.TwoPi)
                    NPC.rotation -= MathHelper.TwoPi;

                if (NPC.rotation > getRotatedIdiot - rotationSpeed && NPC.rotation < getRotatedIdiot + rotationSpeed)
                    NPC.rotation = getRotatedIdiot;

                if (NPC.ai[0] == 0f && !player.dead)
                {
                    if (NPC.ai[1] == 0f)
                        NPC.ai[1] = 300 * Math.Sign((shorkCenter - player.Center).X);

                    Vector2 chargeDirection = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - shorkCenter - NPC.velocity) * chargeThreshold;
                    if (NPC.velocity.X < chargeDirection.X)
                    {
                        NPC.velocity.X += chargeAcceleration;
                        if (NPC.velocity.X < 0f && chargeDirection.X > 0f)
                            NPC.velocity.X += chargeAcceleration;
                    }
                    else if (NPC.velocity.X > chargeDirection.X)
                    {
                        NPC.velocity.X -= chargeAcceleration;
                        if (NPC.velocity.X > 0f && chargeDirection.X < 0f)
                            NPC.velocity.X -= chargeAcceleration;
                    }
                    if (NPC.velocity.Y < chargeDirection.Y)
                    {
                        NPC.velocity.Y += chargeAcceleration;
                        if (NPC.velocity.Y < 0f && chargeDirection.Y > 0f)
                            NPC.velocity.Y += chargeAcceleration;
                    }
                    else if (NPC.velocity.Y > chargeDirection.Y)
                    {
                        NPC.velocity.Y -= chargeAcceleration;
                        if (NPC.velocity.Y > 0f && chargeDirection.Y < 0f)
                            NPC.velocity.Y -= chargeAcceleration;
                    }

                    int shorkFaceDirection = Math.Sign(player.Center.X - shorkCenter.X);
                    if (shorkFaceDirection != 0)
                    {
                        if (NPC.ai[2] == 0f && shorkFaceDirection != NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.direction = shorkFaceDirection;
                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= chargeTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = Vector2.Normalize(player.Center - shorkCenter) * chargeVelocity;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        if (shorkFaceDirection != 0)
                        {
                            NPC.direction = shorkFaceDirection;
                            if (NPC.spriteDirection == 1)
                                NPC.rotation += MathHelper.Pi;

                            NPC.spriteDirection = -NPC.direction;
                        }

                        NPC.netUpdate = true;
                        return;
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= chargeDelay)
                    {
                        NPC.localAI[3] += 1f;
                        if (NPC.localAI[3] >= 2f)
                            NPC.localAI[3] = 0f;

                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                        return;
                    }
                }
            }
            else
            {
                resetAI = false;
                if (NPC.direction == 0)
                    NPC.TargetClosest();

                Point shorkTileCenter = NPC.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(shorkTileCenter);
                bool isInSolidTile = tileSafely.HasUnactuatedTile || tileSafely.LiquidAmount > 0;
                bool shouldDoLunge = false;
                NPC.TargetClosest(false);

                Vector2 targetLungeDirection = NPC.targetRect.Center.ToVector2();
                if (Main.player[NPC.target].velocity.Y > -0.1f && !Main.player[NPC.target].dead && NPC.Distance(targetLungeDirection) > 150f)
                    shouldDoLunge = true;

                NPC.localAI[1] += 1f;

                if (lowLife)
                {
                    bool spawnFlag = NPC.localAI[1] == 150f;
                    if (NPC.CountNPCS(NPCID.SandShark) > 2)
                        spawnFlag = false;

                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int npcType = Main.zenithWorld ? ModContent.NPCType<FusionFeeder>() : NPCID.SandShark;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, npcType, 0, 0f, 0f, 0f, 0f, 255);
                        SoundEngine.PlaySound(RoarSound, NPC.position);
                    }
                }

                if (NPC.localAI[1] >= 300f)
                {
                    NPC.localAI[1] = 0f;
                    if (NPC.localAI[2] > 0f)
                        NPC.localAI[2] = 0f;

                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.ai[3] = 0f;
                            break;
                        case 1:
                            NPC.ai[3] = 1f;
                            break;
                        case 2:
                            NPC.ai[3] = 2f;
                            break;
                    }

                    int random = lowerLife ? 5 : 9;
                    if (lowLife && Main.rand.NextBool(random))
                        NPC.localAI[3] = 1f;

                    NPC.netUpdate = true;
                }

                if (NPC.localAI[0] == -1f && !isInSolidTile)
                    NPC.localAI[0] = 20f;
                if (NPC.localAI[0] > 0f)
                    NPC.localAI[0] -= 1f;

                if (isInSolidTile)
                {
                    bool lungeShouldDecelerate = false;
                    shorkTileCenter = (NPC.Center + new Vector2(0f, 24f)).ToTileCoordinates();
                    tileSafely = Framing.GetTileSafely(shorkTileCenter.X, shorkTileCenter.Y - 2);
                    if (tileSafely.HasUnactuatedTile)
                        lungeShouldDecelerate = true;

                    NPC.ai[1] = lungeShouldDecelerate.ToInt();
                    if (NPC.ai[2] < 30f)
                        NPC.ai[2] += 1f;

                    if (shouldDoLunge)
                    {
                        NPC.TargetClosest();
                        NPC.velocity.X += NPC.direction * 0.15f;
                        NPC.velocity.Y += NPC.directionY * 0.15f;
                        float velocityX = 8f;
                        float velocityY = 6f;
                        switch ((int)NPC.ai[3])
                        {
                            case 0:
                                velocityX = 10f;
                                velocityY = 9f;
                                break;
                            case 1:
                                velocityX = 14f;
                                velocityY = 7f;
                                break;
                            case 2:
                                velocityX = 8f;
                                velocityY = 11f;
                                break;
                        }
                        if (revenge || lowerLife)
                        {
                            velocityX *= 1.1f;
                            velocityY *= 1.1f;
                        }
                        if (youMustDie)
                        {
                            velocityX *= 1.5f;
                            velocityY *= 1.5f;
                        }
                        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -velocityX, velocityX);
                        NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -velocityY, velocityY);
                        Vector2 shorkDirection = NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * NPC.Size.Length() / 2f + NPC.velocity;
                        shorkTileCenter = shorkDirection.ToTileCoordinates();
                        tileSafely = Framing.GetTileSafely(shorkTileCenter);
                        bool isInTile = tileSafely.HasUnactuatedTile;
                        if (!isInTile && Math.Sign(NPC.velocity.X) == NPC.direction && (NPC.Distance(targetLungeDirection) < 600f || youMustDie) && (NPC.ai[2] >= 30f || NPC.ai[2] < 0f))
                        {
                            if (NPC.localAI[0] == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath15, NPC.position);
                                NPC.localAI[0] = -1f;
                                for (int i = 0; i < 25; i++)
                                {
                                    int burrowDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 32, 0f, 0f, 100, default, 2f);
                                    Main.dust[burrowDust].velocity.Y *= 6f;
                                    Main.dust[burrowDust].velocity.X *= 3f;
                                    if (Main.rand.NextBool())
                                    {
                                        Main.dust[burrowDust].scale = 0.5f;
                                        Main.dust[burrowDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                    }
                                }
                                for (int j = 0; j < 50; j++)
                                {
                                    int burrowDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 85, 0f, 0f, 100, default, 3f);
                                    Main.dust[burrowDust2].noGravity = true;
                                    Main.dust[burrowDust2].velocity.Y *= 10f;
                                    burrowDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 268, 0f, 0f, 100, default, 2f);
                                    Main.dust[burrowDust2].velocity.X *= 2f;
                                }
                                int spawnX = (int)(NPC.width / 2);
                                int projType = Main.zenithWorld ? ModContent.ProjectileType<AstralMeteorProj>() : ModContent.ProjectileType<GreatSandBlast>();
                                for (int sand = 0; sand < 5; sand++)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (float)Main.rand.Next(-spawnX, spawnX), NPC.Center.Y,
                                        (float)Main.rand.Next(-3, 4), (float)Main.rand.Next(-12, -6), projType, 40, 0f, Main.myPlayer);
                            }
                            NPC.ai[2] = -30f;

                            Vector2 upwardChargeDirection = NPC.SafeDirectionTo(targetLungeDirection + new Vector2(0f, -80f), -Vector2.UnitY);
                            NPC.velocity = upwardChargeDirection * 18f;
                        }
                    }
                    else
                    {
                        float decelerationXThreshold = 6f;
                        NPC.velocity.X += NPC.direction * 0.1f;
                        if (NPC.velocity.X < -decelerationXThreshold || NPC.velocity.X > decelerationXThreshold)
                            NPC.velocity.X *= 0.95f;

                        if (lungeShouldDecelerate)
                            NPC.ai[0] = -1f;
                        else
                            NPC.ai[0] = 1f;

                        float decelerationYThreshold = 0.06f;
                        float decelerationAmt = 0.01f;
                        if (NPC.ai[0] == -1f)
                        {
                            NPC.velocity.Y -= decelerationAmt;
                            if (NPC.velocity.Y < -decelerationYThreshold)
                                NPC.ai[0] = 1f;
                        }
                        else
                        {
                            NPC.velocity.Y += decelerationAmt;
                            if (NPC.velocity.Y > decelerationYThreshold)
                                NPC.ai[0] = -1f;
                        }

                        if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
                            NPC.velocity.Y *= 0.95f;
                    }
                }
                else
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        if (shouldDoLunge)
                            NPC.TargetClosest();

                        float smallDecelerationXThreshold = 1f;
                        NPC.velocity.X += NPC.direction * 0.1f;
                        if (NPC.velocity.X < -smallDecelerationXThreshold || NPC.velocity.X > smallDecelerationXThreshold)
                            NPC.velocity.X *= 0.95f;
                    }

                    if (NPC.localAI[2] == 0f)
                    {
                        NPC.localAI[2] = 1f;
                        float velocityX = 12f;
                        float velocityY = 12f;
                        switch ((int)NPC.ai[3])
                        {
                            case 0:
                                velocityX = 12f;
                                velocityY = 12f;
                                break;
                            case 1:
                                velocityX = 14f;
                                velocityY = 14f;
                                break;
                            case 2:
                                velocityX = 16f;
                                velocityY = 16f;
                                break;
                        }
                        if (revenge || lowerLife)
                        {
                            velocityX *= 1.1f;
                            velocityY *= 1.1f;
                        }
                        if (youMustDie)
                        {
                            velocityX *= 1.5f;
                            velocityY *= 1.5f;
                        }
                        NPC.velocity.Y = -velocityY;
                        NPC.velocity.X = velocityX * NPC.direction;
                        NPC.netUpdate = true;
                    }

                    NPC.velocity.Y += 0.4f;
                    if (NPC.velocity.Y > 10f)
                        NPC.velocity.Y = 10f;

                    NPC.ai[0] = 1f;
                }
                NPC.rotation = NPC.velocity.Y * NPC.direction * 0.1f;
                NPC.rotation = MathHelper.Clamp(NPC.rotation, -0.1f, 0.1f);
            }

            if (Main.zenithWorld)
            {
                NPC.Calamity().newAI[0]++;
                if (NPC.Calamity().newAI[0] >= 120)
                {
                    SoundEngine.PlaySound(SoundID.Item105, Main.player[NPC.target].Center);
                    for (int i = 0; i < 5; i++)
                    {
                        float speedX = 2f + (float)Main.rand.Next(-8, 5);
                        float speedY = 2f + (float)Main.rand.Next(1, 6);
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, Main.player[NPC.target].Center.Y - 800, speedX, speedY, ModContent.ProjectileType<AstralFlame>(), 40, 0, Main.myPlayer);
                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].timeLeft = 180;
                        }
                    }
                    NPC.Calamity().newAI[0] = 0;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[3] == 0f)
            {
                NPC.spriteDirection = -NPC.direction;
            }
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color mainAfterimageColor = NPC.GetAlpha(drawColor);
            Color extraAfterimageColor = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            if (Main.zenithWorld)
            {
                mainAfterimageColor = Color.Silver;
                extraAfterimageColor = Color.Orange;
            }
            Texture2D texture2D3 = TextureAssets.Npc[NPC.type].Value;
            int currentFrame = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            int y3 = currentFrame * (int)NPC.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, currentFrame);
            Vector2 halfRectSize = rectangle.Size() / 2f;
            int eightConst = 8;
            int afterimageInc = 2;
            int afterimageCounter = 1;
            while (((afterimageInc > 0 && afterimageCounter < eightConst) || (afterimageInc < 0 && afterimageCounter > eightConst)) && CalamityConfig.Instance.Afterimages)
            {
                Color alphaAfterimageColor = NPC.GetAlpha(extraAfterimageColor);
                {
                    goto IL_6899;
                }
                IL_6881:
                afterimageCounter += afterimageInc;
                continue;
                IL_6899:
                float afterimagesRemaining = (float)(eightConst - afterimageCounter);
                if (afterimageInc < 0)
                {
                    afterimagesRemaining = (float)(1 - afterimageCounter);
                }
                alphaAfterimageColor *= afterimagesRemaining / ((float)NPCID.Sets.TrailCacheLength[NPC.type] * 1.5f);
                Vector2 afterimagePos = NPC.oldPos[afterimageCounter];
                float afterimageRotation = NPC.rotation;
                Main.spriteBatch.Draw(texture2D3, afterimagePos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), alphaAfterimageColor, afterimageRotation + NPC.rotation * 0f * (float)(afterimageCounter - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), halfRectSize, NPC.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            var something = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D3, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, mainAfterimageColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, something, 0);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
	    // Hit sound
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(HurtSound, NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int burrowDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[burrowDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[burrowDust].scale = 0.5f;
                        Main.dust[burrowDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 100; j++)
                {
                    int burrowDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[burrowDust2].noGravity = true;
                    Main.dust[burrowDust2].velocity *= 5f;
                    burrowDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[burrowDust2].velocity *= 2f;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.AncientBattleArmorMaterial);

            // Great Sand Shark drops the Desert Key
            npcLoot.Add(ItemID.DungeonDesertKey, 3);

            // 1 Grand Scale guaranteed; on Expert, 33% chance of getting a second one
            npcLoot.Add(ModContent.ItemType<GrandScale>());
            npcLoot.AddIf(() => Main.expertMode, ModContent.ItemType<GrandScale>(), 3);

            npcLoot.Add(ItemID.LightShard, 2);
            npcLoot.Add(ItemID.DarkShard, 2);

            // Trophy
            npcLoot.Add(ModContent.ItemType<GreatSandSharkTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<GreatSandSharkRelic>());
        }

        public override void OnKill()
        {
            // Mark Great Sand Shark as dead
            DownedBossSystem.downedGSS = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 600, true);
        }
    }
}
