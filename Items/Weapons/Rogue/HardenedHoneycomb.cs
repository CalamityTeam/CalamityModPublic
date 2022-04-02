using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class HardenedHoneycomb : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hardened Honeycomb");
            Tooltip.SetDefault(@"Fires a honeycomb that shatters into fragments
Grants the honey buff to players it touches
Stealth strikes can bounce off walls and enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 30;
            item.damage = 25;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 32;
            item.maxStack = 999;
            item.value = 300;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<Honeycomb>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].penetrate = 3;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Hive);
            recipe.AddIngredient(ItemID.CrispyHoneyBlock);
            recipe.AddIngredient(ItemID.BeeWax);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
