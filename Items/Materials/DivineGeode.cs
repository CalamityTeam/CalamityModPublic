using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DivineGeode : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divine Geode");
            Tooltip.SetDefault("A chunk of crystallized holy energy");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.rare = ItemRarityID.Purple;
            item.value = Item.buyPrice(0, 6, 50, 0);
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.45f * num, 0.3f * num, 0f * num);
        }
    }
}
