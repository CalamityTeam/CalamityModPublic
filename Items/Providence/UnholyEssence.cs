using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Providence
{
    public class UnholyEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unholy Essence");
            Tooltip.SetDefault("The essence of profaned creatures");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.rare = 10;
			item.value = Item.buyPrice(0, 6, 50, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            maxFallSpeed = 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.45f * num, 0.3f * num, 0f * num);
        }
    }
}
