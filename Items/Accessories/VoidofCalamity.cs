using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CalamityRing")]
    public class VoidofCalamity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int BrimstoneFlamesDmg => CalamityUtils.ScaleWithDifficulty(15);
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            Item.expert = true;
        }
        int cooldown = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidOfCalamity = true;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.HasIFrames())
                {
                    if (cooldown <= 0)
                    {
                        cooldown = 20;
                        int damage = (int)player.GetBestClassDamage().ApplyTo(BrimstoneFlamesDmg);
                        for (var i = 0; i < 2; i++)
                            CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 5.5f, ModContent.ProjectileType<StandingFire>(), damage, 5f, player.whoAmI);
                    }
                }
                cooldown--;
            }
        }
    }
}
