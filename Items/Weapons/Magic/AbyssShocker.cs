using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AbyssShocker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyss Shocker");
            Tooltip.SetDefault("Fires an erratic lightning bolt that arcs and bounces between enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.width = 86;
            Item.height = 32;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.UseSound = SoundID.Item13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 10;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightningArc>();
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + velocity * 4.5f, velocity, ModContent.ProjectileType<LightningArc>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/AbyssShocker_mask").Value);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14, 0);
    }
}
