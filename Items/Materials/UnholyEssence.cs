using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class UnholyEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Unholy Essence");
            Tooltip.SetDefault("The essence of profaned creatures");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(0, 6, 50, 0);
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.45f * num, 0.3f * num, 0f * num);
        }
    }
}
