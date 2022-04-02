using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class AdamantiteThrowingAxe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Throwing Axe");
            Tooltip.SetDefault("Stealth strikes summon lightning bolts on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 44;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 3.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = 1600;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<AdamantiteThrowingAxeProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AdamantiteBar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
