using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    [LongDistanceNetSync]
    public class GulperEelHead : ModNPC
    {
        private Vector2 patrolSpot = Vector2.Zero;
        public bool detectsPlayer = false;
        public const int minLength = 20;
        public const int maxLength = 21;
        public float speed = 5f; //10
        public float turnSpeed = 0.075f; //0.15
        bool TailSpawned = false;
        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.75f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/GulperEel_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 20
            };
            value.Position.X += 50;
            value.Position.Y += 20;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 135;
            NPC.width = 40; //36
            NPC.height = 84; //20
            NPC.defense = 10;
            NPC.lifeMax = 48000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath13;
            NPC.netAlways = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<GulperEelBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<AbyssLayer3Biome>().Type, ModContent.GetInstance<AbyssLayer4Biome>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.GulperEel")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(patrolSpot);
            writer.Write(detectsPlayer);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            patrolSpot = reader.ReadVector2();
            detectsPlayer = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Point point = NPC.Center.ToTileCoordinates();
            Tile tileSafely = Framing.GetTileSafely(point);
            bool createDust = tileSafely.HasUnactuatedTile && NPC.Distance(Main.player[NPC.target].Center) < 800f;
            if (createDust)
            {
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.TreasureSparkle, 0f, 0f, 150, default(Color), 0.3f);
                    dust.fadeIn = 0.75f;
                    dust.velocity *= 0.1f;
                    dust.noLight = true;
                }
            }

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
                NPC.TargetClosest();

            if ((Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(160f) || NPC.justHit)
                detectsPlayer = true;

            NPC.chaseable = detectsPlayer;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int lol;
                        if (segments >= 0 && segments < minLength)
                        {
                            if (segments == 0)
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<GulperEelBody>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<GulperEelBodyAlt>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<GulperEelTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = (float)NPC.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }

                    TailSpawned = true;
                }
            }

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = 1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = -1;

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 5600f)
            {
                NPC.TargetClosest(false);
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 5600f)
                    NPC.active = false;
            }

            float currentSpeed = speed;
            float currentTurnSpeed = turnSpeed;
            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);

            if (patrolSpot == Vector2.Zero)
                patrolSpot = Main.player[NPC.target].Center;

            float targetXDirection = detectsPlayer ? Main.player[NPC.target].Center.X : patrolSpot.X;
            float targetYDirection = detectsPlayer ? Main.player[NPC.target].Center.Y : patrolSpot.Y;

            if (!detectsPlayer)
            {
                targetYDirection += 500;
                if (Math.Abs(NPC.Center.X - targetXDirection) < 300f) //500
                {
                    if (NPC.velocity.X > 0f)
                    {
                        targetXDirection += 400f;
                    }
                    else
                    {
                        targetXDirection -= 400f;
                    }
                }
            }
            else
            {
                currentSpeed *= 1.5f;
                currentTurnSpeed *= 1.5f;
            }
            float maxCurrentSpeed = currentSpeed * 1.3f;
            float minCurrentSpeed = currentSpeed * 0.7f;
            float speedCompare = NPC.velocity.Length();
            if (speedCompare > 0f)
            {
                if (speedCompare > maxCurrentSpeed)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= maxCurrentSpeed;
                }
                else if (speedCompare < minCurrentSpeed)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= minCurrentSpeed;
                }
            }
            targetXDirection = (float)((int)(targetXDirection / 16f) * 16);
            targetYDirection = (float)((int)(targetYDirection / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetXDirection -= segmentPosition.X;
            targetYDirection -= segmentPosition.Y;
            float targetDistance = (float)System.Math.Sqrt((double)(targetXDirection * targetXDirection + targetYDirection * targetYDirection));
            float absoluteTargetX = System.Math.Abs(targetXDirection);
            float absoluteTargetY = System.Math.Abs(targetYDirection);
            float timeToReachTarget = currentSpeed / targetDistance;
            targetXDirection *= timeToReachTarget;
            targetYDirection *= timeToReachTarget;
            if ((NPC.velocity.X > 0f && targetXDirection > 0f) || (NPC.velocity.X < 0f && targetXDirection < 0f) || (NPC.velocity.Y > 0f && targetYDirection > 0f) || (NPC.velocity.Y < 0f && targetYDirection < 0f))
            {
                if (NPC.velocity.X < targetXDirection)
                {
                    NPC.velocity.X = NPC.velocity.X + currentTurnSpeed;
                }
                else
                {
                    if (NPC.velocity.X > targetXDirection)
                    {
                        NPC.velocity.X = NPC.velocity.X - currentTurnSpeed;
                    }
                }
                if (NPC.velocity.Y < targetYDirection)
                {
                    NPC.velocity.Y = NPC.velocity.Y + currentTurnSpeed;
                }
                else
                {
                    if (NPC.velocity.Y > targetYDirection)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - currentTurnSpeed;
                    }
                }
                if ((double)System.Math.Abs(targetYDirection) < (double)currentSpeed * 0.2 && ((NPC.velocity.X > 0f && targetXDirection < 0f) || (NPC.velocity.X < 0f && targetXDirection > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + currentTurnSpeed * 2f;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - currentTurnSpeed * 2f;
                    }
                }
                if ((double)System.Math.Abs(targetXDirection) < (double)currentSpeed * 0.2 && ((NPC.velocity.Y > 0f && targetYDirection < 0f) || (NPC.velocity.Y < 0f && targetYDirection > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + currentTurnSpeed * 2f; //changed from 2
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - currentTurnSpeed * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (absoluteTargetX > absoluteTargetY)
                {
                    if (NPC.velocity.X < targetXDirection)
                    {
                        NPC.velocity.X = NPC.velocity.X + currentTurnSpeed * 1.1f; //changed from 1.1
                    }
                    else if (NPC.velocity.X > targetXDirection)
                    {
                        NPC.velocity.X = NPC.velocity.X - currentTurnSpeed * 1.1f; //changed from 1.1
                    }
                    if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)currentSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + currentTurnSpeed;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - currentTurnSpeed;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targetYDirection)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + currentTurnSpeed * 1.1f;
                    }
                    else if (NPC.velocity.Y > targetYDirection)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - currentTurnSpeed * 1.1f;
                    }
                    if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)currentSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + currentTurnSpeed;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - currentTurnSpeed;
                        }
                    }
                }
            }
            NPC.rotation = (float)System.Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            vector -= new Vector2((float)GlowTexture.Value.Width, (float)(GlowTexture.Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += halfSizeTexture * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
            Main.spriteBatch.Draw(GlowTexture.Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, halfSizeTexture, 1f, spriteEffects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<GulperEelHead>()))
                return Main.remixWorld ? 2.7f : SpawnCondition.CaveJellyfish.Chance * 0.3f;

            if (spawnInfo.Player.Calamity().ZoneAbyssLayer4 && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<GulperEelHead>()))
                return Main.remixWorld ? 5.4f : SpawnCondition.CaveJellyfish.Chance * 0.6f;

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var postLevi = npcLoot.DefineConditionalDropSet(DropHelper.PostLevi());
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 2, 2, 3, 3, 4));
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 6, 8, 8, 11));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GulperEel").Type, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }
    }
}
