using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverHelm : ModItem
    {
        //Defense and DR Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Helm");
            Tooltip.SetDefault("15% increased damage reduction but 30% decreased damage\n" +
                "+50 max life\n" +
                "Passively regenerates one health point every second");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 30;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 30; //63 => 73 w/ set bonus (+5 w/ Reaver Rage)
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.thorns += 0.33f;
            player.moveSpeed -= 0.2f;
            player.statDefense += 10;
            player.lifeRegen += 3;
            player.aggro += 600;
            modPlayer.reaverDefense = true;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "+10 defense and +3 life regen\n" +
            "Enemies are more likely to target you\n" +
            "Reduces the life regen lost from damage over time debuffs by 20%\n" +
            "All attacks have a small chance to steal life and speed up the rate of life regen\n" +
            "20% decreased movement speed and flight time\n" +
            "Enemy damage is reflected and summons a thorn spike\n" +
            "Reaver Rage has a 25% chance to activate when you are damaged";
            //Reaver Rage provides 30% damage to offset the helm "bonus", 5 def, and 5% melee speed.
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage -= 0.3f;
            player.endurance += 0.15f;
            player.Calamity().reaverRegen = true;
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 6);
            recipe.AddIngredient(ItemID.JungleSpores, 4);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
