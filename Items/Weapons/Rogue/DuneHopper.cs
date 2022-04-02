using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuneHopper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Skipper"); // This will drop from the Sunken Sea Scourge miniboss once it's implemented.
            Tooltip.SetDefault(@"Throws a spear that bounces a lot
Stealth strikes throw three high speed spears");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.damage = 50;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<DuneHopperProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(3);
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX - 3f, speedY - 3f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    int stealth = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<DuneHopperProjectile>(), Math.Max(damage / 3, 1), knockBack, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ScourgeoftheDesert>());
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddIngredient(ModContent.ItemType<PrismShard>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
