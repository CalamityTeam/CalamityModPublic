using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MagnomalyCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnomaly Cannon");
            Tooltip.SetDefault("Launches a powerful exo rocket to nuke anything and everything\n" +
                "Rockets are surrounded by an invisible damaging aura and split into damaging beams on hit\n" +
                "66% chance to not consume rockets");
        }

        public override void SetDefaults()
        {
            item.damage = 357;
            item.ranged = true;
            item.width = 84;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 9.5f;
            item.UseSound = SoundID.Item11;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MagnomalyRocket>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Rocket;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, -10);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MagnomalyRocket>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 66)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ThePack>());
            recipe.AddIngredient(ModContent.ItemType<BlissfulBombardier>());
            recipe.AddIngredient(ModContent.ItemType<AethersWhisper>());
            recipe.AddIngredient(ItemID.ElectrosphereLauncher);
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
