using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FlameScythe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subduction Slicer");
            Tooltip.SetDefault("Throws a scythe that explodes on enemy hits\n" +
            "Stealth strikes also summon an orange pillar of fire on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 50;
            item.height = 48;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.useAnimation = 20;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(gold: 80);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<FlameScytheProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
