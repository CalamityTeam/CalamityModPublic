using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Accessories
{
	public class ShatteredCommunity : ModItem
	{
		// The percentage of a full Rage bar that is gained every second with the Shattered Community equipped.
		public const float RagePerSecond = 0.02f;
		private static readonly Color rarityColorOne = new Color(128, 62, 128);
		private static readonly Color rarityColorTwo = new Color(245, 105, 245);

		private bool everInInventory = false;

		// Base level cost is 400,000 damage dealt while Rage is active.
		// Each successive level costs (400,000 * level) MORE damage, so the total required goes up quadratically.
		// Total required to reach level 60 is 732,000,000 damage dealt.
		// This is within one order of magnitude of the integer limit, so relevant values are stored as longs.
		private const long BaseLevelCost = 400000L;
		private static long LevelCost(int level) => BaseLevelCost * level;
		private static long CumulativeLevelCost(int level) => (BaseLevelCost / 2L) * level * (level + 1);
		private const int MaxLevel = 60;

		internal int rageLevel = 0;
		internal long totalRageDamage = 0L;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shattered Community");
			Tooltip.SetDefault("Ruined by unknowable hatred, it still contains (most of) the power of The Community...\n" +
				"Legendary Accessory\n" +
				"You generate rage over time and rage does not fade away out of combat\n" +
				"Taking damage gives rage, this effect is not hindered by your defensive stats\n" +
				"While Rage Mode is active, taking damage gives only half as much rage\n" +
				"Deal damage with Rage Mode to further empower your wrath\n");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 5));
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.accessory = true;
			item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.ItemSpecific;
		}

		// Not overriding these Clones makes tooltips fail to function correctly due to HoverItem spaghetti.
		public override ModItem Clone()
		{
			var clone = (ShatteredCommunity)base.Clone();
			clone.everInInventory = everInInventory;
			clone.rageLevel = rageLevel;
			clone.totalRageDamage = totalRageDamage;
			return clone;
		}
		public override ModItem Clone(Item item)
		{
			var clone = (ShatteredCommunity)base.Clone();
			clone.everInInventory = everInInventory;
			clone.rageLevel = rageLevel;
			clone.totalRageDamage = totalRageDamage;
			return clone;
		}

		internal static Color GetRarityColor() => CalamityUtils.ColorSwap(rarityColorOne, rarityColorTwo, 3f);

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.shatteredCommunity = true;

			// Shattered Community gives (mostly) the same boosts as normal Community
			// It does not give melee speed or minion knockback, but always gives life regen (instead of only conditionally)
			player.allDamage += 0.1f;
			modPlayer.AllCritBoost(5);
			player.statDefense += 10;
			player.endurance += 0.05f;
			player.lifeRegen += 2;
			player.moveSpeed += 0.2f;

			// Shattered Community provides a stacking +1% Rage Mode damage per level.
			modPlayer.RageDamageBoost += rageLevel * 0.01;
		}

		// Community and Shattered Community are mutually exclusive
		public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().community;
		public override bool CanUseItem(Player player) => false;

		public override void UpdateInventory(Player player)
		{
			everInInventory = true;
		}

		// Produces purple light while in the world.
		public override void PostUpdate()
		{
			float brightness = Main.essScale;
			Lighting.AddLight(item.Center, 0.92f * brightness, 0.42f * brightness, 0.92f * brightness);
		}

		internal static void AccumulateRageDamage(Player player, CalamityPlayer mp, long damage)
		{
			if (!mp.shatteredCommunity)
				return;
			// Static context, so the item ID has to be retrieved from TML.
			int thisItemID = ModContent.ItemType<ShatteredCommunity>();
			ShatteredCommunity sc = null;

			// First, find the Shattered Community in the player's inventory.
			// slots 3, 4, 5, 6, 7, 8, 9 are valid non-vanity accessories
			for (int i = 3; i <= 9; ++i)
			{
				Item acc = player.armor[i];
				if (acc.type == thisItemID)
					sc = acc.modItem as ShatteredCommunity;
			}

			// Safety check in case for some reason the equipped Shattered Community isn't found.
			if (sc is null)
				return;

			// Actually accumulate the damage.
			sc.totalRageDamage += damage;

			// Level up if applicable.
			if (sc.totalRageDamage > CumulativeLevelCost(sc.rageLevel + 1))
			{
				++sc.rageLevel;
				sc.LevelUpEffects(player);
			}
		}

		private void LevelUpEffects(Player player)
		{
			// TODO -- it should probably do something fancy involving dust or whatever
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Shattered Community hides its full tooltip when viewed via Recipe Browser or similar.
			// Instead, it notifies the player that it must be crafted with a full-power Community.
			// Once it has entered a player's inventory, it permanently behaves normally.
			if (!everInInventory)
			{
				tooltips.RemoveAll(line => line.Name != "Tooltip0" && line.Name != "Tooltip1" && line.Name != "Tooltip2" && line.Name.StartsWith("Tooltip"));
				tooltips.Find(line => line.Name == "Tooltip2").text = "Must be crafted with a fully powered Community";
				return;
			}

			// Stat tooltips are added dynamically.
			StringBuilder sb = new StringBuilder(256);

			// Line 6: Current level
			sb.Append("Current level: ");
			sb.Append(rageLevel);
			sb.Append(" (+");
			sb.Append(rageLevel);
			sb.Append("% Rage Mode damage)");
			tooltips.Add(new TooltipLine(mod, "Tooltip6", sb.ToString()));
			sb.Clear();

			if (rageLevel < MaxLevel)
			{
				long progressToNextLevel = totalRageDamage - CumulativeLevelCost(rageLevel);
				long totalToNextLevel = LevelCost(rageLevel + 1);
				double ratio = (double)progressToNextLevel / totalToNextLevel;
				string percent = (100D * ratio).ToString("0.00");

				// Line 7: Progress to next level
				sb.Append("Progress to next level: ");
				sb.Append(percent);
				sb.Append('%');
				tooltips.Add(new TooltipLine(mod, "Tooltip7", sb.ToString()));
				sb.Clear();
			}

			// Line 8: Total damage dealt
			sb.Append("Total Rage Mode damage: ");
			sb.Append(totalRageDamage);
			tooltips.Add(new TooltipLine(mod, "Tooltip8", sb.ToString()));
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound
			{
				{ "real", everInInventory },
				{ "level", rageLevel },
				{ "totalDamage", totalRageDamage }
			};
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			everInInventory = tag.GetBool("real");
			rageLevel = tag.GetInt("level");
			totalRageDamage = tag.GetLong("totalDamage");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(everInInventory);
			writer.Write(rageLevel);
			writer.Write(totalRageDamage);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			everInInventory = reader.ReadBoolean();
			rageLevel = reader.ReadInt32();
			totalRageDamage = reader.ReadInt64();
		}

		public override void AddRecipes()
		{
			ModRecipe r = new ShatteredCommunityRecipe(mod);
			r.SetResult(this);
			r.AddIngredient(ModContent.ItemType<TheCommunity>());
			r.AddIngredient(ModContent.ItemType<HeartofDarkness>());
			r.AddIngredient(ModContent.ItemType<CalamitousEssence>(), 120);
			r.AddRecipe();
		}

		// The Shattered Community can only be crafted with a full-power Community.
		// That is, in a world where all relevant bosses have been defeated.
		private class ShatteredCommunityRecipe : ModRecipe
		{
			public ShatteredCommunityRecipe(Mod mod) : base(mod) { }

			public override bool RecipeAvailable()
			{
				bool vanillaBosses = NPC.downedSlimeKing && NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedQueenBee && NPC.downedBoss3 && Main.hardMode;
				vanillaBosses &= NPC.downedMechBossAny && NPC.downedPlantBoss && NPC.downedGolemBoss && NPC.downedFishron && NPC.downedAncientCultist && NPC.downedMoonlord;
				bool calamityBosses = CalamityWorld.downedProvidence && CalamityWorld.downedDoG && CalamityWorld.downedYharon;
				return vanillaBosses && calamityBosses;
			}
		}
	}
}
