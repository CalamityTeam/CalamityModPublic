using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI.CalamitasEnchants
{
	public static class EnchantmentManager
	{
		internal const int ClearEnchantmentID = -18591774;
		internal const string UpgradeEnchantName = "Exhume";
		public static List<Enchantment> EnchantmentList { get; internal set; } = new List<Enchantment>();
		public static Dictionary<int, int> ItemUpgradeRelationship { get; internal set; } = new Dictionary<int, int>();
		public static Enchantment ClearEnchantment { get; internal set; }
		public static IEnumerable<Enchantment> GetValidEnchantmentsForItem(Item item)
		{
			// Do nothing if the item cannot be enchanted.
			if (item is null || item.IsAir || !item.CanBeEnchantedBySomething())
				yield break;

			// Don't allow any enchantments if the item is an upgrade item.
			if (ItemUpgradeRelationship.ContainsValue(item.type))
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

		public static void ConstructFromModcall(IEnumerable<object> parameters)
		{
			int secondaryArgumentCount = parameters.Count();
			if (secondaryArgumentCount < 4)
				throw new ArgumentNullException("ERROR: A minimum of 4 arguments must be supplied to this command; a name, a description, an id, and a requirement predicate.");
			if (secondaryArgumentCount > 6)
				throw new ArgumentNullException("ERROR: A maximum of 6 arguments can be supplied to this command.");

			string name = string.Empty;
			string description = string.Empty;
			int id = -1;
			Predicate<Item> requirement = null;
			Action<Item> creationEffect = null;
			Action<Player> holdEffect = null;

			// First element - the name.
			if (parameters.ElementAt(0) is string nameElement)
				name = nameElement;
			else
				throw new ArgumentException("The first argument to this command must be a string.");

			// Second element - the description.
			if (parameters.ElementAt(1) is string descriptionElement)
				description = descriptionElement;
			else
				throw new ArgumentException("The second argument to this command must be a string.");

			// Third element - the ID.
			if (parameters.ElementAt(2) is int idElement)
				id = idElement;
			else
				throw new ArgumentException("The third argument to this command must be an int.");

			// Fourth element - the requirement predicate. This determines if an item can be enchanted by said enchant or not.
			if (parameters.ElementAt(3) is Predicate<Item> requirementElement)
				requirement = requirementElement;
			else
				throw new ArgumentException("The fourth argument to this command must be an Item Predicate.");

			// Optional elements - creation and hold effects.
			switch (secondaryArgumentCount)
			{
				case 5:
					object fifthElement = parameters.ElementAt(4);
					if (fifthElement is Action<Item> creationElement)
						creationEffect = creationElement;
					else if (fifthElement is Action<Player> holdElement)
						holdEffect = holdElement;
					else
						throw new ArgumentException("The fifth argument to this command must be an Item or Player Action.");
					break;
				case 6:
					fifthElement = parameters.ElementAt(4);
					object sixthElement = parameters.ElementAt(5);
					if (fifthElement is Action<Item> creationElement2)
					{
						creationEffect = creationElement2;
						holdEffect = sixthElement as Action<Player>;
					}
					else if (fifthElement is Action<Player> holdElement2)
					{
						creationEffect = sixthElement as Action<Item>;
						holdEffect = holdElement2;
					}
					else
						throw new ArgumentException("The fifth argument to this command must be an Item or Player Action and the sixth must be the other action type.");
					break;
			}

			// Ensure the enchantment's ID is not already claimed.
			if (EnchantmentList.Any(enchant => enchant.ID == id) || id == ClearEnchantmentID)
				throw new ArgumentException("An enchantment with this ID already exists. Another one must be specified.");

			EnchantmentList.Add(new Enchantment(name, description, id, creationEffect, holdEffect, requirement));
		}

		internal static void LoadAllEnchantments()
		{
			EnchantmentList = new List<Enchantment>
			{
				new Enchantment(UpgradeEnchantName, "Transforms this item into something significantly better.",
					1,
					null,
					null,
					item => ItemUpgradeRelationship.ContainsKey(item.type)),

				new Enchantment("Cursed", "Summons demons that harm you but drop healing items on death on item usage.",
					100,
					null,
					player => player.Calamity().cursedSummonsEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.summon),

				new Enchantment("Aflame", "Lights enemies ablaze on hit but also causes the user to take damage over time when holding this item.",
					200,
					null,
					player => player.Calamity().flamingItemEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && !item.summon),

				new Enchantment("Oblatory", "Reduces mana cost and greatly increases damage but sometimes causes this item to use your life.",
					300,
					item =>
					{
						item.damage = (int)(item.damage * 1.5);
						item.mana = (int)Math.Ceiling(item.mana * 0.6);
					},
					player => player.Calamity().lifeManaEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.magic && item.mana > 0),

				new Enchantment("Resentful", "Makes the damage of projectiles vary based on how far the hit target is from you. The farther, the more damage, and vice versa.",
					400,
					null,
					player => player.Calamity().farProximityRewardEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.shoot > ProjectileID.None),

				new Enchantment("Bloodthirsty", "Makes the damage of projectiles vary based on how far the hit target is from you. The closer, the more damage, and vice versa.",
					500,
					null,
					player => player.Calamity().closeProximityRewardEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.shoot > ProjectileID.None),

				new Enchantment("Ephemeral", "Causes the damage output of this item to discharge from exhaustive use. Its damage returns naturally when not being used.",
					600,
					null,
					player => player.Calamity().dischargingItemEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && !item.summon),

				new Enchantment("Hellbound", "Causes minions to be created with a 40 second timer. Once it runs out, they explode violently. Minions do more damage the longer they live and idly explode as well.",
					700,
					null,
					player => player.Calamity().explosiveMinionsEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.summon),

				new Enchantment("Tainted", "Removes projectile shooting capabilities of this item. In exchange, two skeletal arms are released on use that slice at the mouse position.",
					800,
					item => item.useTime = item.useAnimation = 25,
					player =>
					{
						player.Calamity().bladeArmEnchant = true;
						bool armsArePresent = false;
						int armType = ModContent.ProjectileType<TaintedBladeSlasher>();
						for (int i = 0; i < Main.maxProjectiles; i++)
						{
							if (Main.projectile[i].type != armType || Main.projectile[i].owner != player.whoAmI || !Main.projectile[i].active)
								continue;

							armsArePresent = true;
							break;
						}

						if (Main.myPlayer == player.whoAmI && !armsArePresent)
						{
							// Yes, this is a LOT of damage but given the limited range of this thing it needs to be extremely powerful when it does actually hit.
							int damage = (int)(player.ActiveItem().damage * player.MeleeDamage() * 5);
							int blade = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TaintedBladeSlasher>(), damage, 0f, player.whoAmI, 0f, player.ActiveItem().type);
							if (Main.projectile.IndexInRange(blade))
								Main.projectile[blade].localAI[0] = 0f;
							
							blade = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TaintedBladeSlasher>(), damage, 0f, player.whoAmI, 1f, player.ActiveItem().type);
							if (Main.projectile.IndexInRange(blade))
								Main.projectile[blade].localAI[0] = -80f;
						}
					},
					item => item.damage > 0 && item.maxStack == 1 && item.melee && !item.noUseGraphic && item.shoot > ProjectileID.None),

				new Enchantment("Treacherous", "Causes this item to sometimes release a monster that hurts both you and enemies when you have less than 50% mana.",
					900,
					null,
					player => player.Calamity().manaMonsterEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && item.magic && item.mana > 0),

				new Enchantment("Withering", "You heal when hurt based on damage dealt. After being hurt, the weapon rapidly drains your life to deal massive damage.",
					1000,
					null,
					player => player.Calamity().witheringWeaponEnchant = true,
					item => item.damage > 0 && item.maxStack == 1 && !item.summon),
			};

			// Special disenchantment thing. This is separated from the list on purpose.
			ClearEnchantment = new Enchantment("Disenchant", string.Empty, ClearEnchantmentID,
				item => item.Calamity().AppliedEnchantment = null,
				item => item.maxStack == 1 && item.shoot >= ProjectileID.None);

			ItemUpgradeRelationship = new Dictionary<int, int>()
			{
				[ModContent.ItemType<TheCommunity>()] = ModContent.ItemType<ShatteredCommunity>(),
				[ModContent.ItemType<BlightedEyeStaff>()] = ModContent.ItemType<ExhumedVigilance>(),
			};
		}

		internal static void UnloadAllEnchantments()
		{
			EnchantmentList = null;
			ItemUpgradeRelationship = null;
		}
	}
}
