using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SulfurHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Helmet");
            Tooltip.SetDefault("8% increased rogue damage\n" +
                "2% increased rogue critical strike chance\n" +
                "Grants underwater breathing");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 2;
            item.defense = 5; 
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SulfurBreastplate>() && legs.type == ModContent.ItemType<SulfurLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Attacking and being attacked by enemies inflicts poison\n" +
                "Grants an additional jump that summons a sulphurous bubble\n" +
                "Provides increased underwater mobility and reduces the severity of the sulphuric waters\n" +
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 100\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sulfurSet = true;
            modPlayer.sulfurJump = true;
            modPlayer.rogueStealthMax += 1f;
            modPlayer.wearingRogueArmor = true;
            player.ignoreWater = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.08f;
            player.Calamity().throwingCrit += 2;
            player.gills = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Acidwood>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UrchinStinger>(), 15);
            recipe.AddIngredient(ModContent.ItemType<SulphurousSand>(), 10);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 10);

            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
