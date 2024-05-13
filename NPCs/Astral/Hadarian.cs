using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Sounds;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.World;
using CalamityMod.BiomeManagers.BestiaryCategories;
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
    public class Hadarian : ModNPC
    {
        public static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/HadarianGlow", AssetRequestMode.AsyncLoad);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers();
            value.Position.X += 10f;
            value.Position.Y += 10f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 50;
            NPC.defense = 8;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 330;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 15, 0);
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HadarianBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 80;
                NPC.defense = 18;
                NPC.knockBackResist = 0.65f;
                NPC.lifeMax = 500;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AstralDesert>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Hadarian")
            });
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoVultureAI(NPC, (CalamityWorld.death ? 0.25f : CalamityWorld.revenge ? 0.2f : 0.15f), (CalamityWorld.death ? 5.5f : CalamityWorld.revenge ? 4.5f : 3.5f), 32, 50, 150, 150);

            //usually done in framing but I put it here because it makes more sense to.
            NPC.rotation = NPC.velocity.X * 0.1f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0f)
            {
                NPC.spriteDirection = NPC.direction;
            }
            else
            {
                if ((double)NPC.velocity.X > 0.5)
                {
                    NPC.spriteDirection = 1;
                }
                if ((double)NPC.velocity.X < -0.5)
                {
                    NPC.spriteDirection = -1;
                }
            }

            if ((NPC.velocity.X == 0f && NPC.velocity.Y == 0f) && !NPC.IsABestiaryIconDummy)
            {
                NPC.frame.Y = 0;
                NPC.frameCounter = 0.0;
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y > frameHeight * 6 || NPC.frame.Y == 0)
                {
                    NPC.frame.Y = frameHeight;
                }
            }

            DoWingDust(frameHeight);
        }

        private void DoWingDust(int frameHeight)
        {
            int frame = NPC.frame.Y / frameHeight;
            Dust d = null;
            switch (frame)
            {
                case 1:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(38, 16, 22, 20), Vector2.Zero, 0.35f);
                    break;
                case 2:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(38, 24, 30, 14), Vector2.Zero);
                    break;
                case 3:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(44, 28, 32, 20), Vector2.Zero);
                    break;
                case 4:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(42, 36, 18, 30), Vector2.Zero, 0.3f);
                    break;
            }

            if (d != null)
            {
                d.customData = 0.03f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            if (NPC.ai[0] == 0f)
            {
                Vector2 position = NPC.Bottom - new Vector2(19f, 42f);
                //20 34 38 42
                Rectangle src = new Rectangle(20, 34, 38, 42);
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, position - screenPos, src, drawColor, NPC.rotation, default, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                //draw glowmask
                spriteBatch.Draw(glowmask.Value, position - screenPos, src, Color.White * 0.6f, NPC.rotation, default, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                return false;
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] != 0f)
            {
                Vector2 origin = new Vector2(41f, 39f);

                //draw glowmask
                spriteBatch.Draw(glowmask.Value, NPC.Center - screenPos - new Vector2(0f, 12f), NPC.frame, Color.White * 0.6f, NPC.rotation, origin, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hit.HitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity * 0.3f, Mod.Find<ModGore>("HadarianGore" + i).Type);
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(3) && spawnInfo.SpawnTileType == ModContent.TileType<AstralSand>() && tile.WallType == WallID.None)
            {
                return 0.25f;
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
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<StarblightSoot>(), 1, 1, 3, 1, 4));
            npcLoot.AddIf(() => DownedBossSystem.downedAstrumAureus, ModContent.ItemType<HadarianWings>(), 10);
        }
    }
}
