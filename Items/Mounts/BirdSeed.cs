using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class BirdSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Folly Feed");
            Tooltip.SetDefault("Summons a monstrosity");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 36;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;

            item.value = Item.sellPrice(gold: 20);
            item.rare = ItemRarityID.Purple;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.NPCHit51;
            item.noMelee = true;
            item.mountType = ModContent.MountType<BUMBLEDOGE>();
        }
    }
}
