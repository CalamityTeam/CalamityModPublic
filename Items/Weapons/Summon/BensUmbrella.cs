using CalamityMod.Items.Materials;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BensUmbrella : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Temporal Umbrella");
            Tooltip.SetDefault("Surprisingly sturdy, I reckon this could defeat the Mafia in a single blow\n" +
							   "Summons a magic hat to hover above your head\n" +
                               "The hat will release a variety of objects to assault your foes\n" +
                               "Requires 5 minion slots to use\n" +
                               "There can only be one");
        }

        public override void SetDefaults()
        {
            item.mana = 99;
            item.damage = 1003;
            item.useStyle = 1;
            item.width = 74;
            item.height = 72;
            item.useTime = item.useAnimation = 10;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.Calamity().customRarity = CalamityRarity.Developer;
            item.rare = 10;
            item.UseSound = SoundID.Item68;
            item.shoot = ModContent.ProjectileType<MagicHat>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<MagicHat>() && p.owner == player.whoAmI)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == type)
                {
                    projectile.Kill();
                }
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SpikecragStaff>());
            recipe.AddIngredient(ModContent.ItemType<RadiantResolution>());
            recipe.AddIngredient(ItemID.Umbrella);
            recipe.AddIngredient(ItemID.TopHat);
			recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
