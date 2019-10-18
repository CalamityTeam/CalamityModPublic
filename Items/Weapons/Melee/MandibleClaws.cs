using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MandibleClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mandible Claws");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.damage = 14;
            item.melee = true;
            item.useAnimation = 6;
            item.useStyle = 1;
            item.useTime = 6;
            item.useTurn = true;
            item.knockBack = 3.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 18;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
        }
    }
}
