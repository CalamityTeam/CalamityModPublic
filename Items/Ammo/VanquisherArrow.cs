using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
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
            SacrificeTotal = 99;
            DisplayName.SetDefault("Vanquisher Arrow");
            Tooltip.SetDefault("Pierces through tiles\n" +
                "Spawns extra homing arrows as it travels");
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 46;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(copper: 28);
            Item.shoot = ModContent.ProjectileType<VanquisherArrowMain>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Arrow;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Ammo/VanquisherArrowGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient<CosmiliteBar>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
