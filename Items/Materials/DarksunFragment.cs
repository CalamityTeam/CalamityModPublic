using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DarksunFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darksun Fragment");
            Tooltip.SetDefault("A shard of lunar and solar energy");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 12);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.5f * num, 0.5f * num);
        }
    }
}
