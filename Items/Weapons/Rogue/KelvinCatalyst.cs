using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class KelvinCatalyst : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Catalyst");
            Tooltip.SetDefault("Throws an icy blade that splits into multiple ice stars on enemy hits\n" +
            "Stealth strikes will briefly gain sentience and ram nearby enemies before returning to the player");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 60;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 30;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.height = 20;
            item.value = Item.buyPrice(gold: 36);
            item.rare = ItemRarityID.Pink;
            item.Calamity().donorItem = true;
            item.shoot = ModContent.ProjectileType<KelvinCatalystBoomerang>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IceStar>(), 100);
            recipe.AddIngredient(ModContent.ItemType<Avalanche>());
            recipe.AddIngredient(ModContent.ItemType<EffluviumBow>());
            recipe.AddIngredient(ModContent.ItemType<GlacialCrusher>());
            recipe.AddIngredient(ModContent.ItemType<Icebreaker>());
            recipe.AddIngredient(ModContent.ItemType<SnowstormStaff>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 10);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
