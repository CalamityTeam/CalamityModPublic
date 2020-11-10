using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TriactisTruePaladinianMageHammerofMight : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triactis' True Paladinian Mage-Hammer of Might");
            Tooltip.SetDefault("Explodes on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 160;
            item.damage = 10000;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.knockBack = 50f;
            item.UseSound = SoundID.Item1;
            item.height = 160;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<TriactisOPHammer>();
            item.shootSpeed = 25f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceRogue = true;
			Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalaxySmasherRogue>());
            recipe.AddIngredient(ItemID.SoulofMight, 30);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
