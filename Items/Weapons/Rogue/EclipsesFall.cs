using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EclipsesFall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            Tooltip.SetDefault("When the sun goes dark, you will know judgment\n" +
            "Summons spears from the sky on hit\n" +
            "Stealth strikes impale enemies and summon a constant barrage of spears over time");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 564;
            item.knockBack = 3.5f;
            item.useAnimation = item.useTime = 21;
            item.autoReuse = true;
            item.Calamity().rogue = true;
            item.shootSpeed = 15f;
            item.shoot = ModContent.ProjectileType<EclipsesFallMain>();

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = item.height = 72;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                type = ModContent.ProjectileType<EclipsesStealth>();

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NightsGaze>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 12);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 8);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
