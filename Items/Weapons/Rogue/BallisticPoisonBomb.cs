using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BallisticPoisonBomb : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ballistic Poison Bomb");
            Tooltip.SetDefault("Throws a sticky bomb that explodes into spikes and poison clouds\n" +
            "Stealth strikes throw three at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 30;
            item.damage = 50;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 26;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<BallisticPoisonBombProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 5;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position, perturbedspeed, type, Math.Max(damage / 3, 1), knockBack, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= Main.rand.Next(2,6);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeafoamBomb>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<SulphurousSand>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
