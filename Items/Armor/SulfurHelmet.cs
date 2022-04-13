using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SulfurHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Helmet");
            Tooltip.SetDefault("4% increased rogue damage\n" +
                "2% increased rogue critical strike chance\n" +
                "Grants underwater breathing");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 95\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sulfurSet = true;
            modPlayer.sulfurJump = true;
            modPlayer.rogueStealthMax += 0.95f;
            modPlayer.wearingRogueArmor = true;
            player.ignoreWater = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.04f;
            player.Calamity().throwingCrit += 2;
            player.gills = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UrchinStinger>(15).
                AddIngredient<Acidwood>(10).
                AddIngredient<SulphurousSand>(10).
                AddIngredient<SulfuricScale>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
