using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class PalladiumJavelin : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palladium Javelin");
            Tooltip.SetDefault("Stealth strikes split into more javelins");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.damage = 68;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 19;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.shoot = ProjectileID.StarAnise;
            item.maxStack = 999;
            item.value = 1200;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<PalladiumJavelinProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.425f);

            int javelin = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (javelin.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[javelin].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                if (!player.Calamity().StealthStrikeAvailable())
                    Main.projectile[javelin].usesLocalNPCImmunity = false;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PalladiumBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
