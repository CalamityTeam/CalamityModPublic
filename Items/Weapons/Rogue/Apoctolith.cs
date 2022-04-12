using Terraria.DataStructures;
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
            Item.damage = 120;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<ApoctolithProj>();
            Item.width = 66;
            Item.height = 64;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.Calamity().rogue = true;
            Item.autoReuse = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Check if stealth is full
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * 1.25f), knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ThrowingBrick>(), 100).AddIngredient(ModContent.ItemType<Voidstone>(), 20).AddIngredient(ModContent.ItemType<SeaPrism>(), 15).AddIngredient(ModContent.ItemType<Lumenite>(), 8).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ApoctolithGlow").Value);
        }
    }
}
