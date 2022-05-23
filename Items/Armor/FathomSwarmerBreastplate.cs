using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class FathomSwarmerBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Fathom Swarmer Breastplate");
            Tooltip.SetDefault("6% increased damage reduction\n" +
                "6% increased minion damage\n" +
                "Boosted defense and regen increased while submerged in liquid\n" +
                "Reduces defense loss within the Abyss");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.06f;
            player.endurance += 0.06f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.statDefense += 10;
                player.lifeRegen += 5;
            }
            player.Calamity().fathomSwarmerBreastplate = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpiderBreastplate).
                AddIngredient<SeaRemains>(12).
                AddIngredient<PlantyMush>(10).
                AddIngredient<AbyssGravel>(18).
                AddIngredient<DepthCells>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
