using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class VanquisherArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vanquisher Arrow");
            Tooltip.SetDefault("Pierces through tiles\n" +
                "Spawns extra homing arrows as it travels");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.ranged = true;
            item.width = 22;
            item.height = 46;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3.5f;
            item.value = Item.sellPrice(copper: 28);
            item.shoot = ModContent.ProjectileType<VanquisherArrowMain>();
            item.shootSpeed = 10f;
            item.ammo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Ammo/VanquisherArrowGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>());
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this, 999);
            recipe.AddRecipe();
        }
    }
}
