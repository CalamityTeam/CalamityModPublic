using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class StellarKnife : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stellar Knife");
			Tooltip.SetDefault("Throws knives that stop middair and then home into enemies\n" +
							   "Za Warudo");
		}

		public override void SafeSetDefaults()
		{
			item.width = 32;
			item.height = 34;
			item.damage = 60;
			item.crit += 4;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 8;
			item.useStyle = 1;
			item.useTime = 8;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.rare = 7;
			item.shoot = mod.ProjectileType("StellarKnife");
			item.shootSpeed = 10f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
	}
}
