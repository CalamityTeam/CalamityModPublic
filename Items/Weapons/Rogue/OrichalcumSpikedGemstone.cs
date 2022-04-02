using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class OrichalcumSpikedGemstone : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orichalcum Spiked Gemstone");
            Tooltip.SetDefault("Stealth strikes last longer and summon petals on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 37;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 13;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 24;
            item.shoot = ProjectileID.StarAnise;
            item.maxStack = 999;
            item.value = 1200;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<OrichalcumSpikedGemstoneProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int gemstone = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (gemstone.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[gemstone].Calamity().stealthStrike = true;
                    Main.projectile[gemstone].usesLocalNPCImmunity = true;
                    Main.projectile[gemstone].timeLeft = 900;
                    Main.projectile[gemstone].penetrate = -1;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.OrichalcumBar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
