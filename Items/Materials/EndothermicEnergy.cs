using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Materials
{
	public class EndothermicEnergy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endothermic Energy");
            Tooltip.SetDefault("Great for preventing heat stroke");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0f, 0f, 1.2f * brightness);
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
