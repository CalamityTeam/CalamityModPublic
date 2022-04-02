using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MeteorFist : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Fist");
            Tooltip.SetDefault("Fires a fist that explodes \n" +
                               "Stealth strikes make the fist ricochet between enemies up to 4 times");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.damage = 15;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 30;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.height = 28;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<MeteorFistProj>();
            item.shootSpeed = 5f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<MeteorFistProj>(), damage, knockBack, player.whoAmI, 0f, 4f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
