using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SarosPossession : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saros Possession");
            Tooltip.SetDefault("Gain absolute control over light itself\n" +
                               "Can only be summoned once\n" +
                               "Uses 8 minion slots");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.knockBack = 4f;
            Item.mana = 10;

            Item.shoot = ModContent.ProjectileType<SarosAura>();

            Item.width = Item.height = 56;
            Item.useTime = Item.useAnimation = 10;

            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_BetsyFlameBreath;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sirius>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
