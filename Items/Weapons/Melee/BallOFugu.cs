using CalamityMod.Projectiles.Melee.MaceFlails;
using CalamityMod.Systems.Collections;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BallOFugu : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static readonly SoundStyle BlowSound = new("CalamityMod/Sounds/Item/FuguBlow") { PitchVariance = 0.1f };

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Poisoned];
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 10;
            Item.damage = 25;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<BallOFuguProj>();
            Item.shootSpeed = 12f;
        }
    }
}
