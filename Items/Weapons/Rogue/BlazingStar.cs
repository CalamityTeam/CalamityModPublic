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
            Item.damage = 129;
            Item.Calamity().rogue = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 4;

            Item.shootSpeed = Speed;
            Item.shoot = ModContent.ProjectileType<BlazingStarProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                if (Item.stack != 1)
                {
                    damage = (int)(damage * 1.55f);

                    for (int i = 0; i < Item.stack; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-MathHelper.ToRadians(8f), MathHelper.ToRadians(8f), i / (float)(Item.stack - 1)));
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

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < Item.stack;
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Glaive>(), 1).AddIngredient(ItemID.HellstoneBar, 3).AddIngredient(ModContent.ItemType<EssenceofChaos>(), 4).AddTile(TileID.Anvils).Register();
        }
    }
}
