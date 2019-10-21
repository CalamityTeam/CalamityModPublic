using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CosmiliteBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmilite Bar");
            Tooltip.SetDefault("A chunk of highly-resistant cosmic steel");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0f * num, 0.5f * num);
        }
    }
}
