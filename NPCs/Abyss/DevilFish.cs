using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
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
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
namespace CalamityMod.NPCs.Abyss
{
    public class DevilFish : ModNPC
    {
        public static readonly SoundStyle MaskBreakSound = new("CalamityMod/Sounds/Custom/DevilMaskBreak");

        public bool brokenMask = false;
        public int hitCounter = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionXOverride = 5f
            };
            value.Position.X += 30f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90;
            NPC.width = 136;
            NPC.height = 62;
            NPC.defense = 999999;
            NPC.lifeMax = 400;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.85f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<DevilFishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer3Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DevilFish")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(brokenMask);
            writer.Write(hitCounter);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            brokenMask = reader.ReadBoolean();
            hitCounter = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            float speedBoost = brokenMask ? 1.5f : 1;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            NPC.noGravity = true;
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            NPC.chaseable = brokenMask;

            if (hitCounter >= 5 && !brokenMask)
            {
                brokenMask = true;
                NPC.HitSound = SoundID.NPCHit1;
                NPC.defense = 15;
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DevilFishMask" + i).Type, 1f);
                    }
                }
                SoundEngine.PlaySound(MaskBreakSound, NPC.Center);
            }

            if (NPC.wet)
            {
                bool canAttack = brokenMask;
                NPC.TargetClosest(false);
                if (player.statLife <= player.statLifeMax2 / 4)
                {
                    canAttack = true;
                }
                if ((!player.wet || player.dead || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height)) && canAttack)
                {
                    canAttack = false;
                }
                if (!canAttack)
                {
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
                    NPC.TargetClosest(true);
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * (CalamityWorld.death ? 0.5f : CalamityWorld.revenge ? 0.375f : 0.25f) * speedBoost;
                    NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * (CalamityWorld.death ? 0.3f : CalamityWorld.revenge ? 0.225f : 0.15f) * speedBoost;
                    float velocity = CalamityWorld.death ? 12f : CalamityWorld.revenge ? 9f : 6f;
                    if (NPC.velocity.X > velocity * speedBoost)
                    {
                        NPC.velocity.X = velocity * speedBoost;
                    }
                    if (NPC.velocity.X < -velocity * speedBoost)
                    {
                        NPC.velocity.X = -velocity * speedBoost;
                    }
                    if (NPC.velocity.Y > velocity * speedBoost)
                    {
                        NPC.velocity.Y = velocity * speedBoost;
                    }
                    if (NPC.velocity.Y < -velocity * speedBoost)
                    {
                        NPC.velocity.Y = -velocity * speedBoost;
                    }
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.1f;
                    if (NPC.velocity.X < -2.5f || NPC.velocity.X > 2.5f)
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
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.94f;
                    if ((double)NPC.velocity.X > -0.2 && (double)NPC.velocity.X < 0.2)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
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
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return brokenMask;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet && !NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
            }
            if (!brokenMask || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y > frameHeight * 7)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 8 || NPC.frame.Y > frameHeight * 15)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/DevilFishGlow").Value;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 180, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water)
            {
                return Main.remixWorld ? 4.95f : SpawnCondition.CaveJellyfish.Chance * 0.55f;
            }
            return 0f;
        }

        public static void DefineDevilFishLoot(NPCLoot npcLoot)
        {
            var postClone = npcLoot.DefineConditionalDropSet(DropHelper.PostCal());
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 1, 2, 2, 3));
            postClone.Add(ModContent.ItemType<Lumenyl>(), 2);
            npcLoot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<ScoriaOre>(), 1, 3, 9);
            npcLoot.Add(ModContent.ItemType<PyreMantle>(), 1, 10, 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineDevilFishLoot(npcLoot);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Devilfish").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Devilfish2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Devilfish3").Type, 1f);
                }
            }

            if (hitCounter <= 5)
            {
                ++hitCounter;
            }
        }
    }
}
