using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BlazingStar : RogueWeapon
    {
        public const float Speed = 13f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Star");
            Tooltip.SetDefault("Stacks up to 4\n" +
                               "Stealth strikes release all stars at once with infinite piercing");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 129;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item1;
            item.maxStack = 4;

            item.shootSpeed = Speed;
            item.shoot = ModContent.ProjectileType<BlazingStarProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                if (item.stack != 1)
                {
                    damage = (int)(damage * 1.55f);

                    for (int i = 0; i < item.stack; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-MathHelper.ToRadians(8f), MathHelper.ToRadians(8f), i / (float)(item.stack - 1)));
                        Projectile proj = Projectile.NewProjectileDirect(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                        if (proj.whoAmI.WithinBounds(Main.maxProjectiles))
                            proj.Calamity().stealthStrike = true;

                        Projectile projectile = Projectile.NewProjectileDirect(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                        if (projectile.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            projectile.penetrate = -1;
                            projectile.Calamity().stealthStrike = true;
                        }

                    }
                    return false;
                }
            }
            return true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < item.stack;
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Glaive>(), 1);
            recipe.AddIngredient(ItemID.HellstoneBar, 3);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
