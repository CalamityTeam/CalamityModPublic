using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AccretionDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Disk");
            Tooltip.SetDefault("Throws a disk that has a chance to generate several disks if enemies are near it\n" +
            "Stealth strikes fly slower but travel farther, pierce through enemies, and spawn extra disks more frequently");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 100;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.shoot = ModContent.ProjectileType<AccretionDiskProj>();
            item.shootSpeed = 13f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                speedX *= 0.7f;
                speedY *= 0.7f;
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].timeLeft *= 3;
                    Main.projectile[proj].localNPCHitCooldown *= 2;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MangroveChakram>());
            recipe.AddIngredient(ModContent.ItemType<FlameScythe>());
            recipe.AddIngredient(ModContent.ItemType<TerraDisk>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
