using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod.Sounds;
using Terraria.Graphics.Effects;
using CalamityMod.World;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumRover : ModNPC
    {
        public float TimeSpentStuck
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float SuperchargeTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 90f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -4f;
        public bool Supercharged => SuperchargeTimer > 0;

        //GetFuckedBoi
        public int mineDelay = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            AIType = -1;
            NPC.aiStyle = -1;
            NPC.damage = 10;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 4;
            NPC.lifeMax = 32;
            NPC.knockBackResist = Main.zenithWorld ? 0f : 0.15f;
            NPC.value = Item.buyPrice(0, 0, 1, 15);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WulfrumRoverBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            if (Main.zenithWorld)
                NPC.scale = 2f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WulfrumRover")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            int frame = (int)(NPC.frameCounter / 5) % (Main.npcFrameCount[NPC.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[NPC.type] / 2;

            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            if (Supercharged)
            {
                SuperchargeTimer--;
                NPC.defense = CalamityWorld.LegendaryMode ? 20 : 13;
            }
            else if (!Supercharged)
            {
                NPC.defense = CalamityWorld.LegendaryMode ? 10 : 4;
            }

            Player player = Main.player[NPC.target];
            if (Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) &&
                Math.Abs(player.Center.X - NPC.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - NPC.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - NPC.Center.X) * (NPC.confused ? -1 : 1);
                if (NPC.direction != direction)
                {
                    NPC.direction = direction;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.collideX)
            {
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }

            if (NPC.oldPosition == NPC.position)
            {
                TimeSpentStuck++;
                if (Main.netMode != NetmodeID.MultiplayerClient && TimeSpentStuck > StuckJumpPromptTime)
                {
                    NPC.velocity.Y = JumpSpeed;
                    TimeSpentStuck = 0f;
                    NPC.netUpdate = true;
                }
            }
            else
                TimeSpentStuck = 0f;

            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MaxMovementSpeedX * NPC.direction, 0.0125f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur || (!spawnInfo.Player.ZoneOverworldHeight && !Main.remixWorld) || (!spawnInfo.Player.ZoneNormalCaverns && spawnInfo.Player.ZoneGlowshroom && Main.remixWorld))
                return 0f;

            return (Main.remixWorld ? SpawnCondition.Cavern.Chance : SpawnCondition.OverworldDaySlime.Chance) * (Main.hardMode ? 0.010f : 0.135f) * (NPC.AnyNPCs(ModContent.NPCType<WulfrumAmplifier>()) ? 5.5f : 1f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 4; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (NPC.life <= 0)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hit.HitDirection, -1f, 0, default, 1f);
                    }

                    if (!Main.dedServ)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumRoverGore1").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumRoverGore2").Type, 1f);

                        int randomGoreCount = Main.rand.Next(1, 4);
                        for (int i = 0; i < randomGoreCount; i++)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString()).Type, 1f);
                        }
                    }
                }
                if (Main.zenithWorld && mineDelay == 0)
                {
                    Vector2 roverBase = new Vector2(NPC.Center.X,NPC.Center.Y+5f);
                    int mine = Projectile.NewProjectile(NPC.GetSource_FromAI(), roverBase, Vector2.Zero, ProjectileID.ProximityMineI, 50, 0f);
                    if (mine.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[mine].friendly = false;
                        Main.projectile[mine].hostile = true;
                        Main.projectile[mine].timeLeft = 60;
                    }
                    mineDelay = CalamityWorld.LegendaryMode ? 3 : 5;
                }
                else if (Main.zenithWorld && mineDelay >= 1)
                    mineDelay--;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Supercharged)
            {
                //old
                /*
                Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/NormalNPCs/WulfrumRoverShield").Value;
                Rectangle frame = shieldTexture.Frame(1, 11, 0, (int)(Main.GlobalTimeWrappedHourly * 8) % 11);
                spriteBatch.Draw(shieldTexture,
                                 NPC.Center - screenPos + Vector2.UnitY * (NPC.gfxOffY + 6f),
                                 frame,
                                 Color.White * 0.625f,
                                 NPC.rotation,
                                 shieldTexture.Size() * 0.5f / new Vector2(1f, 11f),
                                 NPC.scale + (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 0.1f,
                                 SpriteEffects.None,
                                 0f);
                */

                //0.6 : The noise upscale
                //0.15 the scale its drawn at usually, 0.2 on Zenith
                float scale = (Main.zenithWorld ? 0.2f : 0.15f) + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + NPC.whoAmI * 0.2f));
                float noiseScale = 0.6f;

                Effect shieldEffect = Terraria.Graphics.Effects.Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity);
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color blueTint = new Color(51, 102, 255);
                Color cyanTint = new Color(71, 202, 255);
                Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                shieldEffect.Parameters["shieldColor"].SetValue(blueTint.ToVector3());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                if (RoverDrive.NoiseTex == null)
                    RoverDrive.NoiseTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise");

                Texture2D tex = RoverDrive.NoiseTex.Value;
                Vector2 pos = NPC.Center + NPC.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 1, 1, 2);
            npcLoot.Add(ModContent.ItemType<RoverDrive>(), 10);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.AddIf(info => info.npc.ModNPC<WulfrumRover>().Supercharged, ModContent.ItemType<EnergyCore>());
        }
    }
}
