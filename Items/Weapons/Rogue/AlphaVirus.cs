using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AlphaVirus : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alpha Virus");
            Tooltip.SetDefault("Throws a giant plague cell with a lethal aura that splits into 6 plague seekers on death\n" +
                               "Stealth strikes cause the plague cell to move slower, accumulating an aura of swirling plague seekers as it flies");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 333;
            item.width = 44;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 31;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 31;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.maxStack = 1;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.shoot = ModContent.ProjectileType<AlphaVirusProjectile>();
            item.shootSpeed = 4f;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, item.shootSpeed, 0f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EpidemicShredder>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

    }
}
