using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AstralHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Helm");
            Tooltip.SetDefault("Danger detection");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 40, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.defense = 17; //63
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AstralBreastplate>() && legs.type == ModContent.ItemType<AstralLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased movement speed and +3 max minions\n" +
                "35% increased damage and 25% increased critical strike chance\n" +
                "Whenever you crit an enemy fallen, hallowed, and astral stars will rain down\n" +
                "This effect has a 1 second cooldown before it can trigger again";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.astralStarRain = true;
            player.moveSpeed += 0.05f;
            player.allDamage += 0.35f;
            player.maxMinions += 3;
            modPlayer.AllCritBoost(25);
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.dangerSense = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 8);
            recipe.AddIngredient(ItemID.MeteoriteBar, 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
