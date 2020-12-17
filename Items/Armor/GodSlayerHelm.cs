using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Horned Greathelm");
            Tooltip.SetDefault("14% increased melee damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.defense = 48; //96
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerDamage = true;
            player.setBonus = "You will survive fatal damage and will be healed 150 HP if an attack would have killed you\n" +
                "This effect can only occur once every 45 seconds\n" +
                "While the cooldown for this effect is active all life regen is halved\n" +
                "Taking over 80 damage in one hit will cause you to release a swarm of high-damage god killer darts\n" +
                "Enemies take a lot of damage when they hit you\n" +
                "An attack that would deal 80 damage or less will have its damage reduced to 1";
            player.thorns += 2.5f;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.14f;
            player.meleeCrit += 14;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 14);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
