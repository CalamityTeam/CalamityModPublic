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
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 10; //47 +10 underwater
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
            player.GetDamage(DamageClass.Summon) += 0.1f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.GetDamage(DamageClass.Summon) += 0.3f;
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.GetDamage(DamageClass.Summon) += 0.05f;
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
            CreateRecipe(1).AddIngredient(ItemID.SpiderMask).AddIngredient(ModContent.ItemType<VictideBar>(), 5).AddIngredient(ModContent.ItemType<PlantyMush>(), 6).AddIngredient(ModContent.ItemType<AbyssGravel>(), 11).AddIngredient(ModContent.ItemType<DepthCells>(), 3).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
