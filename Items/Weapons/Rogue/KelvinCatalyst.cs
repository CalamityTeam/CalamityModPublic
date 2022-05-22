using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class KelvinCatalyst : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Catalyst");
            Tooltip.SetDefault("Throws an icy blade that splits into multiple ice stars on enemy hits\n" +
            "Stealth strikes will briefly gain sentience and ram nearby enemies before returning to the player");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.damage = 60;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 36);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<KelvinCatalystBoomerang>();
            Item.shootSpeed = 8f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<IceStar>(100).
                AddIngredient<Avalanche>().
                AddIngredient<EffluviumBow>().
                AddIngredient<GlacialCrusher>().
                AddIngredient<Icebreaker>().
                AddIngredient<SnowstormStaff>().
                AddIngredient<EssenceofEleum>(10).
                AddTile(TileID.IceMachine).
                Register();
        }
    }
}
