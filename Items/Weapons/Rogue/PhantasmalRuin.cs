using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class PhantasmalRuin : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Ruin");
            Tooltip.SetDefault(@"Fires an enormous ghost lance that emits lost souls as it flies
Explodes into tormented souls on enemy hits
Stealth strikes continuously leave spectral clones in their wake");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 1180;
            item.knockBack = 8f;

            item.width = 102;
            item.height = 98;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 35;
            item.useAnimation = 35;
            item.autoReuse = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<PhantasmalRuinProj>();
            item.UseSound = SoundID.Item1;
            item.Calamity().rogue = true;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LuminousStriker>());
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<PhantomLance>(), 500);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
