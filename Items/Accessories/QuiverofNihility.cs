using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class QuiverofNihility : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Quiver of Nihility");
            Tooltip.SetDefault("Summons a ring of four void fields to orbit you\n"+"Arrows that pass through these fields gain 20% more damage and double the speed");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (player.Calamity().voidField)
                return false;

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidField = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidFieldGenerator>()] < 4)
                {
                    for (int v = 0; v < 4; v++)
                    {
                        Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<VoidFieldGenerator>(), 0, 0f, Main.myPlayer, v);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("QuiversGroup").
                AddIngredient<DarkPlasma>(3).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
