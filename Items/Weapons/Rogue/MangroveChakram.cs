using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MangroveChakram : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mangrove Chakram");
            Tooltip.SetDefault("Stealth strikes leave a trail of lingering flowers");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 84;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 14;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.height = 38;
            item.value = Item.buyPrice(gold: 60);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<MangroveChakramProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.25f);

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
