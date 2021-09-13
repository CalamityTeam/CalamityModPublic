using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class HellcasterFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharon Soul Fragment");
            Tooltip.SetDefault("A shard of a godly soul");
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.3f * num, 0.05f * num);
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 45);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }
    }
}
