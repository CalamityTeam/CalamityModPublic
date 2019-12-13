using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TerrorTalons : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Talons");
            Tooltip.SetDefault("Fires small wavering claws\n" +
            "Stealth strikes launch a bigger, faster claw that flies straight and pierces indefinitely");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 103;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.shoot = ModContent.ProjectileType<TalonSmallProj>();
            item.shootSpeed = 12.5f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                float stealthDamageMult = 1.1f;
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<TalonLargeProj>(), (int)(damage * stealthDamageMult), knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}
