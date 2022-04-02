using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EventHorizon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Event Horizon");
            Tooltip.SetDefault("Nothing, not even light, can return.\n" +
            "Fires a ring of stars to home in on nearby enemies\n" +
            "Stars spawn black holes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 46;

            item.damage = 275;
            item.knockBack = 3.5f;
            item.noMelee = true;
            item.magic = true;
            item.mana = 12;

            item.useTime = item.useAnimation = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;

            item.UseSound = SoundID.Item84;
            item.shoot = ModContent.ProjectileType<EventHorizonStar>();
            item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (float i = 0; i < 8; i++)
            {
                float angle = MathHelper.TwoPi / 8f * i;
                Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, type, damage, knockBack, player.whoAmI, angle, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Starfall>());
            recipe.AddIngredient(ModContent.ItemType<NuclearFury>());
            recipe.AddIngredient(ModContent.ItemType<RelicofRuin>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 8);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
