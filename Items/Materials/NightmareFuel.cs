using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class NightmareFuel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nightmare Fuel");
            Tooltip.SetDefault("May drain your sanity");
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            maxFallSpeed = 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.7f * num, 0.7f * num, 0f);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 2);
            item.Calamity().postMoonLordRarity = 14;
        }
    }
}
