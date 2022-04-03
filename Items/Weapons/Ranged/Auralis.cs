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
            Item.damage = 695;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuralisBullet>();
            Item.shootSpeed = 7.5f;
            Item.useAmmo = AmmoID.Bullet;

            Item.width = 96;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float damageMult = MathHelper.Lerp(0f, 0.25f, player.Calamity().auralisStealthCounter / 300f);
            add += damageMult;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), Item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AuralisGlow"));
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 50;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SniperRifle).AddIngredient(ModContent.ItemType<UeliaceBar>(), 5).AddIngredient(ModContent.ItemType<AstralJelly>(), 5).AddIngredient(ModContent.ItemType<Stardust>(), 50).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
