using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
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
            item.damage = 33;
            item.ranged = true;
            item.width = 22;
            item.height = 46;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3.5f;
            item.value = 2250;
            item.shoot = ModContent.ProjectileType<VanquisherArrow>();
            item.shootSpeed = 10f;
            item.ammo = 40;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(11f, 21f);
            spriteBatch.Draw(mod.GetTexture("Items/Weapons/VanquisherArrowGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar");
            recipe.AddIngredient(null, "NightmareFuel");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar");
            recipe.AddIngredient(null, "EndothermicEnergy");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
