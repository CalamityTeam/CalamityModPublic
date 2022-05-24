using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MolecularManipulator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molecular Manipulator");
            Tooltip.SetDefault("Is it nullable or not? Let's find out!\n" +
                "Fires a fast null bullet that distorts NPC stats");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 580;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<NullShot2>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<NullShot2>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NullificationRifle>().
                AddIngredient<DarkPlasma>(2).
                AddIngredient<CoreofCalamity>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
