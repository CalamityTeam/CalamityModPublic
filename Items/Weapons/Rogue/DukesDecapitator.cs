using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class DukesDecapitator : RogueWeapon
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Duke's Decapitator");
			Tooltip.SetDefault("Throws a hydro axe which shreds enemies when it comes into contact with them\n"
							  +"The faster it’s spinning, the more times it hits before disappearing\n"
							  +"Stealth Strikes make it emit short-ranged bubbles.");
		}

		public override void SafeSetDefaults()
		{
			item.width = 60;
			item.height = 64;
			item.damage = 90;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.autoReuse = true;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTime = 30;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<DukesDecapitatorProj>();
			item.shootSpeed = 15f;
            item.Calamity().rogue = true;
		}
	}
}
