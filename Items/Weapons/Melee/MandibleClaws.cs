using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MandibleClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mandible Claws");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 7;
            Item.useTurn = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
