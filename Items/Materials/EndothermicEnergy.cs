using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
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
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f, 0f, 1.2f * num);
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
