using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BouncySpikyBall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncy Spiky Ball");
            Tooltip.SetDefault(@"Throws a very bouncy ball that richochets off walls and enemies
Receives a small boost in damage and velocity after bouncing off an enemy
Stealth strikes provide a bigger boost after richocheting");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 10;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.consumable = true;
            item.maxStack = 999;
            item.height = 14;
            item.value = Item.buyPrice(0, 0, 1, 0);
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<BouncyBol>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpikyBall, 20);
            recipe.AddIngredient(ItemID.PinkGel);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
