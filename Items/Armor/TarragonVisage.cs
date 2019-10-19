using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class TarragonVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Visage");
            Tooltip.SetDefault("Temporary immunity to lava and immunity to cursed inferno, fire, cursed, and chilled debuffs\n" +
                "Can move freely through liquids\n" +
                "10% increased ranged damage and critical strike chance\n" +
                "5% increased damage reduction");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 50, 0, 0);
            item.defense = 21; //98
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
            modPlayer.tarraRanged = true;
            player.setBonus = "Reduces enemy spawn rates\n" +
                "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
                "Ranged critical strikes will cause an explosion of leaves\n" +
                "Ranged projectiles have a chance to split into life energy on death";
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.1f;
            player.rangedCrit += 10;
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
