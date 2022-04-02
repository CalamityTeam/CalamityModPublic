using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TerrorTalons : RogueWeapon
    {
        private float sign = 1f;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Talons");
            Tooltip.SetDefault("Fires small wavering claws\n" +
            "Stealth strikes launch a large, high speed claw which pierces");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 47;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 7;
            item.useAnimation = 7;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<TalonSmallProj>();
            item.shootSpeed = 10.5f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                float stealthDamageMult = 4.875f;
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<TalonLargeProj>(), (int)(damage * stealthDamageMult), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            else
            {
                // flip flop every standard projectile
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<TalonSmallProj>(), damage, knockBack, player.whoAmI, 0f, sign);
                sign = -sign;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
