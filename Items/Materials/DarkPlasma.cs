using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DarkPlasma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Plasma");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 4));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0.45f * num, 0.7f * num);
        }
    }
}
