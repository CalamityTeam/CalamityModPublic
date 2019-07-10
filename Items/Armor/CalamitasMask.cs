using System.Collections.Generic;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class CalamitasMask : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas Mask");
        }

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 20;
			item.rare = 1;
			item.vanity = true;
		}

		public override bool DrawHead()
		{
			return false;
		}
	}
}
