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
            Item.width = 40;
            Item.damage = 47;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<TalonSmallProj>();
            Item.shootSpeed = 10.5f;
            Item.Calamity().rogue = true;
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
            CreateRecipe(1).AddIngredient(ItemID.FeralClaws).AddIngredient(ItemID.ChlorophyteBar, 5).AddIngredient(ItemID.SoulofFright, 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
