using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Systems.Mechanic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CalClone
{
    [AutoloadBossHead]
    public class CalamitasClone : ModNPC
    {
        public static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle BulletHellWarning = new("CalamityMod/Sounds/Custom/CalamitasClone/BulletHellEnding");
        public static readonly SoundStyle BulletHellEnd = new("CalamityMod/Sounds/Custom/CalamitasClone/BulletHellEnd");
        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Custom/CalamitasClone/CalCloneDash", 3);
        public static readonly SoundStyle CalamitousFireballSound = new SoundStyle("CalamityMod/Sounds/Custom/CalamitasClone/CalClone_BigFireballBit", 4) with {MaxInstances = 4};
        public static readonly SoundStyle CalamitousExplosionSound = new SoundStyle("CalamityMod/Sounds/Custom/CalamitasClone/CalClone_Explosion", 3) with { MaxInstances = 4 };
        public SlotId BulletHellWarnSlot;

        public ArenaWallSystem.Box ArenaBox = null;
        void UpdateArena(ArenaWallSystem.Box box)
        {
            if (box.borderColor == Color.Gray || box.oldData.borderColor == Color.Gray)
                return;
            for (var i2 = 0; i2 < box.Size.Y / 400f; i2++)
            {
                var p = Vector2.Lerp(box.BottomRight, box.TopRight, Main.rand.NextFloat());
                Dust.NewDustPerfect(p, DustID.Clentaminator_Red, p.DirectionFrom(box.Center) * Main.rand.NextFloat(0, 5), Scale: Main.rand.NextFloat(0.1f, 1f), newColor: box.borderColor);

                p = Vector2.Lerp(box.TopLeft, box.BottomLeft, Main.rand.NextFloat());
                Dust.NewDustPerfect(p, DustID.Clentaminator_Red, p.DirectionFrom(box.Center) * Main.rand.NextFloat(0, 5), Scale: Main.rand.NextFloat(0.1f, 1f), newColor: box.borderColor);

            }
            for (var i2 = 0; i2 < box.Size.X / 400f; i2++)
            {
                var p = Vector2.Lerp(box.TopLeft, box.TopRight, Main.rand.NextFloat());
                Dust.NewDustPerfect(p, DustID.Clentaminator_Red, p.DirectionFrom(box.Center) * Main.rand.NextFloat(0, 5), Scale: Main.rand.NextFloat(0.1f, 1f), newColor: box.borderColor);
                p = Vector2.Lerp(box.BottomRight, box.BottomLeft, Main.rand.NextFloat());
                Dust.NewDustPerfect(p, DustID.Clentaminator_Red, p.DirectionFrom(box.Center) * Main.rand.NextFloat(0, 5), Scale: Main.rand.NextFloat(0.1f, 1f), newColor: box.borderColor);
            }
        }

        void DrawArena(ArenaWallSystem.Box box)
        {

            var color = Color.Black * 0.75f;
            //Inside Fill
            box.DrawBoxWithOffset(box.borderThickness * 0.5f, box.borderThickness, Color.Black * 0.75f);
            //Inner Border
            box.DrawBoxWithOffset(4, 8, box.borderColor);
            //Inner Border Clones
            float amount = 4;
            float totalDistance = 64f;
            for (var i = Main.GlobalTimeWrappedHourly % 1; i < amount; i++)
            {
                box.DrawBoxWithOffset(totalDistance * (i / amount) + 4, 4, box.borderColor * (1 - i / amount));
            }
            //Outer Border
            box.DrawBoxWithOffset(box.borderThickness - 4, 4, box.borderColor);
        }
        public static Vector4 GetArenaSize(bool brothersActive = false, float lifeRatio = 0, bool inBulletHell = false)
        {
            var baseSize = new Vector4(1600, 800, 0, 800);
            if (brothersActive)
                baseSize *= 1.25f;
            if (NPC.AnyNPCs(ModContent.NPCType<SoulSeeker>()))
                baseSize *= new Vector4(1.5f, 0.75f, 1.5f, 0.75f);
            if (!CalamityWorld.death)
                baseSize *= 1.25f;
            if (lifeRatio < 0.1f && !inBulletHell && CalamityWorld.death)
                baseSize *= MathHelper.Lerp(Main.getGoodWorld ? 0.22f : 0.4f, 1f, lifeRatio * 10f); // Scale down the lower health calclone has. Much lower bound on FTW.

            return baseSize + new Vector4(-22, 0, 22, 0);
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.65f
            };
            value.Position.Y -= 10f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int DartDamage = 22; // 88
        public static int HellblastDamage = 25; // 100
        public static int HellfireballDamage = 25; // 100
        public static int FireblastDamage = 35; // 140; Also applies to GFB Gigablasts

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 70; // 140
            NPC.npcSlots = 14f;
            NPC.width = 120;
            NPC.height = 120;

            NPC.defense = 25;
            NPC.value = Item.buyPrice(gold: 15);
            NPC.LifeMaxNERB(30000, 46875, 520000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Gives black background
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Calamitas")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);

            // Variables for increasing difficulty
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.7f || death;
            bool phase3 = lifeRatio < 0.35f;
            bool phase4 = lifeRatio <= 0.1f && death;

            // Don't take damage during bullet hells
            NPC.dontTakeDamage = calamityGlobalNPC.newAI[2] > 0f;

            // Variable for live brothers
            bool brotherAlive = false;

            // For seekers
            CalamityGlobalNPC.calamitas = NPC.whoAmI;

            // Seeker ring
            if (calamityGlobalNPC.newAI[1] == 0f && phase3 && expertMode)
            {
                SoundEngine.PlaySound(SoundID.Item72, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int seekerAmt = death ? 10 : 5;
                    int seekerSpread = 360 / seekerAmt;
                    int seekerDistance = death ? 180 : 150;
                    for (int i = 0; i < seekerAmt; i++)
                    {
                        int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * seekerSpread) * seekerDistance)), (int)(NPC.Center.Y + (Math.Cos(i * seekerSpread) * seekerDistance)), ModContent.NPCType<SoulSeeker>(), NPC.whoAmI, 0, 0, 0, -1);
                        Main.npc[spawn].ai[0] = i * seekerSpread;
                    }
                }

                string key = "Mods.CalamityMod.Status.Boss.CalamitasBossText3";
                Color messageColor = Color.Orange;
                CalamityUtils.BroadcastLocalizedText(key, messageColor);

                calamityGlobalNPC.newAI[1] = 1f;
            }

            // CIT 14SEP2024: Fixed bug where her phases would get offset by dealing a large amount of damage in a single hit to take her into a new phase.
            // newAI[0] now starts at 1 and has 0.3 subtracted from it for each phase, instead of it storing her current health when she enters a new phase.

            // Do bullet hell or spawn brothers
            if (calamityGlobalNPC.newAI[0] == 0f && NPC.life > 0)
                calamityGlobalNPC.newAI[0] = 1f;

            // Bullet hells at 70% and 10%, brothers at 40%
            if (NPC.life > 0)
            {
                int calClonePhaseThreshold = (int)(NPC.lifeMax * 0.3);
                if (((NPC.life + calClonePhaseThreshold) / (float)NPC.lifeMax) < calamityGlobalNPC.newAI[0])
                {
                    calamityGlobalNPC.newAI[0] -= 0.3f;
                    if (calamityGlobalNPC.newAI[0] <= 0.1f)
                    {
                        SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                        calamityGlobalNPC.newAI[2] = 2f;

                        if (Main.zenithWorld)
                            calamityGlobalNPC.newAI[3] = 0f;

                        SpawnDust();
                    }
                    else if (calamityGlobalNPC.newAI[0] <= 0.4f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.position.Y + NPC.height, ModContent.NPCType<Cataclysm>(), NPC.whoAmI);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.position.Y + NPC.height, ModContent.NPCType<Catastrophe>(), NPC.whoAmI);
                        }

                        string key = "Mods.CalamityMod.Status.Boss.CalamitasBossText2";
                        Color messageColor = Color.Orange;
                        CalamityUtils.BroadcastLocalizedText(key, messageColor);

                        SpawnDust();
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                        calamityGlobalNPC.newAI[2] = 1f;

                        if (Main.zenithWorld)
                            calamityGlobalNPC.newAI[3] = 0f;

                        SpawnDust();
                    }
                }
            }

            // Immunity if brothers are alive
            if (CalamityGlobalNPC.cataclysm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.cataclysm].active)
                    brotherAlive = true;
            }
            if (CalamityGlobalNPC.catastrophe != -1)
            {
                if (Main.npc[CalamityGlobalNPC.catastrophe].active)
                    brotherAlive = true;
            }

            if (brotherAlive)
                NPC.dontTakeDamage = true;

            bool inBulletHell = calamityGlobalNPC.newAI[2] > 0f;

            //arena in rev+
            if (revenge)
            {
                if (ArenaBox is null)
                {
                    ArenaBox = new()
                    {
                        position = Main.player[NPC.FindClosestPlayer()].Center,
                        boxDimensions = new Vector4(2000),
                        borderThickness = 2000,
                        RemovalCondition = () => !(Main.npc[NPC.whoAmI].active) || Main.npc[NPC.whoAmI].type != Type,
                        UpdateBox = UpdateArena,
                        DrawBox = DrawArena,
                        DespawnAction = (box) =>
                        {
                            box.boxDimensions += new Vector4(64);
                            if (box.Size.X > 5000)
                                return true;
                            return false;
                        }
                    };
                    ArenaWallSystem.ActiveBoxes.Add(ArenaBox);
                }
                ArenaBox.NewDimensions = Vector4.Lerp(ArenaBox.boxDimensions, GetArenaSize(brotherAlive, lifeRatio, inBulletHell), lifeRatio > 0.9f ? 0.1f : 0.025f);
                if (ArenaBox.oldData is not null)
                    ArenaBox.oldData.borderColor = Color.White;
                if (brotherAlive)
                    ArenaBox.borderColor = Color.Lerp(ArenaBox.borderColor, Color.Lerp(new Color(0, 255, 255), new Color(255, 0, 229), (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.5f) + 1) * 0.5f), 0.03f);
                else if (NPC.AnyNPCs(ModContent.NPCType<SoulSeeker>()))
                    ArenaBox.borderColor = Color.Lerp(ArenaBox.borderColor, Color.Lerp(Color.Crimson, new Color(255, 106, 0), (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.5f) + 1) * 0.5f), 0.03f);
                else if (NPC.dontTakeDamage)
                {
                    ArenaBox.borderColor = Color.Lerp(ArenaBox.borderColor, Color.Gray, 0.1f);
                    ArenaBox.oldData.borderColor = Color.Gray;
                }
                else
                    ArenaBox.borderColor = Color.Lerp(ArenaBox.borderColor, Color.Lerp(Color.Crimson, Color.IndianRed, (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.5f) + 1) * 0.25f), 0.03f);
            }

            void SpawnDust()
            {
                int dustAmt = 50;
                int random = 3;

                for (int j = 0; j < 10; j++)
                {
                    random += j;
                    int dustAmtSpawned = 0;
                    int scale = random * 6;
                    float dustPositionX = NPC.Center.X - (scale / 2);
                    float dustPositionY = NPC.Center.Y - (scale / 2);
                    while (dustAmtSpawned < dustAmt)
                    {
                        float dustVelocityX = Main.rand.Next(-random, random);
                        float dustVelocityY = Main.rand.Next(-random, random);
                        float dustVelocityScalar = random * 2f;
                        float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
                        dustVelocity = dustVelocityScalar / dustVelocity;
                        dustVelocityX *= dustVelocity;
                        dustVelocityY *= dustVelocity;
                        int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].position.X = NPC.Center.X;
                        Main.dust[dust].position.Y = NPC.Center.Y;
                        Main.dust[dust].position.X += Main.rand.Next(-10, 11);
                        Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
                        Main.dust[dust].velocity.X = dustVelocityX;
                        Main.dust[dust].velocity.Y = dustVelocityY;
                        dustAmtSpawned++;
                    }
                }
            }

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Target variable
            Player player = Main.player[NPC.target];


            //Invincible outside of arena
            if (ArenaBox is not null)
            {
                if (!Collision.CheckAABBvAABBCollision(ArenaBox.TopLeft,ArenaBox.Size,player.position,player.Size))
                    NPC.dontTakeDamage = true;
            }

            // Rotation
            Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 59f);
            Vector2 lookAt = new Vector2(player.position.X - (player.width / 2), player.position.Y - (player.height / 2));
            Vector2 rotationVector = npcCenter - lookAt;

            float rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            else if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            float rotationAmt = 0.1f;
            if (NPC.rotation < rotation)
            {
                if ((rotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= rotationAmt;
                else
                    NPC.rotation += rotationAmt;
            }
            else if (NPC.rotation > rotation)
            {
                if ((NPC.rotation - rotation) > MathHelper.Pi)
                    NPC.rotation += rotationAmt;
                else
                    NPC.rotation -= rotationAmt;
            }

            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;

            // Despawn
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellWarnSlot, out var warningSound) && warningSound.IsPlaying)
                        warningSound.Stop();

                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.alpha = 0;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Distance from destination where Cal Clone stops moving
            float movementDistanceGateValue = 100f;

            // How fast Cal Clone moves to the destination
            float baseVelocity = (expertMode ? 10f : 8.5f) * (NPC.ai[1] == 4f ? 1.4f : 1f);
            float baseAcceleration = (expertMode ? 0.18f : 0.155f) * (NPC.ai[1] == 4f ? 1.4f : 1f);
            if (revenge)
            {
                baseVelocity += 1.5f * (1f - lifeRatio);
                baseAcceleration += 0.03f * (1f - lifeRatio);
            }
            if (death)
            {
                baseVelocity += 1.5f * (1f - lifeRatio);
                baseAcceleration += 0.03f * (1f - lifeRatio);
            }
            if (Main.getGoodWorld)
            {
                baseVelocity *= 1.15f;
                baseAcceleration *= 1.15f;
            }

            // What side Cal Clone should be on, relative to the target
            int xPos = 1;
            if (NPC.Center.X < player.Center.X)
                xPos = -1;

            // How far Cal Clone should be from the target
            float averageDistance = 400f;
            float chargeDistance = phase4 ? 300f : 400f;

            // This is where Cal Clone should be
            Vector2 destination = (calamityGlobalNPC.newAI[2] > 0f || NPC.ai[1] == 0f) ? new Vector2(player.Center.X, player.Center.Y - averageDistance) :
                NPC.ai[1] == 1f ? new Vector2(player.Center.X + averageDistance * xPos, player.Center.Y) :
                new Vector2(player.Center.X + chargeDistance * xPos, player.Center.Y);

            // Add some random distance to the destination after certain attacks
            if (NPC.localAI[0] == 1f)
            {
                NPC.localAI[0] = 0f;
                NPC.localAI[2] = Main.rand.Next(-300, 301);
                NPC.netUpdate = true;
            }

            // Add a bit of randomness to the destination
            if (death)
            {
                if (NPC.ai[1] == 0f)
                    destination.X += NPC.localAI[2];
                else
                    destination.Y += NPC.localAI[2];
            }

            // How far Cal Clone is from where she's supposed to be
            Vector2 distanceFromDestination = destination - NPC.Center;

            // Movement
            if (NPC.ai[1] == 0f || NPC.ai[1] == 1f || NPC.ai[1] == 4f || calamityGlobalNPC.newAI[2] > 0f)
                CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, baseAcceleration, true);

            // Bullet hell phase
            if (calamityGlobalNPC.newAI[2] > 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (calamityGlobalNPC.newAI[3] < 900f)
                {
                    calamityGlobalNPC.newAI[3] += 1f;
                    NPC.dontTakeDamage = true;
                    NPC.alpha = 255;

                    float rotX = player.Center.X - NPC.Center.X;
                    float rotY = player.Center.Y - NPC.Center.Y;
                    NPC.rotation = (float)Math.Atan2(rotY, rotX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (calamityGlobalNPC.newAI[2] == 2f)
                        {
                            int type = ModContent.ProjectileType<BurningFireblast>();
                            int damage = FireblastDamage;
                            if (Main.zenithWorld)
                                type = ModContent.ProjectileType<BurningGigablast>();

                            float gigaBlastFrequency = Main.getGoodWorld ? 120f : expertMode ? 180f : 240f;
                            float projSpeed = 5f;
                            if (calamityGlobalNPC.newAI[3] <= 300f)
                            {
                                if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from top
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer);
                            }
                            else if (calamityGlobalNPC.newAI[3] <= 600f && calamityGlobalNPC.newAI[3] > 300f)
                            {
                                if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from right
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -projSpeed, 0f, type, damage, 0f, Main.myPlayer);
                            }
                            else if (calamityGlobalNPC.newAI[3] > 600f)
                            {
                                if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from top
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    NPC.ai[0] += 1f;
                    float hellblastGateValue = expertMode ? 12f : 16f;
                    if (NPC.ai[0] >= hellblastGateValue)
                    {
                        NPC.ai[0] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<CalamitousDart>();
                            int damage = HellblastDamage;
                            float projSpeed = 4f;
                            // Blasts aimed directly at the player's horizontal position, does not spawn during the second bullet hell
                            if (calamityGlobalNPC.newAI[3] % (hellblastGateValue * 6f) == 0f && calamityGlobalNPC.newAI[2] != 2f)
                            {
                                float distance = Main.rand.NextBool() ? -1000f : 1000f;
                                float velocity = distance == -1000f ? projSpeed : -projSpeed;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }

                            if (calamityGlobalNPC.newAI[3] < 300f) // Blasts from above
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }
                            else if (calamityGlobalNPC.newAI[3] < 600f) // Blasts from left and right
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 0.5f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 0.5f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }
                            else // Blasts from above, left, and right
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed - 1f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 1f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 1f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[3] == 900f - 360f)
                        BulletHellWarnSlot = SoundEngine.PlaySound(CalamitasClone.BulletHellWarning with { Volume = 0.75f }, player.Center);
                    if (calamityGlobalNPC.newAI[3] > 900f - 360f)
                    {
                        if (SoundEngine.TryGetActiveSound(BulletHellWarnSlot, out var warningSound) && warningSound.IsPlaying)
                            warningSound.Position = player.Center;
                    }
                }
                else
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    SoundEngine.PlaySound(CalamitasClone.BulletHellEnd with { Volume = 0.75f }, player.Center);

                    // Prevent bullshit charge hits when second bullet hell ends.
                    if (phase4)
                    {
                        NPC.ai[1] = 4f;
                        NPC.ai[2] = -105f;
                        NPC.TargetClosest();
                    }
                    else
                    {
                        if (death)
                        {
                            int AIState = Main.rand.Next(3);
                            switch (AIState)
                            {
                                case 0:
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                    break;
                                case 1:
                                    NPC.ai[1] = 1f;
                                    NPC.ai[2] = 0f;
                                    break;
                                case 2:
                                    NPC.ai[1] = 4f;
                                    NPC.ai[2] = -105f;
                                    NPC.TargetClosest();
                                    break;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        if (death)
                            NPC.localAI[0] = 1f;
                    }

                    NPC.netUpdate = true;

                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active)
                        {
                            if (projectile.type == ModContent.ProjectileType<CalamitousDart>() || projectile.type == ModContent.ProjectileType<BurningBolt>())
                            {
                                if (projectile.timeLeft > 60)
                                    projectile.timeLeft = 60;
                            }
                            else if (projectile.type == ModContent.ProjectileType<BurningFireblast>())
                            {
                                projectile.ai[2] = 1f;

                                if (projectile.timeLeft > 60)
                                    projectile.timeLeft = 60;
                            }
                        }
                    }
                }

                return;
            }
            else if (Main.zenithWorld)
            {
                if (calamityGlobalNPC.newAI[3] < 900f)
                    calamityGlobalNPC.newAI[3] += 1f;
                else
                    calamityGlobalNPC.newAI[3] = 0f;

                NPC.ai[0] += 1f;
                float hellblastGateValue = 30f;
                if (NPC.ai[0] >= hellblastGateValue)
                {
                    NPC.ai[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<CalamitousDart>();
                        int damage = HellblastDamage;
                        float projSpeed = 4f;
                        if (calamityGlobalNPC.newAI[3] % (hellblastGateValue * 6f) == 0f)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = distance == -1000f ? projSpeed : -projSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                        }

                        if (calamityGlobalNPC.newAI[3] < 300f) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer, 2f, 0f);
                        }
                        else if (calamityGlobalNPC.newAI[3] < 600f) // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 0.5f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 0.5f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                        }
                        else // Blasts from above, left, and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed - 1f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 1f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 1f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                        }
                    }
                }
            }

            NPC.alpha = NPC.dontTakeDamage ? 255 : 0;

            // Float above target and fire hellfireballs
            if (NPC.ai[1] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[2] += 1f;
                float phaseTimer = 400f - (death ? 120f * (1f - lifeRatio) : 0f);
                if (NPC.ai[2] >= phaseTimer || phase4)
                {
                    // Prevent going to other positions during brothers phase
                    if (!brotherAlive)
                        NPC.ai[1] = death && !phase4 && Main.rand.NextBool() ? 4f : 1f;

                    NPC.ai[2] = 0f;
                    if (death)
                        NPC.localAI[0] = 1f;

                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !brotherAlive)
                {
                    NPC.localAI[1] += 1f;
                    if (expertMode)
                        NPC.localAI[1] += death ? 2f * (1f - lifeRatio) : 1f - lifeRatio;
                    if (revenge)
                        NPC.localAI[1] += 0.5f;

                    if (NPC.localAI[1] >= 120f)
                    {
                        NPC.localAI[1] = 0f;
                        SoundEngine.PlaySound(CalamitousFireballSound, NPC.Center);

                        float projectileVelocity = expertMode ? 14f : 12.5f;
                        int type = ModContent.ProjectileType<CalamitousFireball>();
                        Vector2 predictionVector = Main.getGoodWorld ? player.velocity * 20f : Vector2.Zero;
                        Vector2 fireballVelocity = Vector2.Normalize(player.Center + predictionVector - NPC.Center) * projectileVelocity;
                        Vector2 offset = Vector2.Normalize(fireballVelocity) * 40f;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, fireballVelocity, type, HellfireballDamage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                        Main.projectile[proj].netUpdate = true;
                    }
                }
            }

            // Float to the side of the target and fire
            else if (NPC.ai[1] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient && !brotherAlive)
                {
                    NPC.localAI[1] += 1f;
                    if (revenge)
                        NPC.localAI[1] += 0.5f;
                    if (expertMode)
                        NPC.localAI[1] += 0.5f;

                    if (NPC.localAI[1] >= 50f && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                    {
                        NPC.localAI[1] = 0f;

                        float projectileVelocity = expertMode ? 12.5f : 11f;
                        int type = ModContent.ProjectileType<CalamitousDart>();
                        int damage = HellblastDamage;
                        Vector2 fireballVelocity = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                        Vector2 offset = Vector2.Normalize(fireballVelocity) * 40f;

                        if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            type = ModContent.ProjectileType<CalamitousFireball>();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, fireballVelocity, type, HellfireballDamage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                        }
                        else
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, fireballVelocity, type, damage, 0f, Main.myPlayer, 1f);
                            SoundEngine.PlaySound(CalamitousFireballSound, NPC.Center);
                        }
                    }
                }

                NPC.ai[2] += 1f;
                float phaseTimer = 240f - (death ? 60f * (1f - lifeRatio) : 0f);
                if (NPC.ai[2] >= phaseTimer || phase4)
                {
                    if (brotherAlive)
                        NPC.ai[1] = 0f;
                    else if (death && !phase4 && Main.rand.NextBool())
                        NPC.ai[1] = 0f;
                    else
                        NPC.ai[1] = phase2 && revenge ? 4f : 0f;

                    NPC.ai[2] = 0f;
                    if (death)
                        NPC.localAI[0] = 1f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[1] == 2f)
            {
                // Set damage
                NPC.damage = NPC.defDamage;
                SoundEngine.PlaySound(ChargeSound, NPC.Center);
                NPC.rotation = rotation;

                float chargeVelocity = phase4 ? 30f : death ? 28f : 25f;

                Vector2 vector = Vector2.Normalize(player.Center - NPC.Center);
                NPC.velocity = vector * chargeVelocity;

                NPC.ai[1] = 3f;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[1] == 3f) // Dashing time
            {
                // Set damage
                NPC.damage = NPC.defDamage;

                NPC.ai[2] += 1f;

                float chargeTime = phase4 ? 35f : death ? 40f : 45f;
                if (NPC.ai[2] >= chargeTime)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.velocity *= 0.9f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;
                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        NPC.velocity.Y = 0f;
                }
                else
                {
                    //VFX
                    for (var i = 0; i < 5; i++)
                    {
                        var p = CalamitasMetaball.SpawnParticle(NPC.Center + NPC.velocity + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(1f) * 64f, NPC.velocity.RotatedByRandom(0.25f) * 0.5f, 32f);
                        p.SizeScaling = 0.9f;
                    }
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - MathHelper.PiOver2;

                    // Leave behind slow hellblasts in Death Mode
                    if (Main.netMode != NetmodeID.MultiplayerClient && death && phase4 && NPC.ai[2] % 6f == 0f)
                    {
                        int type = ModContent.ProjectileType<CalamitousDart>();
                        Vector2 fireballVelocity = Main.getGoodWorld ? Main.rand.NextVector2CircularEdge(0.02f, 0.02f) : NPC.velocity * 0.01f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireballVelocity, type, HellblastDamage, 0f, Main.myPlayer, 1f, 0f, 2f); // ai[2] is used here to distinguish acceleration in its ai
                    }
                }

                if (NPC.ai[2] >= chargeTime + 10f)
                {
                    if (!phase4)
                        NPC.ai[3] += 1f;

                    NPC.ai[2] = 0f;

                    NPC.rotation = rotation;
                    NPC.netUpdate = true;

                    if (NPC.ai[3] >= 2f)
                    {
                        NPC.TargetClosest();
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        return;
                    }

                    NPC.ai[1] = 4f;
                }
            }

            // Prepare dash
            else
            {
                NPC.damage = 0;

                NPC.ai[2] += 1f;
                float telegraphDuration = phase4 ? 15f : 30f;


                float startTelegraphTime = phase4 ? -25f : -10f; // Start 40 frames before dash
                if (NPC.ai[2] >= startTelegraphTime && NPC.ai[2] < telegraphDuration && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Lines converge inward
                    if (Main.rand.NextBool(3))
                    {

                        Vector2 dustVel2 = (Vector2.UnitX).RotatedByRandom(100) * Main.rand.NextFloat(23f, 28f);

                        var p = CalamitasMetaball.SpawnParticle(NPC.Center + dustVel2.SafeNormalize(Vector2.UnitX) * 420, -dustVel2 * 1.2f, 16f * Main.rand.NextFloat(1.4f, 2.35f));
                        p.Scale = new(1f, 0.33f);
                        p.rotation = p.Velocity.ToRotation();
                        p.SizeScaling = 0.95f;
                    }
                }

                if (NPC.ai[2] >= telegraphDuration)
                {
                    NPC.ai[1] = 2f; // Start dash
                    NPC.ai[2] = 0f;
                    if (death)
                        NPC.localAI[0] = 1f;

                    NPC.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.npcFrameCount[Type] / 2));
            Color white = Color.White;
            float colorLerpAmt = 0.5f;
            int afterimageAmt = 6;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phase4 = lifeRatio <= 0.1f && death;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, white, colorLerpAmt);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 offset = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    offset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, offset, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            npcOffset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, npcOffset, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            Color color = Color.Lerp(Color.White, Color.Red, 0.5f);
            if (Main.zenithWorld)
            {
                color = Color.CornflowerBlue;
            }

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i++)
                {
                    Color extraAfterimageColor = color;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, white, colorLerpAmt);
                    extraAfterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 offset = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    offset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, offset, NPC.frame, extraAfterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            if (NPC.ai[1] == 4f)
            {
                // Same logic as in AI
                float telegraphDuration = phase4 ? 15f : 30f;
                float startTelegraphTime = phase4 ? -25f : -10f;

                float glowTimeElapsed = NPC.ai[2] - startTelegraphTime;
                float timeForMaxGlow = telegraphDuration - startTelegraphTime;

                float lifeFadeIn = Utils.GetLerpValue(0, timeForMaxGlow, glowTimeElapsed, true);

                float glowSine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f); // Period of full pulse
                float pulse = MathHelper.Lerp(0.7f, 1f, glowSine); // Least protruding to most protruding
                float finalGlowIntensity = pulse * lifeFadeIn;

                // Create 20 visual copies of calclone that draw behind to create a glowy outline effect
                for (int i = 0; i < 20; i++)
                {
                    float rotationOffset = (MathHelper.TwoPi * i / 15);
                    Vector2 glowOffset = rotationOffset.ToRotationVector2() * (3f + glowSine * 1f) * finalGlowIntensity;

                    // Use the drawPosition variable that incorporates the screen offset
                    Main.spriteBatch.Draw(texture, NPC.Center - screenPos + glowOffset, NPC.frame, Color.Red with { A = 150 } * finalGlowIntensity, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, npcOffset, NPC.frame, color, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CalamitasCloneBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Items
                int[] items = new int[]
                {
                    ModContent.ItemType<Oblivion>(),
                    ModContent.ItemType<Animosity>(),
                    ModContent.ItemType<LashesofChaos>(),
                    ModContent.ItemType<EntropysVigil>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<VoidofCalamity>()));
                normalOnly.Add(ModContent.ItemType<ChaosStone>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<Regenerator>(), DropHelper.NormalWeaponDropRateFraction);

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofHavoc>(), 1, 8, 10);
                normalOnly.Add(ModContent.ItemType<AshesofCalamity>(), 1, 25, 30);

                // Vanity
                normalOnly.Add(ModContent.ItemType<CalamitasCloneMask>(), 7);
                var calVanity = ItemDropRule.Common(ModContent.ItemType<HoodOfCalamity>(), 10);
                calVanity.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RobesOfCalamity>()));
                normalOnly.Add(calVanity);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<CalamitasCloneTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CalamitasCloneRelic>());

            // GFB Ashes of Annihilation drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<AshesofAnnihilation>(), 1, 6, 9), true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCalamitasClone, ModContent.ItemType<LoreCalamitasClone>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<Bandit>() }, DownedBossSystem.downedCalamitasClone);

            // Mark the Calamitas Clone as dead
            DownedBossSystem.downedCalamitasClone = true;
            CalamityNetcode.SyncWorld();
        }

        public override void BossLoot(ref int potionType) => potionType = ItemID.GreaterHealingPotion;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas5").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Calamitas6").Type, NPC.scale);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int brimDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }
    }
}
