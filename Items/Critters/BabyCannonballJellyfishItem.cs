using CalamityMod.NPCs.Abyss;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyCannonballJellyfishItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        //Intentionally NOT bait due to bestiary entry on them being used as ammunition, although dynamite fishing in terraria does sound pretty fun
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 30, 0);
            Item.width = 26;
            Item.height = 24;
            Item.makeNPC = (short)ModContent.NPCType<BabyCannonballJellyfish>();
            Item.rare = ItemRarityID.Green;
        }
    }
}
