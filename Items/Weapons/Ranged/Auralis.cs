using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/PlasmaBlast");
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float damageMult = MathHelper.Lerp(0f, 0.25f, player.Calamity().auralisStealthCounter / 300f);
            damage += damageMult;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AuralisGlow").Value);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Player player) => Main.rand.Next(100) >= 50;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SniperRifle).
                AddIngredient<UeliaceBar>(5).
                AddIngredient<AstralJelly>(5).
                AddIngredient<Stardust>(50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
