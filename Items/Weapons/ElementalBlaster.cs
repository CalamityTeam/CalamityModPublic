using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ElementalBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Blaster");
            Tooltip.SetDefault("Does not consume ammo\n" +
                "Fires a storm of rainbow blasts");
        }

        public override void SetDefaults()
        {
            item.damage = 77;
            item.ranged = true;
            item.width = 104;
            item.height = 42;
            item.useTime = 2;
            item.useAnimation = 6;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.75f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<RainbowBlast>();
            item.shootSpeed = 18f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SpectralstormCannon");
            recipe.AddIngredient(null, "ClockGatlignum");
            recipe.AddIngredient(null, "PaintballBlaster");
            recipe.AddIngredient(null, "GalacticaSingularity", 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
