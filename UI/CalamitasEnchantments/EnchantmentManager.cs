using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CalamityMod.UI.CalamitasEnchants
{
	public static class EnchantmentManager
	{
		public static List<Enchantment> EnchantmentList { get; internal set; } = new List<Enchantment>();
		public static Enchantment ClearEnchantment { get; internal set; }
		public static IEnumerable<Enchantment> GetValidEnchantmentsForItem(Item item)
		{
			// Do nothing if the item cannot be enchanted.
			if (item.IsAir || item.damage <= 0)
				yield break;

			// Only give the option to clear if the item already has an enchantment.
			if (item.Calamity().AppliedEnchantment.HasValue)
			{
				yield return ClearEnchantment;
				yield break;
			}

			// If an enchanted has already been selected, only show it to make space for the description.
			// It can be unselected to reveal the others again.
			if (CalamitasEnchantUI.SelectedEnchantment.HasValue)
			{
				yield return CalamitasEnchantUI.SelectedEnchantment.Value;
				yield break;
			}

			// Otherwise check based on all the requirements for all enchantments.
			foreach (Enchantment enchantment in EnchantmentList)
			{
				// Don't incorporate an enchantment in the list if the item already has it.
				if (item.Calamity().AppliedEnchantment.HasValue && item.Calamity().AppliedEnchantment.Value.Equals(enchantment))
					continue;

				if (enchantment.ApplyRequirement(item))
					yield return enchantment;
			}
		}

		public static Enchantment? FindByName(string name, StringComparison comparisonType)
		{
			Enchantment? enchantment = EnchantmentList.FirstOrDefault(enchant => enchant.Name.Equals(name, comparisonType));
			if (enchantment.HasValue && !enchantment.Value.Equals(default(Enchantment)))
				return enchantment;
			return null;
		}

		public static void LoadAllEnchantments()
		{
			EnchantmentList.Add(new Enchantment("Self-Sacrificial", "Does 10% more damage at the cost of 20% of your max life",
				item => item.damage = (int)(item.damage * 1.1), 
				player => player.Calamity().sacrificeEnchant = true, 
				item => item.damage > 0));

			EnchantmentList.Add(new Enchantment("Self-Sacrificial", "Does 10% more damage at the cost of 20% of your max life",
				item => item.damage = (int)(item.damage * 1.1),
				player => player.Calamity().sacrificeEnchant = true,
				item => item.damage > 0));

			// Special disenchantment thing. This is separated from the list on purpose.
			ClearEnchantment = new Enchantment("Disenchant", string.Empty,
				item => item.Calamity().AppliedEnchantment = null,
				item => item.damage > 0);
		}

		public static void UnloadAllEnchantments() => EnchantmentList = null;
	}
}
