using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaWireHemmedVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Wire-Hemmed Visage");
            Tooltip.SetDefault("20% increased magic damage and critical strike chance, +100 max mana, and 20% reduced mana usage\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 24; //132
            item.Calamity().postMoonLordRarity = 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AuricTeslaBodyArmor") && legs.type == mod.ItemType("AuricTeslaCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Magic Tarragon, Bloodflare, God Slayer, and Silva armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 10%";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerMage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.manaCost *= 0.8f;
            player.magicDamage += 0.2f;
            player.magicCrit += 20;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaMaskedCap");
            recipe.AddIngredient(null, "GodSlayerVisage");
            recipe.AddIngredient(null, "BloodflareHornedMask");
            recipe.AddIngredient(null, "TarragonMask");
            recipe.AddIngredient(null, "AuricOre", 60);
            recipe.AddIngredient(null, "EndothermicEnergy", 10);
            recipe.AddIngredient(null, "NightmareFuel", 10);
            recipe.AddIngredient(null, "Phantoplasm", 8);
            recipe.AddIngredient(null, "DarksunFragment", 6);
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 5);
            recipe.AddIngredient(null, "CoreofCalamity", 2);
            recipe.AddIngredient(null, "GalacticaSingularity");
            recipe.AddIngredient(null, "PsychoticAmulet");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
