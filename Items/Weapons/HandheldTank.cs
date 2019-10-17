using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class HandheldTank : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Handheld Tank");
        }

        public override void SetDefaults()
        {
            item.width = 110;
            item.height = 46;
            item.ranged = true;
            item.damage = 1850;
            item.crit += 15;
            item.knockBack = 16f;
            item.useTime = 71;
            item.useAnimation = 71;
            item.autoReuse = true;

            item.useStyle = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TankCannon");
            item.noMelee = true;

            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;

            item.shoot = ModContent.ProjectileType<Projectiles.HandheldTankShell>();
            item.shootSpeed = 6f;
            item.useAmmo = AmmoID.Rocket;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.HandheldTankShell>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-33, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(null, "Shroomer");
            r.AddIngredient(ItemID.IronBar, 50);
            r.anyIronBar = true;
            r.AddIngredient(null, "DivineGeode", 5);
            r.AddIngredient(ItemID.TigerSkin);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
