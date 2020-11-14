using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RainbowPartyCannon : ModItem
    {
        public static readonly Color[] ColorSet = new Color[]
        {
            new Color(188, 192, 193), // White
            new Color(157, 100, 183), // Purple
            new Color(249, 166, 77), // Honey-ish orange
            new Color(255, 105, 234), // Pink
            new Color(67, 204, 219), // Sky blue
            new Color(249, 245, 99), // Bright yellow
            new Color(236, 168, 247), // Purplish pink
        };
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Party Cannon");
            Tooltip.SetDefault("You should never see this lol");
        }

        public override void SetDefaults()
        {
            item.damage = 2200;
            item.magic = true;
            item.mana = 25;
            item.width = 52;
            item.height = 30;
            item.crit += 4;
            item.useTime = item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Developer;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<RainbowPartyCannonProjectile>();
            item.channel = true;
            item.shootSpeed = 20f;
        }

        public static string DiscoHex => $"c/{(int)MathHelper.Lerp(156f, 255f, Main.DiscoR / 256f):X2}{108:X2}{251:X2}:";

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tt2 = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            tt2.text = $"[" + DiscoHex + "Let the rainbow remind you that together we will always shine...]";
        }
    }
}
