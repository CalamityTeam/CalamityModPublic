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
            Item.width = 22;
            Item.damage = 15;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<MeteorFistProj>();
            Item.shootSpeed = 5f;
            Item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.MeteoriteBar, 10).AddTile(TileID.Anvils).Register();
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
