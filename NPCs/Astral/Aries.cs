using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.Astral
{
    public class Aries : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aries");
            Main.npcFrameCount[NPC.type] = 8;
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/AriesGlow", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            NPC.damage = 50;
            NPC.width = 66;
            NPC.height = 64;
            NPC.aiStyle = 41;
            NPC.defense = 14;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0.6f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.DeathSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCKilled/AstralEnemyDeath");
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AriesBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 85;
                NPC.defense = 24;
                NPC.knockBackResist = 0.5f;
                NPC.lifeMax = 450;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AstralSurface,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Biologically, the creature suffers terribly. Its legs do not allow it to rest, constantly hopping across the infected landscape. It does not care as its mind is far gone. It continues this until death.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC.SpawnDustOnNPC(NPC, 66, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(44, 18, 12, 12));
            if (NPC.velocity.Y == 0f)
            {
                NPC.frame.Y = 0;
            }
            else if ((double)NPC.velocity.Y < -1.5)
            {
                NPC.frame.Y = frameHeight * 7;
            }
            else if ((double)NPC.velocity.Y < 0)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else if ((double)NPC.velocity.Y > 1.5)
            {
                NPC.frame.Y = frameHeight * 6;
            }
            else
            {
                NPC.frame.Y = frameHeight * 5;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, ModContent.DustType<AstralOrange>(), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, NPC.Center - screenPos, NPC.frame, Color.White * 0.6f, NPC.rotation, new Vector2(33, 31), 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return 0.15f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Stardust>(), 2, 1, 2, 1, 3));
            npcLoot.AddIf(() => DownedBossSystem.downedAstrumAureus, ModContent.ItemType<StellarKnife>(), 7);
        }
    }
}
