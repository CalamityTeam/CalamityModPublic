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
            Item.damage = 129;
            Item.knockBack = 1f;
            Item.shootSpeed = 18f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 12;
            Item.reuseDelay = 48;
            Item.width = 64;
            Item.height = 84;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ModContent.ProjectileType<DrataliornusBow>();
            Item.value = Item.buyPrice(platinum: 2, gold: 50);
            Item.rare = ItemRarityID.Red;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.useTurn = false;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = false;
            }
            else
            {
                Item.noUseGraphic = true;
                if (player.ownedProjectileCounts[Item.shoot] > 0)
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BlossomFlux>()).AddIngredient(ModContent.ItemType<EffulgentFeather>(), 12).AddIngredient(ModContent.ItemType<HellcasterFragment>(), 4).AddIngredient(ModContent.ItemType<AuricBar>(), 5).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
