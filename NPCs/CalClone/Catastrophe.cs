using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
    public class Catastrophe : ModNPC
    {
        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 0.8f,
                Scale = 0.5f
            };
            value.Position.Y -= 10;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int BallDamage = 22; // 88

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 54; // 108
            NPC.npcSlots = 5f;
            NPC.width = 120;
            NPC.height = 120;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;

            NPC.defense = 10;
            NPC.LifeMaxNERB(7000, 10000, 80000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
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
            int associatedNPCType = ModContent.NPCType<CalamitasClone>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Gives black background
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Catastrophe")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
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

            if (CalamityGlobalNPC.calamitas < 0 || !Main.npc[CalamityGlobalNPC.calamitas].active)
            {
                if (NPC.alpha < 255)
                {
                    NPC.velocity *= 0.9f;

                    NPC.alpha += 2;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;

                    int dustAmount = (int)Math.Round(MathHelper.Lerp(1f, 5f, (255 - NPC.alpha) / 255f));
                    for (int i = 0; i < dustAmount; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, 0f, -1f, 90, default, Main.rand.NextFloat(0.5f, 2f));
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].fadeIn = 1f;
                    }
                }
                else
                    NPC.active = false;

                return;
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            CalamityGlobalNPC.catastrophe = NPC.whoAmI;

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            float calCloneBroPlayerXDist = NPC.position.X + (NPC.width / 2) - player.position.X - (player.width / 2);
            float calCloneBroPlayerYDist = NPC.position.Y + NPC.height - 59f - player.position.Y - (player.height / 2);
            float calCloneBroRotation = (float)Math.Atan2(calCloneBroPlayerYDist, calCloneBroPlayerXDist) + MathHelper.PiOver2;
            if (calCloneBroRotation < 0f)
                calCloneBroRotation += MathHelper.TwoPi;
            else if (calCloneBroRotation > MathHelper.TwoPi)
                calCloneBroRotation -= MathHelper.TwoPi;

            float calCloneBroRotationSpeed = 0.15f;
            if (NPC.rotation < calCloneBroRotation)
            {
                if ((calCloneBroRotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= calCloneBroRotationSpeed;
                else
                    NPC.rotation += calCloneBroRotationSpeed;
            }
            else if (NPC.rotation > calCloneBroRotation)
            {
                if ((NPC.rotation - calCloneBroRotation) > MathHelper.Pi)
                    NPC.rotation += calCloneBroRotationSpeed;
                else
                    NPC.rotation -= calCloneBroRotationSpeed;
            }

            if (NPC.rotation > calCloneBroRotation - calCloneBroRotationSpeed && NPC.rotation < calCloneBroRotation + calCloneBroRotationSpeed)
                NPC.rotation = calCloneBroRotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > calCloneBroRotation - calCloneBroRotationSpeed && NPC.rotation < calCloneBroRotation + calCloneBroRotationSpeed)
                NPC.rotation = calCloneBroRotation;

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
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
                        NPC.netUpdate = true;
                    }

                    return;
                }
            }

            if (NPC.ai[1] == 0f)
            {
                float calCloneBroProjAttackMaxSpeed = 4.5f;
                float calCloneBroProjAttackAccel = 0.2f;

                if (Main.getGoodWorld)
                {
                    calCloneBroProjAttackMaxSpeed *= 1.15f;
                    calCloneBroProjAttackAccel *= 1.15f;
                }

                int calCloneBroProjAttackDirection = 1;
                if (NPC.Center.X < player.Center.X)
                    calCloneBroProjAttackDirection = -1;

                Vector2 calCloneBroProjLocation = NPC.Center;
                float calCloneBroProjTargetX = player.Center.X + (calCloneBroProjAttackDirection * 180) - calCloneBroProjLocation.X;
                float calCloneBroProjTargetY = player.Center.Y - calCloneBroProjLocation.Y;
                float calCloneBroProjTargetDist = (float)Math.Sqrt(calCloneBroProjTargetX * calCloneBroProjTargetX + calCloneBroProjTargetY * calCloneBroProjTargetY);

                if (expertMode)
                {
                    if (calCloneBroProjTargetDist > 300f)
                        calCloneBroProjAttackMaxSpeed += 0.5f;
                    if (calCloneBroProjTargetDist > 400f)
                        calCloneBroProjAttackMaxSpeed += 0.5f;
                    if (calCloneBroProjTargetDist > 500f)
                        calCloneBroProjAttackMaxSpeed += 0.55f;
                    if (calCloneBroProjTargetDist > 600f)
                        calCloneBroProjAttackMaxSpeed += 0.55f;
                    if (calCloneBroProjTargetDist > 700f)
                        calCloneBroProjAttackMaxSpeed += 0.6f;
                    if (calCloneBroProjTargetDist > 800f)
                        calCloneBroProjAttackMaxSpeed += 0.6f;
                }

                calCloneBroProjTargetDist = calCloneBroProjAttackMaxSpeed / calCloneBroProjTargetDist;
                calCloneBroProjTargetX *= calCloneBroProjTargetDist;
                calCloneBroProjTargetY *= calCloneBroProjTargetDist;

                if (NPC.velocity.X < calCloneBroProjTargetX)
                {
                    NPC.velocity.X += calCloneBroProjAttackAccel;
                    if (NPC.velocity.X < 0f && calCloneBroProjTargetX > 0f)
                        NPC.velocity.X += calCloneBroProjAttackAccel;
                }
                else if (NPC.velocity.X > calCloneBroProjTargetX)
                {
                    NPC.velocity.X -= calCloneBroProjAttackAccel;
                    if (NPC.velocity.X > 0f && calCloneBroProjTargetX < 0f)
                        NPC.velocity.X -= calCloneBroProjAttackAccel;
                }
                if (NPC.velocity.Y < calCloneBroProjTargetY)
                {
                    NPC.velocity.Y += calCloneBroProjAttackAccel;
                    if (NPC.velocity.Y < 0f && calCloneBroProjTargetY > 0f)
                        NPC.velocity.Y += calCloneBroProjAttackAccel;
                }
                else if (NPC.velocity.Y > calCloneBroProjTargetY)
                {
                    NPC.velocity.Y -= calCloneBroProjAttackAccel;
                    if (NPC.velocity.Y > 0f && calCloneBroProjTargetY < 0f)
                        NPC.velocity.Y -= calCloneBroProjAttackAccel;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= (180f - (death ? 90f * (1f - lifeRatio) : 0f)))
                {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0f;
                    NPC.target = 255;
                    NPC.netUpdate = true;
                }

                bool fireDelay = NPC.ai[2] > 120f || lifeRatio < 0.9f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height) && fireDelay)
                {
                    NPC.localAI[2] += 1f;
                    if (NPC.localAI[2] > 36f)
                    {
                        NPC.localAI[2] = 0f;
                        SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[1] += 1f;
                        if (revenge)
                            NPC.localAI[1] += 0.5f;

                        if (NPC.localAI[1] > 50f)
                        {
                            NPC.localAI[1] = 0f;
                            float calCloneBroProjSpeed = death ? 14f : 12f;
                            int type = ModContent.ProjectileType<CatastrophicCinder>();
                            calCloneBroProjLocation = NPC.Center + new Vector2(112,0).RotatedBy(NPC.rotation + MathHelper.PiOver2);
                            calCloneBroProjTargetX = player.position.X + (player.width / 2) - calCloneBroProjLocation.X;
                            calCloneBroProjTargetY = player.position.Y + (player.height / 2) - calCloneBroProjLocation.Y;
                            calCloneBroProjTargetDist = (float)Math.Sqrt(calCloneBroProjTargetX * calCloneBroProjTargetX + calCloneBroProjTargetY * calCloneBroProjTargetY);
                            calCloneBroProjTargetDist = calCloneBroProjSpeed / calCloneBroProjTargetDist;
                            calCloneBroProjTargetX *= calCloneBroProjTargetDist;
                            calCloneBroProjTargetY *= calCloneBroProjTargetDist;
                            calCloneBroProjTargetY += NPC.velocity.Y * 0.5f;
                            calCloneBroProjTargetX += NPC.velocity.X * 0.5f;
                            calCloneBroProjLocation.X -= calCloneBroProjTargetX;
                            calCloneBroProjLocation.Y -= calCloneBroProjTargetY;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), calCloneBroProjLocation.X, calCloneBroProjLocation.Y, calCloneBroProjTargetX, calCloneBroProjTargetY, type, BallDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            else
            {
                if (NPC.ai[1] == 1f)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    NPC.rotation = calCloneBroRotation;

                    float calCloneBroChargeSpeed = (NPC.AnyNPCs(ModContent.NPCType<Cataclysm>()) ? 12f : 16f) + (death ? 4f * (1f - lifeRatio) : 0f);
                    if (expertMode)
                        calCloneBroChargeSpeed += 2f;
                    if (revenge)
                        calCloneBroChargeSpeed += 2f;
                    if (Main.getGoodWorld)
                        calCloneBroChargeSpeed *= 1.25f;

                    Vector2 calCloneBroChargeCenter = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    float calCloneBroChargeTargetXDist = player.position.X + (player.width / 2) - calCloneBroChargeCenter.X;
                    float calCloneBroChargeTargetYDist = player.position.Y + (player.height / 2) - calCloneBroChargeCenter.Y;
                    float calCloneBroChargeTargetDistance = (float)Math.Sqrt(calCloneBroChargeTargetXDist * calCloneBroChargeTargetXDist + calCloneBroChargeTargetYDist * calCloneBroChargeTargetYDist);
                    calCloneBroChargeTargetDistance = calCloneBroChargeSpeed / calCloneBroChargeTargetDistance;
                    NPC.velocity.X = calCloneBroChargeTargetXDist * calCloneBroChargeTargetDistance;
                    NPC.velocity.Y = calCloneBroChargeTargetYDist * calCloneBroChargeTargetDistance;
                    NPC.ai[1] = 2f;

                    if (Main.zenithWorld)
                    {
                        SoundEngine.PlaySound(SupremeCalamitas.SupremeCalamitas.BrimstoneShotSound, NPC.Center);

                        int type = ModContent.ProjectileType<BurningBolt>();

                        int totalProjectiles = death ? 10 : revenge ? 8 : expertMode ? 6 : 4;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        float velocity = 5f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity);
                        float projectileVelocityToPass = velocity * 3f;
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, CalamitasClone.DartDamage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                        }

                        for (int i = 0; i < 6; i++)
                            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f);
                    }
                    return;
                }

                if (NPC.ai[1] == 2f)
                {
                    NPC.ai[2] += 1f + (death ? 0.5f * (1f - lifeRatio) : 0f);
                    if (expertMode)
                        NPC.ai[2] += 0.25f;
                    if (revenge)
                        NPC.ai[2] += 0.25f;

                    if (NPC.ai[2] >= 60f) //50
                    {
                        NPC.velocity.X *= 0.93f;
                        NPC.velocity.Y *= 0.93f;

                        if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                            NPC.velocity.X = 0f;
                        if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                            NPC.velocity.Y = 0f;
                    }
                    else
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - MathHelper.PiOver2;

                    if (NPC.ai[2] >= 90f) //80
                    {
                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;
                        NPC.rotation = calCloneBroRotation;
                        if (NPC.ai[3] >= 4f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                            return;
                        }
                        NPC.ai[1] = 1f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2));
            int afterimageAmt = 7;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimageDrawPos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimageDrawPos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color pinkLerp = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color extraAfterimageColor = pinkLerp;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, Color.White, 0.5f);
                    extraAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 extraAfterimageDrawPos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    extraAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    extraAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, extraAfterimageDrawPos, NPC.frame, extraAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, pinkLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CheckActive() => false;

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            IItemDropRuleCondition KilledLast = DropHelper.If(() => !NPC.AnyNPCs(ModContent.NPCType<Cataclysm>()), desc: DropHelper.CatastropheKilledLast);
            npcLoot.Add(ItemDropRule.ByCondition(KilledLast, ModContent.ItemType<CatastropheTrophy>(), 5));
            npcLoot.Add(ItemDropRule.ByCondition(KilledLast, ModContent.ItemType<CrushsawCrasher>()));
        }

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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Catastrophe").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Catastrophe2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Catastrophe3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Catastrophe4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Catastrophe5").Type, NPC.scale);
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
