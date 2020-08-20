using Terraria.ModLoader;
using CalamityMod.NPCs.AcidRain;
using Terraria.ID;

namespace CalamityMod.Items.SummonItems
{
	public class BloodwormItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodworm");
		}

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.maxStack = 20;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.bait = 4444;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.consumable = true;
            item.noUseGraphic = true;
            item.makeNPC = (short)ModContent.NPCType<BloodwormNormal>();
        }
    }
}
