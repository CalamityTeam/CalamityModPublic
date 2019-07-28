using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage.RareVariants
{
    public class TheReaper : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Reaper");
			Tooltip.SetDefault("Slice 'n dice");
		}

		public override void SafeSetDefaults()
		{
			item.width = 80;
			item.damage = 325;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 22;
			item.useTime = 22;
			item.useStyle = 1;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Valediction2");
			item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}
	}
}
