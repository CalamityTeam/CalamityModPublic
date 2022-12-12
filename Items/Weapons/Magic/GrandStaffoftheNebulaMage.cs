using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GrandStaffoftheNebulaMage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Staff of the Nebula Mage");
            Tooltip.SetDefault("The true power of the Nebula Mage rests in your hands...");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 120;
            Item.height = 124;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<GrandStaffoftheNebulaMage_Held>();
            Item.shootSpeed = 9f;
        }

        public override void OnConsumeMana(Player player, int manaConsumed) => player.statMana += manaConsumed;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NebulaArcanum).
                AddIngredient(ItemID.NebulaBlaze).
                AddIngredient<CosmiliteBar>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
