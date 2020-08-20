using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Materials
{
	public class NightmareFuel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nightmare Fuel");
            Tooltip.SetDefault("May drain your sanity");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.7f * brightness, 0.7f * brightness, 0f);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 2);
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }
    }
}
