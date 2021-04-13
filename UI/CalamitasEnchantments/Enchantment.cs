using System;
using Terraria;

namespace CalamityMod.UI.CalamitasEnchants
{
	public struct Enchantment
	{
		public string Name;
		public string Description;
		public Action<Item> CreationEffect;
		public Action<Player> HoldEffect;
		public Predicate<Item> ApplyRequirement;
		public Enchantment(string name, string description, Action<Item> creationEffect, Action<Player> holdEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = creationEffect;
			HoldEffect = holdEffect;
			ApplyRequirement = requirement;
		}
		public Enchantment(string name, string description, Action<Player> holdEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = null;
			HoldEffect = holdEffect;
			ApplyRequirement = requirement;
		}
		public Enchantment(string name, string description, Action<Item> creationEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = creationEffect;
			HoldEffect = null;
			ApplyRequirement = requirement;
		}
	}
}
