using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ExecutionersBlade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner's Blade");
            Tooltip.SetDefault("Throws a stream of homing blades");
        }

        public override void SafeSetDefaults()
        {
            item.width = 64;
            item.damage = 550;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 3;
            item.useAnimation = 9;
            item.useStyle = 1;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<ExecutionersBladeProj>();
            item.shootSpeed = 26f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(32f, 30f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ExecutionersBladeGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 11);
            recipe.AddRecipeGroup("NForEE", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
