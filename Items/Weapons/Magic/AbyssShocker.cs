using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AbyssShocker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<StaticDischarge>()];
        }
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 32;
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useAnimation = Item.useTime = 19;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<LightningArc>();
            Item.shootSpeed = 14f;

            Item.UseSound = SoundID.Item13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => position += velocity.SafeNormalize(Vector2.UnitX) * 56f;

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/AbyssShocker_mask").Value);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14, 0);
    }
}
