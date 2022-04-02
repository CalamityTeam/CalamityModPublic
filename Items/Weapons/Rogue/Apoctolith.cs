using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Apoctolith : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Maybe catching bricks with your face isn't such a hot idea...\n" +
                "Critical hits tear away enemy defense\n" +
                "Stealth strikes shatter and briefly stun enemies");
            DisplayName.SetDefault("Apoctolith");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 120;
            item.shootSpeed = 15f;
            item.shoot = ModContent.ProjectileType<ApoctolithProj>();
            item.width = 66;
            item.height = 64;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 10f;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
            item.autoReuse = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Check if stealth is full
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 1.25f), knockBack, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ThrowingBrick>(), 100);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 20);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ApoctolithGlow"));
        }
    }
}
