using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class PhantomLance : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Lance");
            Tooltip.SetDefault(@"Fires a spectral javelin that rapidly releases lost souls
Fades away and slows down over time
Lost souls released later deal less damage
Stealth strikes don't slow down and souls always deal full damage");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 70;
            item.knockBack = 5f;

            item.width = 62;
            item.height = 68;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Yellow;
            item.useTime = 23;
            item.useAnimation = 23;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.consumable = true;
            item.Calamity().rogue = true;

            item.autoReuse = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<PhantomLanceProj>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 1.75f), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpectreBar, 2);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>());
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
