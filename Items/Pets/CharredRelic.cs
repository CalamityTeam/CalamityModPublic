using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Buffs.Pets;

namespace CalamityMod.Items.Pets
{
    public class CharredRelic : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Relic");
            Tooltip.SetDefault("Contains a small amount of brimstone");
        }

        public override void SetDefaults()
        {
            item.shoot = ModContent.ProjectileType<BrimlingPet>();
            item.buffType = ModContent.BuffType<BrimlingBuff>();
            item.rare = 4;
            item.UseSound = SoundID.NPCHit51;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
