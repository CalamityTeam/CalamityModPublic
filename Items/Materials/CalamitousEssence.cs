using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CalamitousEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitous Essence");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 14;
            item.maxStack = 999;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(gold: 24);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.1f * num, 0.1f * num, 0.1f * num);
        }
    }
}
