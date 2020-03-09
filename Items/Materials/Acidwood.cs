using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class Acidwood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acidwood");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 22;
            item.maxStack = 999;
            item.value = 0;
        }
    }
}
