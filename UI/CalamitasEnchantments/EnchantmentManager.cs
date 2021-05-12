using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CalamityMod.UI.CalamitasEnchants
{
	public static class EnchantmentManager
	{
		internal const int ClearEnchantmentID = -18591774;
		public static List<Enchantment> EnchantmentList { get; internal set; } = new List<Enchantment>();
		public static Enchantment ClearEnchantment { get; internal set; }
		public static IEnumerable<Enchantment> GetValidEnchantmentsForItem(Item item)
		{
			// Do nothing if the item cannot be enchanted.
			if (item is null || item.IsAir || !item.CanBeEnchantedBySomething())
				yield break;

			// Only give the option to clear if the item already has an enchantment.
			if (item.Calamity().AppliedEnchantment.HasValue)
			{
				yield return ClearEnchantment;
				yield break;
			}

			// Check based on all the requirements for all enchantments.
			foreach (Enchantment enchantment in EnchantmentList)
			{
				// Don't incorporate an enchantment in the list if the item already has it.
				if (item.Calamity().AppliedEnchantment.HasValue && item.Calamity().AppliedEnchantment.Value.Equals(enchantment))
					continue;

				if (enchantment.ApplyRequirement(item))
					yield return enchantment;
			}
		}

		public static Enchantment? FindByID(int id)
		{
			Enchantment? enchantment = EnchantmentList.FirstOrDefault(enchant => enchant.ID == id);
			if (enchantment.HasValue && !enchantment.Value.Equals(default(Enchantment)))
				return enchantment;
			return null;
		}

		public static void LoadAllEnchantments()
		{
			EnchantmentList = new List<Enchantment>
			{
				new Enchantment("Cursed", "Summons demons that harm you but drop healing items on death on item usage.", 
					100,
					item => item.damage = (int)(item.damage * 1.1),
					player => player.Calamity().cursedSummonsEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.summon),

				new Enchantment("Aflame", "Lights enemies ablaze on hit but also causes the user to take damage over time when holding this item.",
					200,
					null,
					player => player.Calamity().flamingItemEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && !item.summon),
			};

			// Special disenchantment thing. This is separated from the list on purpose.
			ClearEnchantment = new Enchantment("Disenchant", string.Empty, ClearEnchantmentID,
				item => item.Calamity().AppliedEnchantment = null,
				item => item.maxStack == 1);
		}

		public static void UnloadAllEnchantments() => EnchantmentList = null;
	}
}
