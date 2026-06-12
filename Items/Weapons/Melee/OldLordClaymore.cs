using CalamityMod.Items.BaseItems;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("OldLordOathsword")]
    public class OldLordClaymore : CustomUseProjItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3];
        }

        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 76;
            Item.damage = 100;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.crit = 11;
            Item.useAnimation = Item.useTime = 90; // Yes it's actually supposed to be this slow

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<OldLordClaymoreHoldout>();
            Item.useTurn = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OldLordClaymoreGlow").Value);
        }
        public override bool MeleePrefix() => true;
    }
}
