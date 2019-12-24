using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MetalMonstrosity : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Metal Monstrosity");
            Tooltip.SetDefault("Hurls a heavy metal ball that shatters on impact \n" +
                               "Stealth Strikes makes the ball release spikes as it travels \n" +
                               "'This has to hurt'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.rare = 3;
            item.value = Item.buyPrice(0, 4, 0, 0);

            item.damage = 30;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<MetalChunk>();
            item.shootSpeed = 7f;

            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<MetalChunk>(), damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[proj].Calamity().stealthStrike = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpikyBall, 500);
            recipe.AddIngredient(ItemID.Spike, 80);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
