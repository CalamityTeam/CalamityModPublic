using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.BiomeManagers.BestiaryCategories;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class FusionFeeder : ModNPC
    {
        public static Asset<Texture2D> glowmask;

        public Player Target => Main.player[NPC.target];

        public ref float VerticalMovementDirection => ref NPC.ai[0];

        public ref float ChargeDelay => ref NPC.ai[2];

        public ref float SearchSoundCreationDelay => ref NPC.localAI[0];

        public const int TimeBetweenCharges = 60;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/FusionFeederGlow", AssetRequestMode.AsyncLoad);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.5f,
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
            value.Position.X += 40f;
            value.Position.Y -= 6f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.width = 120;
            NPC.height = 48;
            NPC.damage = 45;
            NPC.aiStyle = -1;
            NPC.lifeMax = 400;
            NPC.defense = 12;
            NPC.DR_NERD(0.15f);
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.knockBackResist = 0.8f;
            NPC.behindTiles = true;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            AnimationType = NPCID.SandShark;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FusionFeederBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 65;
                NPC.defense = 22;
                NPC.knockBackResist = 0.7f;
                NPC.lifeMax = 600;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AstralDesert>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public static bool ValidMovementPosition(Tile tile)
        {
            return tile.HasUnactuatedTile;
        }

        public override void AI()
        {
            // Initialize direction if necessary by getting an initial target.
            if (NPC.direction == 0)
                NPC.TargetClosest(true);

            Point tileCheckPoint = NPC.Bottom.ToTileCoordinates();
            Tile checkTile = Framing.GetTileSafely(tileCheckPoint);
            bool canMoveFreely = ValidMovementPosition(checkTile);
            canMoveFreely |= NPC.wet;
            NPC.TargetClosest();

            Vector2 targetCenter = NPC.targetRect.Center.ToVector2();
            float distanceFromTarget = NPC.Distance(targetCenter);
            bool attemptingToAttackTarget = Target.velocity.Y > -0.1f && !Target.dead && distanceFromTarget > 150f;

            if (SearchSoundCreationDelay == -1f && !canMoveFreely)
                SearchSoundCreationDelay = 20f;
            if (SearchSoundCreationDelay > 0f)
                SearchSoundCreationDelay--;

            if (canMoveFreely)
            {
                if (NPC.soundDelay == 0)
                {
                    NPC.soundDelay = (int)MathHelper.Clamp(distanceFromTarget / 40f, 10f, 20f);
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }

                tileCheckPoint = (NPC.Center + Vector2.UnitY * 24f).ToTileCoordinates();
                checkTile = Framing.GetTileSafely(tileCheckPoint.X, tileCheckPoint.Y - 2);
                bool belowSand = ValidMovementPosition(checkTile);

                // Increment the charge delay.
                if (ChargeDelay < TimeBetweenCharges / 2)
                    ChargeDelay++;

                // Move towards the target and lunge at them, releasing meteors.
                if (attemptingToAttackTarget)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    NPC.TargetClosest(true);
                    NPC.velocity += new Vector2(NPC.direction, NPC.directionY) * 0.15f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -6f, 6f);
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -3f, 3f);

                    Vector2 aheadPosition = NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * NPC.Size.Length() * 2f + NPC.velocity;
                    tileCheckPoint = aheadPosition.ToTileCoordinates();
                    checkTile = Framing.GetTileSafely(tileCheckPoint);
                    bool shouldntCharge = ValidMovementPosition(checkTile);
                    if (!shouldntCharge && NPC.wet)
                        shouldntCharge = checkTile.LiquidAmount > 0;

                    if (!shouldntCharge && Math.Sign(NPC.velocity.X) == NPC.direction && distanceFromTarget < 540f && (ChargeDelay >= TimeBetweenCharges / 2 || ChargeDelay < 0f))
                    {
                        if (SearchSoundCreationDelay == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);
                            SearchSoundCreationDelay = -1f;
                        }

                        // Release a meteor at the target.
                        if (ChargeDelay > 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item73, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = DownedBossSystem.downedAstrumAureus ? (Main.masterMode ? 34 : Main.expertMode ? 40 : 55) : (Main.masterMode ? 30 : Main.expertMode ? 35 : 45);
                                Vector2 meteorShootVelocity = NPC.SafeDirectionTo(Target.Center) * 9f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + meteorShootVelocity * 6f, meteorShootVelocity, ModContent.ProjectileType<AstralMeteorProj>(), damage, 0f);
                            }
                        }

                        ChargeDelay = -TimeBetweenCharges / 2;
                        NPC.velocity = NPC.SafeDirectionTo(targetCenter - Vector2.UnitY * 80f) * 16f;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Rebound on collision.
                    if (NPC.collideX)
                    {
                        NPC.velocity.X *= -1f;
                        NPC.direction *= -1;
                        NPC.netUpdate = true;
                    }
                    if (NPC.collideY)
                    {
                        NPC.netUpdate = true;
                        NPC.velocity.Y *= -1f;
                        NPC.directionY = Math.Sign(NPC.velocity.Y);
                        VerticalMovementDirection = NPC.directionY;
                    }

                    // Accelerate in the current direction.
                    // If the speed is too high, exponentially decelerate.
                    float movementDirectionSwitchThreshold = 0.06f;
                    float horizontalSearchAcceleration = 0.1f;
                    float horizontalSearchMaxSpeed = 6f;
                    float verticalSearchAcceleration = 0.01f;
                    float verticalSearchMaxSpeed = 0.5f;
                    VerticalMovementDirection = (!belowSand).ToDirectionInt();
                    NPC.velocity.X += NPC.direction * horizontalSearchAcceleration;
                    if (Math.Abs(NPC.velocity.X) > horizontalSearchMaxSpeed)
                        NPC.velocity.X *= 0.95f;

                    if (VerticalMovementDirection == -1f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - verticalSearchAcceleration;
                        if (NPC.velocity.Y < -movementDirectionSwitchThreshold)
                            VerticalMovementDirection = 1f;
                    }
                    else
                    {
                        NPC.velocity.Y += verticalSearchAcceleration;
                        if (NPC.velocity.Y > movementDirectionSwitchThreshold)
                            VerticalMovementDirection = -1f;
                    }
                    if (Math.Abs(NPC.velocity.Y) > verticalSearchMaxSpeed)
                        NPC.velocity.Y *= 0.95f;
                }
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.velocity.Y == 0f)
                {
                    // Search for any potential closer targets if attempting to attack.
                    if (attemptingToAttackTarget)
                        NPC.TargetClosest(true);

                    // Accelerate in the current direction.
                    // If the speed is too high, exponentially decelerate.
                    NPC.velocity.X += 0.1f;
                    if (Math.Abs(NPC.velocity.X) > 1f)
                        NPC.velocity.X *= 0.95f;
                }

                // Fall downward.
                NPC.velocity.Y += 0.3f;
                if (NPC.velocity.Y > 10f)
                    NPC.velocity.Y = 10f;
                VerticalMovementDirection = 1f;
            }
            NPC.rotation = MathHelper.Clamp(NPC.velocity.Y * NPC.direction * 0.1f, -0.2f, 0.2f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FusionFeeder")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 134, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(46, 4, 60, 6), Vector2.Zero, 0.55f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hit.HitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 25);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.NextFloat(0f, NPC.width), Main.rand.NextFloat(0f, NPC.height)), NPC.velocity * rand, Mod.Find<ModGore>("FusionFeederGore" + i).Type);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 offset = new Vector2(0f, 10f);
            Vector2 origin = new Vector2(67f, 23f);
            if (NPC.IsABestiaryIconDummy)
                drawColor = Color.White;

            //draw shark
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + offset, NPC.frame, drawColor, NPC.rotation, origin, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            //draw glowmask
            spriteBatch.Draw(glowmask.Value, NPC.Center - screenPos + offset, NPC.frame, Color.White * 0.6f, NPC.rotation, origin, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(3))
            {
                return 0.14f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 75, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.SharkFin, 8);
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<StarblightSoot>(), 1, 2, 3, 3, 4));
        }
    }
}
