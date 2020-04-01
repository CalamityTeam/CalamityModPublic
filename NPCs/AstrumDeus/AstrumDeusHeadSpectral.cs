using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    public class AstrumDeusHeadSpectral : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.damage = 120;
            npc.npcSlots = 5f;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 25;
			npc.LifeMaxNERB(37500, 53800, 1300000);
			double HPBoost = (double)CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.scale = 1.2f;
            if (Main.expertMode)
            {
                npc.scale = 1.35f;
            }
            npc.boss = true;
            npc.value = Item.buyPrice(0, 20, 0, 0);
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
            else
                music = MusicID.Boss3;
            bossBag = ModContent.ItemType<AstrumDeusBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
			CalamityAI.AstrumDeusAI(npc, mod, true, true);
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow2");
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));
			Color color36 = npc.Calamity().newAI[0] == 1f ? new Color(250, 150, Main.DiscoB, npc.alpha) : Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			for (int num155 = 1; num155 < num153; num155 += 2)
			{
				Color color38 = lightColor;
				color38 = Color.Lerp(color38, color36, amount9);
				color38 = npc.GetAlpha(color38);
				color38 *= (float)(num153 - num155) / 15f;
				Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
				vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
				vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
				spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.Calamity().newAI[0] == 1f ? new Color(250, 150, Main.DiscoB, npc.alpha) : npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
			Color color42 = Color.Lerp(Color.White, Color.Orange, 0.5f);

			for (int num163 = 1; num163 < num153; num163++)
			{
				Color color41 = color37;
				color41 = Color.Lerp(color41, color36, amount9);
				color41 *= (float)(num153 - num163) / 15f;
				Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
				vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
				vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
				spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				Color color43 = color42;
				color43 = Color.Lerp(color43, color36, amount9);
				color43 *= (float)(num153 - num163) / 15f;
				spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (npc.Calamity().newAI[0] == 1f && Main.player[npc.target].gravDir != -1f)
            {
                return false;
            }
            return true;
        }

		public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<Stardust>();
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<AstrumDeusHeadSpectral>(),
                ModContent.NPCType<AstrumDeusBodySpectral>(),
                ModContent.NPCType<AstrumDeusTailSpectral>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<AstrumDeusTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAstrumDeus>(), !CalamityWorld.downedStarGod);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAstralInfection>(), !CalamityWorld.downedStarGod);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedStarGod, 4, 2, 1);

            // Drop a large spray of all 4 lunar fragments
            int minFragments = Main.expertMode ? 20 : 12;
            int maxFragments = Main.expertMode ? 32 : 20;
            DropHelper.DropItemSpray(npc, ItemID.FragmentSolar, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentVortex, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentNebula, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentStardust, minFragments, maxFragments);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                DropHelper.DropItemSpray(npc, ModContent.ItemType<Stardust>(), 50, 80, 5);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheMicrowave>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<StarSputter>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Starfall>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GodspawnHelixStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RegulusRiot>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Quasar>(), DropHelper.RareVariantDropRateInt);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<HideofAstrumDeus>(), DropHelper.RareVariantDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<AstrumDeusMask>(), 7);
            }

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!CalamityWorld.downedStarGod)
            {
                string key = "Mods.CalamityMod.AstralBossText";
                Color messageColor = Color.Gold;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Mark Astrum Deus as dead
            CalamityWorld.downedStarGod = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
