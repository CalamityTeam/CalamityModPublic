using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Starmageddon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.knockBack = 4f;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 30;
            Item.width = 166;
            Item.height = 62;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType<StarmageddonHeld>();
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/StarmageddonGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StarmageddonHeld>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Starfleet>().
                AddIngredient<BarracudaGun>().
                AddIngredient<ExodiumCluster>(15).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
