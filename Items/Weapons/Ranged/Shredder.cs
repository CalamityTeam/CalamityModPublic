using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shredder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shredder");
            Tooltip.SetDefault("The myth, the legend, the weapon that drops more frames than any other\n" +
                "Fires a barrage of energy bolts that split and bounce\n" +
                "Right click to fire a barrage of normal bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 27;
            item.ranged = true;
            item.width = 56;
            item.height = 24;
            item.useTime = 4;
            item.reuseDelay = 12;
            item.useAnimation = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int bulletAmt = 4;
            if (player.altFunctionUse == 2)
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = speedX + Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = speedY + Main.rand.Next(-30, 31) * 0.05f;
                    int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
            else
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = speedX + Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = speedY + Main.rand.Next(-30, 31) * 0.05f;
                    int shredderBoltDamage = (int)(0.85f * damage);
                    int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ChargedBlast>(), shredderBoltDamage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ChargedDartRifle>());
            recipe.AddIngredient(ModContent.ItemType<FrostbiteBlaster>());
            recipe.AddIngredient(ItemID.Shotgun);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
