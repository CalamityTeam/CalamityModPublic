using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class EvasionScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evasion Scarf");
            Tooltip.SetDefault("True melee strikes deal 15% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a successful dodge you must wait 13 seconds before you can dodge again\n" +
                "This cooldown will be 50 percent longer if you have Chaos State\n" +
                "While on cooldown, Chaos State will be 50 percent longer");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.evasionScarf = true;
            modPlayer.dashMod = 1;
            player.dash = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CounterScarf>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.Silk, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
