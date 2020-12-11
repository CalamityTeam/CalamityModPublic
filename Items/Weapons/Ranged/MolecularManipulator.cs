using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
                "Fires a fast null bullet that distorts NPC stats\n" +
                "Uses your life as ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 890;
            item.ranged = true;
            item.width = 56;
            item.height = 34;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 25f;
            item.shoot = ModContent.ProjectileType<NullShot2>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.statLife -= 5;
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.Male ? player.name + " was vaporized by the imbuement of his life." : player.name + " was vaporized by the imbuement of her life."), 1000.0, 0, false);
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<NullShot2>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NullificationRifle>());
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 2);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
