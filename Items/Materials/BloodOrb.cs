using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BloodOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Orb");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 1, copper: 20);
            item.rare = 1;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (item.width / 2)) / 16f), (int)((item.position.Y + (item.height / 2)) / 16f), 0.75f * num, 0f, 0f);
        }
    }
}
