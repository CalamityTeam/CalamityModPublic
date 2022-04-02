using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class FathomSwarmerVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fathom Swarmer Visage");
            Tooltip.SetDefault("5% increased minion damage\n" +
                "Provides breathing and light underwater");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 10; //47 +10 underwater
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FathomSwarmerBreastplate>() && legs.type == ModContent.ItemType<FathomSwarmerBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "10% increased minion damage and +2 max minions\n" +
                "Grants the ability to climb walls\n" +
                "30% increased minion damage while submerged in liquid\n" +
                "Provides a moderate amount of light and moderately reduces breath loss in the abyss";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fathomSwarmer = true;
            player.spikedBoots = 2;
            player.maxMinions += 2;
            player.minionDamage += 0.1f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.minionDamage += 0.3f;
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.minionDamage += 0.05f;
            if (player.breath <= player.breathMax + 2 && !modPlayer.ZoneAbyss)
            {
                player.breath = player.breathMax + 3;
            }
            modPlayer.fathomSwarmerVisage = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.3f, 0.9f, 1.35f);
            }

        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiderMask);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<PlantyMush>(), 6);
            recipe.AddIngredient(ModContent.ItemType<AbyssGravel>(), 11);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
