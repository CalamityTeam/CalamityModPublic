using System.Collections.Generic;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class AureusMask : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Aureus Mask");
        }

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 28;
			item.rare = 1;
			item.vanity = true;
		}

		public override bool DrawHead()
		{
			return false;
		}
	}
}