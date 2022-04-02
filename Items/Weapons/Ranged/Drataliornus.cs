using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Drataliornus : ModItem
    {
        private const double RightClickDamageRatio = 0.6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drataliornus");
            Tooltip.SetDefault(@"Fires an escalating stream of fireballs.
Fireballs rain meteors, leave dragon dust trails, and launch additional bolts at max speed.
Taking damage while firing the stream will interrupt it and reduce your wing flight time.
Right click to fire two devastating barrages of five empowered fireballs.
'Just don't get hit'");
        }

        public override void SetDefaults()
        {
            item.damage = 129;
            item.knockBack = 1f;
            item.shootSpeed = 18f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 24;
            item.useTime = 12;
            item.reuseDelay = 48;
            item.width = 64;
            item.height = 84;
            item.UseSound = SoundID.Item5;
            item.shoot = ModContent.ProjectileType<DrataliornusBow>();
            item.value = Item.buyPrice(platinum: 2, gold: 50);
            item.rare = ItemRarityID.Red;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.useTurn = false;
            item.useAmmo = AmmoID.Arrow;
            item.autoReuse = true;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noUseGraphic = false;
            }
            else
            {
                item.noUseGraphic = true;
                if (player.ownedProjectileCounts[item.shoot] > 0)
                {
                    return false;
                }
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2) //tsunami
            {
                int flameID = ModContent.ProjectileType<DrataliornusFlame>();
                const int numFlames = 5;
                int flameDamage = (int)(damage * RightClickDamageRatio);

                const float num3 = 0.471238898f;
                Vector2 spinningpoint = new Vector2(speedX, speedY);
                spinningpoint.Normalize();
                spinningpoint *= 36f;
                for (int index1 = 0; index1 < numFlames; ++index1)
                {
                    float num8 = index1 - (numFlames - 1) / 2;
                    Vector2 vector2_5 = spinningpoint.RotatedBy(num3 * num8, new Vector2());
                    Projectile.NewProjectile(position.X + vector2_5.X, position.Y + vector2_5.Y, speedX, speedY, flameID, flameDamage, knockBack, player.whoAmI, 1f, 0f);
                }
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DrataliornusBow>(), 0, 0f, player.whoAmI);
            }

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlossomFlux>());
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 12);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 4);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
