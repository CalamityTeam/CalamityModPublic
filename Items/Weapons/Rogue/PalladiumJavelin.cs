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
            Item.width = 44;
            Item.damage = 68;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 19;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 44;
            Item.shoot = ProjectileID.StarAnise;
            Item.maxStack = 999;
            Item.value = 1200;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<PalladiumJavelinProjectile>();
            Item.shootSpeed = 16f;
            Item.Calamity().rogue = true;
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
            CreateRecipe(100).AddIngredient(ItemID.PalladiumBar).AddTile(TileID.Anvils).Register();
        }
    }
}
