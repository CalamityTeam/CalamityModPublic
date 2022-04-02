using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Auralis : ModItem
    {
        public static readonly Color blueColor = new Color(0, 77, 255);
        public static readonly Color greenColor = new Color(0, 255, 77);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auralis");
            Tooltip.SetDefault("Fires a high speed glowing bullet that inflicts debilitating debuffs\n" +
                "Right click to zoom out\n" +
                "Standing still provides increasing damage bonuses up to 25%\n" +
                "Standing still for 5 or more seconds while using the scope ability will summon an aurora\n" +
                "The aurora reduces the damage of the next projectile hit by a flat 100\n" +
                "This effect lasts up to 20 seconds and has a 30 second cooldown\n" +
                "50% chance to not consume bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 695;
            item.ranged = true;
            item.useTime = item.useAnimation = 30;
            item.knockBack = 10f;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AuralisBullet>();
            item.shootSpeed = 7.5f;
            item.useAmmo = AmmoID.Bullet;

            item.width = 96;
            item.height = 34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float damageMult = MathHelper.Lerp(0f, 0.25f, player.Calamity().auralisStealthCounter / 300f);
            add += damageMult;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/AuralisGlow"));
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 50;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SniperRifle);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
