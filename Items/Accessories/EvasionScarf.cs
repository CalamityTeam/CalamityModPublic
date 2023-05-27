using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class EvasionScarf : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<TrueMeleeDamageClass>() += 0.15f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.evasionScarf = true;
            modPlayer.DashID = CounterScarfDash.ID;
            player.dashType = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CounterScarf>().
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddIngredient(ItemID.Silk, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
