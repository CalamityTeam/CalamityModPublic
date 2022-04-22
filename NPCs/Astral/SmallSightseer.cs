using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;

namespace CalamityMod.NPCs.Astral
{
    public class SmallSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Sightseer");
            Main.npcFrameCount[NPC.type] = 4;

            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/SmallSightseerGlow", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 40;
            NPC.damage = 38;
            NPC.defense = 16;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 310;
            NPC.DeathSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCKilled/AstralEnemyDeath");
            NPC.noGravity = true;
            NPC.knockBackResist = 0.58f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SmallSightseerBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 58;
                NPC.defense = 26;
                NPC.knockBackResist = 0.48f;
                NPC.lifeMax = 460;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AstralSurface,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Within their shells, the virus brews a potent chemical. When the seer approaches a foe, it spits this chemical out of its mandibles, hoping to melt the intruder’s flesh.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.05f + NPC.velocity.Length() * 0.667f;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > NPC.height * 2)
                {
                    NPC.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 80, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(NPC, (CalamityWorld.death ? 8.7f : 5.8f), (CalamityWorld.death ? 0.045f : 0.03f), 350f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.NextFloat(0f, NPC.width), Main.rand.NextFloat(0f, NPC.height)), NPC.velocity * rand, Mod.Find<ModGore>("SmallSightseerGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(glowmask, NPC.Center - screenPos + new Vector2(0, 4f), new Rectangle(0, NPC.frame.Y, 80, NPC.frame.Height), Color.White * 0.75f, NPC.rotation, new Vector2(40f, 20f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return spawnInfo.Player.ZoneDesert ? 0.16f : 0.2f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Stardust>(), 1, 1, 2, 1, 3));
        }
    }
}
