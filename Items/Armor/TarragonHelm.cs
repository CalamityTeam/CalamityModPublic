using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class TarragonHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Helm");
            Tooltip.SetDefault("Temporary immunity to lava and immunity to cursed inferno, fire, cursed, and chilled debuffs\n" +
                "Can move freely through liquids\n" +
                "5% increased damage reduction\n" +
                "10% increased melee damage and critical strike chance\n" +
                "Helm of the disciple of ancients");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 50, 0, 0);
            item.defense = 33; //98
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            player.setBonus = "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
                "You have a 25% chance to gain a life regen buff when you take damage\n" +
                "Press Y to cloak yourself in life energy that heavily reduces enemy contact damage for 10 seconds\n" +
                "This has a 30 second cooldown";
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
            player.endurance += 0.05f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Chilled] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 7);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
