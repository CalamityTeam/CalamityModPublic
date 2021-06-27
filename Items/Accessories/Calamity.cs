using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class Calamity : ModItem
	{
		public const float MaxNPCSpeed = 5f;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calamity");
			Tooltip.SetDefault("Lights your mouse ablaze with a brimstone fire and causes a flaming aura to appear around it\n" +
				"Enemies near the aura take immense damage and are inflicted with Vulnerability Hex\n" +
				"The mouse fire effect can be shown without the aura and damage effects by putting this item in a vanity slot");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
		}

		public override void SetDefaults()
		{
			item.width = 44;
			item.height = 108;
			item.rare = ItemRarityID.Purple;
			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.expert = true;
			item.vanity = true;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.Calamity().blazingMouseDamageEffects = true;
			player.Calamity().ableToDrawBlazingMouse = true;
		}

		public override void UpdateEquip(Player player) => player.Calamity().ableToDrawBlazingMouse = true;
	}
}
